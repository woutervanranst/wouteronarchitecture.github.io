---
layout: post
title: 'From Layers to Rings – Hexagonal Architectures Explained (by Silas Graffy)'
date: 2025-01-12 18:03:07
permalink: /from-layers-to-rings-hexagonal-architectures-explained-by-silas-graffy/
---

<p>This article is a proper English translation of <a href="https://www.maibornwolff.de/know-how/von-schichten-zu-ringen-hexagonale-architekturen-erklaert/">Von Schichten zu Ringen – Hexagonale Architekturen erklärt by Silas Graffy</a>. I found it to be one of the best explanations and motivations for the architecture style, but the 'Chrome Autotranslation' was severely lacking. </p>

<p>In summary:</p>

<ol class="wp-block-list">
<li>Classical <strong>layered </strong>architectures do not work: they put the database as a foundation and they tend to evolve into 'everything talks to everything spaghetti'.</li>

<li>In the mid-90's, building on the Dependency Inversion Principle, Alistair Cockburn coined the <strong>hexagonal </strong>architecture: it decouples the core application logic from external concerns (such as databases).</li>

<li>Because six sides was rather random, the name was changed in 2005 to <strong>Ports and Adapters</strong>.</li>

<li>If we add the internal/external notion, we arrive at the <strong>Onion Architecture</strong> (Jeffrey Palermo in 2008).</li>

<li>This inspired <strong>Clean Architecture</strong> (Robert C. Martin in 2012), as a more generalized form of the Onion Architecture.</li>
</ol>

<p>Enjoy the read.</p>

<!--more-->

<h3 class="wp-block-heading">Why Layers Aren’t Enough…</h3>

<p>Let’s begin with something we’re all familiar with: a classic layered architecture. A user interacts with the application’s domain layer through the UI component, which in turn accesses a database via a Data Abstraction Layer (DAL).</p>

<figure class="wp-block-image aligncenter size-large is-resized"><img src="/wp-content/uploads/2016/05/1.png" alt="" style="width:154px;height:auto"/></figure>

<p>However, such an architecture conveys a misleading image:<strong> the database as the foundation of the software architecture</strong>—reminiscent of the days when software design began with an entity-relationship model. Moreover, dependencies exist from the domain layer's business code to the technical code in the DAL, as shown by the relationships between the classes involved (simplified in the diagram, with just one class per layer).</p>

<figure class="wp-block-image aligncenter size-large is-resized"><img src="/wp-content/uploads/2016/05/2.png" alt="" style="width:408px;height:auto"/></figure>

<p>This dependency often forces us to adapt our business code even when only the technical infrastructure changes—a frequent scenario in long-lived software systems. Conversely, implementing such business improvements becomes much simpler if no external dependencies—on technical infrastructure or otherwise—exist.</p>

<h3 class="wp-block-heading">…And What Can Be Done About It</h3>

<p>So, what does the clever software crafter who wants to decouple the domain and DAL do? They extract an interface (a good idea anyway, for testing and mocking purposes):</p>

<figure class="wp-block-image aligncenter size-large is-resized"><img src="/wp-content/uploads/2016/05/3.png" alt="" style="width:400px;height:auto"/></figure>

<p>While the business code in the domain layer no longer depends on the technical implementation in the DAL class, nothing changes yet at the level of component dependencies. The Dependency Inversion Principle (DIP) offers a solution here (beautifully explained by Robert C. Martin in his blog post <em><a href="/wp-content/uploads/2016/05/8.png">A Little Architecture</a></em>, presented as a fictional debate):</p>

<figure class="wp-block-image aligncenter size-large is-resized"><img src="/wp-content/uploads/2016/05/4.png" alt="" style="width:390px;height:auto"/></figure>

<p>The crucial step in dependency inversion is to ensure the interface becomes part of the domain model—e.g., by naming methods in a business, rather than technical, manner. The resulting component dependencies look like this:</p>

<figure class="wp-block-image aligncenter size-large is-resized"><img src="/wp-content/uploads/2016/05/5.png" alt="" style="width:178px;height:auto"/></figure>

<hr class="wp-block-separator has-alpha-channel-opacity"/>

<h3 class="wp-block-heading">Inside and Outside, Instead of Top and Bottom</h3>

<p>Most software systems have more than just a user interface and a database (as a persistence medium). Often, there are APIs to expose the software’s functionality to other systems (e.g., via a REST gateway), logging to files or other storage, email notifications for certain events, and more. By organizing the code needed for these purposes—where necessary, incorporating the DIP—around the domain layer, the resulting architecture might look like this:</p>

<figure class="wp-block-image aligncenter size-large is-resized"><img src="/wp-content/uploads/2016/05/6.png" alt="" style="width:462px;height:auto"/></figure>

<p>In the mid-1990s, Alistair Cockburn began visualizing this kind of architecture with a <a href="http://alistair.cockburn.us/Hexagonal+architecture">hexagon</a>:</p>

<figure class="wp-block-image aligncenter size-large is-resized"><img src="/wp-content/uploads/2016/05/7.png" alt="" style="width:496px;height:auto"/></figure>

<p>As the six sides seemed more or less arbitrary, the name was changed in 2005 to Ports and Adapters. In this nomenclature, the interfaces within the application core (domain) represent the "ports," while their implementations in the outer hexagon act as adapters between the application core and users, databases, log files, external systems, etc.</p>

<hr class="wp-block-separator has-alpha-channel-opacity"/>

<h3 class="wp-block-heading">Layers 2.0</h3>

<p>If we abandon the hexagonal representation and instead make a finer distinction between "internal" and "external," we arrive at the <a href="http://jeffreypalermo.com/blog/the-onion-architecture-part-1/">Onion Architecture</a> proposed by Jeffrey Palermo in 2008:</p>

<figure class="wp-block-image aligncenter size-large"><img src="/wp-content/uploads/2016/05/8.png" alt=""/></figure>

<p>At its <strong>core lies a domain model</strong> that describes the application's business components without dependencies. Surrounding it is the <strong>domain services </strong>ring, containing business logic that spans multiple elements of the domain model. This layer depends solely on the domain model and represents the entirety of the business logic. <strong>Application services</strong>, which implement application-specific logic (e.g., access control), utilize this business logic. <strong>Infrastructure, user interfaces, APIs</strong>, and tests reside in an outer layer.</p>

<p>Dependencies always flow from the outside inward, never the other way around. This applies not just to code dependencies but also to data formats. For example, a string whose format is defined by an external system should never be interpreted in a layer closer to the core than infrastructure. Instead, inner layers define formats—or better yet, custom data types—that the string is mapped to by the adapter in the infrastructure layer.</p>

<p>Robert C. Martin's 2012 <em><a href="https://blog.8thlight.com/uncle-bob/2012/08/13/the-clean-architecture.html">Clean Architecture</a></em> can be viewed as a derivative of the Onion Architecture, differing mainly in terminology.</p>

<p>Advantages of this architectural pattern include:</p>

<ul class="wp-block-list">
<li>Business logic can be compiled, deployed, and reused independently of the infrastructure.</li>

<li>Application logic can be utilized by different UIs, batch jobs, daemons/services, and tests alike.</li>

<li>The application core remains independent of external systems.</li>

<li>Persistence mechanisms can be swapped out depending on deployment scenarios with ease.</li>

<li>Business and technical code are consistently separated.</li>
</ul>

<h3 class="wp-block-heading">Microservices, Self-contained Systems, and Domain-Driven Design</h3>

<p>For microservices and self-contained systems (SCS), the Onion Architecture is a natural fit. Each service or SCS is implemented as its own onion. From one system’s perspective, all others are external and must not represent dependencies in the inner layers.</p>

<p>In Domain-Driven Design (DDD), each onion represents a bounded context, each with its own ubiquitous language. A context map describes their relationships. Translation code, if required by the type of relationship, can be implemented as its own onion layer (e.g., an anti-corruption layer). Layers can, of course, be extended as needed. With "entities" (not to be confused with DDD entities), Robert C. Martin’s Clean Architecture describes the implementation of DDD’s shared kernel concept. Application and domain services contain elements also called services and repositories in DDD, while the domain model includes DDD entities, value objects, and aggregate roots.</p>

<h3 class="wp-block-heading">Conclusion</h3>

<p>The architectural style described here—called Hexagonal, Ports and Adapters, Onion, or Clean Architecture depending on the source—is a consistent evolution of layered architectures through dependency inversion. It keeps business code in the application core independent of technical code in UIs, tests, infrastructure, etc.</p>

<p>Like Domain-Driven Design, hexagonal architectures are well-suited for designing and sustainably implementing long-lived software systems with complex business logic. However, for throwaway software, prototypes, simple CRUD systems, and other use cases where rapid application development is often employed, the required architectural investment may not be justified.</p>
