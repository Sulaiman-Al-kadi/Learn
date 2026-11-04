# Exercise — Cache per key

Apply cache-aside with one cache entry per product.

1. `GetAsync(id)`: return the cached price. On a miss, call `_source` and cache the result for 5 minutes.
2. `Invalidate(id)`: forget one product's price, and leave the others cached.
