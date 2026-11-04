# Exercise — Short-lived results

Cached data goes stale, so give it a lifetime.

`GetRate` caches the value from `_fetch` for the `ttl` given to the constructor. When the entry expires, the next call fetches again.

Also write `TryGetCached`, which returns the cached value without ever fetching.
