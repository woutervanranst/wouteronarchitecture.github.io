---
layout: post
title: 'Eagerly consuming a collection of completing Tasks'
date: 2024-09-05 07:13:00
permalink: /eagerly-consuming-a-collection-of-completing-tasks/
---

Imagine you start a bunch of files (say, uploading or downloading files big and small) and after the first one completes, you want to do something else -- how do you `await` that first task?

**NOTE: The below code is not production ready. I recommend looking into .NET 9's Task.WhenEach()**

There are many existing data structures in .NET, but most are FIFO based. In this case, I don't care about the order they were added ('produced'), I care about whether the `Task` is completed, and I want consume the completed ones:

```
var taskCollection = new ConcurrentConsumingTaskCollection<string>();

taskCollection.Add(UploadFileAsync("largefile"));
taskCollection.Add(UploadFileAsync("otherlargefile"));
taskCollection.Add(UploadFileAsync("smallfile"));
taskCollection.Add(UploadFileAsync("supersmallfile"));

// Signal that no more tasks will be added
taskCollection.CompleteAdding();

// Consume the completed uploads as they finish
await foreach (var result in taskCollection.ConsumingEnumerable())
{
   // i want them in order supersmallfile > smallfile > largefile / otherlargefile
   // NOT in FIFO order largefile otherlargefile smallfile supersmallfile
   Console.WriteLine($"Processed: {result}");
}
```

## Additional considerations

1.  **Concurrent Producers and Consumers**: Multiple producers should be able to add tasks to the system concurrently, while multiple consumers should be able to process tasks as they complete.
2.  **Completion-Based Task Processing**: Tasks should be processed as soon as they complete, regardless of the order in which they were added.
3.  **Signalling completion**: Once all tasks have been added and processed, the system should gracefully stop. The system should also prevent any new tasks from being added once it has been signaled that no more tasks will be added.
4.  **Thread-Safe Management**: Task addition and consumption should be thread-safe to prevent race conditions or duplicate processing.

## Existing Solutions & Their Limitations

At first glance, several existing .NET constructs may seem like potential candidates for this problem, such as **[the Task Parallel Library Dataflow](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library)**, **[System.Threading.Channels](https://devblogs.microsoft.com/dotnet/an-introduction-to-system-threading-channels/)**, or its predecessor **[BlockingCollection](https://learn.microsoft.com/en-us/dotnet/standard/collections/thread-safe/blockingcollection-overview)**. However, upon closer inspection, they are all FIFO.

## Designing the `ConcurrentConsumingTaskCollection`

After evaluating the existing options, I decided to design a custom collection that would satisfy all our requirements: the **`ConcurrentConsumingTaskCollection<T>`**.

### 1. Core Data Structure: Channels

The collection uses `System.Threading.Channels` as the central mechanism for managing task production and consumption. Channels are designed for producer-consumer scenarios and allow multiple producers to add tasks while enabling multiple consumers to process them concurrently. This works perfectly for the collection’s goal of handling tasks as they complete, as opposed to FIFO-based queues that handle tasks in the order they are added.

### 2. Completion-Based Task Processing: Leveraging `ContinueWith`

The `Add()` method leverages `Task.ContinueWith` to trigger a continuation that adds the `Task` to the `Channel`. The continuation runs **as soon as the task completes**, regardless of its order of addition.

This approach ensures tasks are processed in the order they **finish**, not the order they are added. The completed task is written to the channel, where consumers can pick it up.

### 3. Signaling Completion

A key requirement of the system is that it gracefully shuts down once all tasks are completed. This is handled by the `CompleteAdding()` method, which signals that no more tasks will be added. However, the system also ensures that tasks still in progress at the time of this call are allowed to complete.

By tracking the number of active tasks with `activeTaskCount`, the system only completes the channel when the task count reaches zero (i.e., when all tasks have been completed). This allows consumers to finish consuming tasks and prevents new tasks from being added after completion is signaled.

Check out the latest code on [WouterVanRanst.Utils on GitHub](https://github.com/woutervanranst/utils/blob/main/src/WouterVanRanst.Utils/Collections/ConcurrentConsumingTaskCollection.cs) or the snippet below (!) may be out of date.

```
using System.Runtime.CompilerServices;
using System.Threading.Channels;

/// <summary>
/// A thread-safe collection of tasks that are consumed as they complete.
/// Tasks can be added concurrently by multiple producers and consumed by multiple consumers.
/// The collection processes tasks in the order they complete, regardless of the order they were added.
/// </summary>
/// <typeparam name="T">The type of the result returned by the tasks.</typeparam>
public sealed class ConcurrentConsumingTaskCollection<T>
{
    private readonly Channel<Task<T>> channel = Channel.CreateUnbounded<Task<T>>(new UnboundedChannelOptions { AllowSynchronousContinuations = false, SingleReader = false, SingleWriter = false });

    private bool addingCompleted = false;
    private int activeTaskCount = 0;

    public void Add(Task<T> task)
    {
        if (addingCompleted)
            throw new InvalidOperationException("Cannot add tasks after completion.");

        Interlocked.Increment(ref activeTaskCount);

        task.ContinueWith(async t =>
        {
            await channel.Writer.WriteAsync(t);

            // Decrement active task count and complete the writer if done
            if (Interlocked.Decrement(ref activeTaskCount) == 0 && addingCompleted)
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

    public bool IsCompleted => addingCompleted && activeTaskCount == 0 && channel.Reader.Completion.IsCompleted;

    public async IAsyncEnumerable<Task<T>> GetConsumingEnumerable([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var t in channel.Reader.ReadAllAsync(cancellationToken))
            yield return t;
    }
}
```
