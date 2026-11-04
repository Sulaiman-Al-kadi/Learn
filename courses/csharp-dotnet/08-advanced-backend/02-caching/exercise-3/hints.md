## Hint 1
`_cache.Set("rate", value, _ttl)` stores with an expiry.

## Hint 2
`_cache.TryGetValue("rate", out decimal value)` reads without fetching.
