---
title: SleekGrid Home
---

### Classic Samples (to check backward compatibility)

{% assign classic_examples_pages = site.pages | where: "dir", "/examples/classic/" %}

{% for p in classic_examples_pages %}
- [{{ p.title }}]({{ site.baseurl }}{{ p.url | replace: ".html", "" }}){% endfor %}
