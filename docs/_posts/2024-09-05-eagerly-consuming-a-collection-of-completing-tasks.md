---
layout: post
title: 'Eagerly consuming a collection of completing Tasks'
date: 2024-09-05 07:13:00
permalink: /eagerly-consuming-a-collection-of-completing-tasks/
---

<p>Imagine you start a bunch of files (say, uploading or downloading files big and small) and after the first one completes, you want to do something else -- how do you <code>await</code> that first task?</p>


<p><strong>NOTE: The below code is not production ready. I recommend looking into .NET 9's Task.WhenEach()</strong></p>

<p>There are many existing data structures in .NET, but most are FIFO based. In this case, I don't care about the order they were added ('produced'), I care about whether the <code>Task</code> is completed, and I want consume the completed ones:</p>

<pre><code>var taskCollection = new ConcurrentConsumingTaskCollection&lt;string&gt;();

taskCollection.Add(UploadFileAsync("largefile"));
taskCollection.Add(UploadFileAsync("otherlargefile"));
taskCollection.Add(UploadFileAsync("smallfile"));
taskCollection.Add(UploadFileAsync("supersmallfile"));

// Signal that no more tasks will be added
taskCollection.CompleteAdding();

// Consume the completed uploads as they finish
await foreach (var result in taskCollection.ConsumingEnumerable())
{
   // i want them in order supersmallfile &gt; smallfile &gt; largefile / otherlargefile
   // NOT in FIFO order largefile otherlargefile smallfile supersmallfile
   Console.WriteLine($"Processed: {result}");
}</code></pre>

<h2>Additional considerations</h2>

<ol>
<li><strong>Concurrent Producers and Consumers</strong>: Multiple producers should be able to add tasks to the system concurrently, while multiple consumers should be able to process tasks as they complete.</li>

<li><strong>Completion-Based Task Processing</strong>: Tasks should be processed as soon as they complete, regardless of the order in which they were added.</li>

<li><strong>Signalling completion</strong>: Once all tasks have been added and processed, the system should gracefully stop. The system should also prevent any new tasks from being added once it has been signaled that no more tasks will be added.</li>

<li><strong>Thread-Safe Management</strong>: Task addition and consumption should be thread-safe to prevent race conditions or duplicate processing.</li>
</ol>

<h2>Existing Solutions &amp; Their Limitations</h2>

<p>At first glance, several existing .NET constructs may seem like potential candidates for this problem, such as <strong><a href="https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library">the Task Parallel Library Dataflow</a></strong>, <strong><a href="https://devblogs.microsoft.com/dotnet/an-introduction-to-system-threading-channels/">System.Threading.Channels</a></strong>, or its predecessor <strong><a href="https://learn.microsoft.com/en-us/dotnet/standard/collections/thread-safe/blockingcollection-overview">BlockingCollection</a></strong>. However, upon closer inspection, they are all FIFO.</p>

<h2>Designing the <code>ConcurrentConsumingTaskCollection</code></h2>

<p>After evaluating the existing options, I decided to design a custom collection that would satisfy all our requirements: the <strong><code>ConcurrentConsumingTaskCollection&lt;T&gt;</code></strong>.</p>

<h3><strong>1. Core Data Structure: Channels</strong></h3>

<p>The collection uses <code>System.Threading.Channels</code> as the central mechanism for managing task production and consumption. Channels are designed for producer-consumer scenarios and allow multiple producers to add tasks while enabling multiple consumers to process them concurrently. This works perfectly for the collection’s goal of handling tasks as they complete, as opposed to FIFO-based queues that handle tasks in the order they are added.</p>

<h3>2. <strong>Completion-Based Task Processing: Leveraging <code>ContinueWith</code></strong></h3>

<p>The <code>Add()</code> method leverages <code>Task.ContinueWith</code> to trigger a continuation that adds the <code>Task</code> to the <code>Channel</code>. The continuation runs <strong>as soon as the task completes</strong>, regardless of its order of addition.</p>

<p>This approach ensures tasks are processed in the order they <strong>finish</strong>, not the order they are added. The completed task is written to the channel, where consumers can pick it up.</p>

<h3>3. <strong>Signaling Completion</strong></h3>

<p>A key requirement of the system is that it gracefully shuts down once all tasks are completed. This is handled by the <code>CompleteAdding()</code> method, which signals that no more tasks will be added. However, the system also ensures that tasks still in progress at the time of this call are allowed to complete.</p>

<p>By tracking the number of active tasks with <code>activeTaskCount</code>, the system only completes the channel when the task count reaches zero (i.e., when all tasks have been completed). This allows consumers to finish consuming tasks and prevents new tasks from being added after completion is signaled.</p>

<p>Check out the latest code on <a href="https://github.com/woutervanranst/utils/blob/main/src/WouterVanRanst.Utils/Collections/ConcurrentConsumingTaskCollection.cs">WouterVanRanst.Utils on GitHub</a> or the snippet below (!) may be out of date.</p>

<pre><code>using System.Runtime.CompilerServices;
using System.Threading.Channels;

/// &lt;summary&gt;
/// A thread-safe collection of tasks that are consumed as they complete.
/// Tasks can be added concurrently by multiple producers and consumed by multiple consumers.
/// The collection processes tasks in the order they complete, regardless of the order they were added.
/// &lt;/summary&gt;
/// &lt;typeparam name="T"&gt;The type of the result returned by the tasks.&lt;/typeparam&gt;
public sealed class ConcurrentConsumingTaskCollection&lt;T&gt;
{
    private readonly Channel&lt;Task&lt;T&gt;&gt; channel = Channel.CreateUnbounded&lt;Task&lt;T&gt;&gt;(new UnboundedChannelOptions { AllowSynchronousContinuations = false, SingleReader = false, SingleWriter = false });

    private bool addingCompleted = false;
    private int activeTaskCount = 0;

    public void Add(Task&lt;T&gt; task)
    {
        if (addingCompleted)
            throw new InvalidOperationException("Cannot add tasks after completion.");

        Interlocked.Increment(ref activeTaskCount);

        task.ContinueWith(async t =&gt;
        {
            await channel.Writer.WriteAsync(t);

            // Decrement active task count and complete the writer if done
            if (Interlocked.Decrement(ref activeTaskCount) == 0 &amp;&amp; addingCompleted)
            {
                channel.Writer.Complete();
            }
        }, TaskContinuationOptions.ExecuteSynchronously);
    }

    public void CompleteAdding()
    {
        addingCompleted = true;

        if (Interlocked.CompareExchange(ref activeTaskCount, 0, 0) == 0)
        {
            channel.Writer.Complete();
        }
    }

    public bool IsCompleted =&gt; addingCompleted &amp;&amp; activeTaskCount == 0 &amp;&amp; channel.Reader.Completion.IsCompleted;

    public async IAsyncEnumerable&lt;Task&lt;T&gt;&gt; GetConsumingEnumerable(&#91;EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var t in channel.Reader.ReadAllAsync(cancellationToken))
            yield return t;
    }
}</code></pre>
