---
layout: default
title: Wouter on Architecture
---

Random musings mostly on (Cloud) Architecture, Azure and .NET.

{% for post in site.posts %}
## [{{ post.title }}]({{ post.url | relative_url }})
<div class="post-list-date">{{ post.date | date: "%B %-d, %Y" }}</div>
{% if post.excerpt != post.content %}
{{ post.excerpt }}
{% endif %}

{% endfor %}
