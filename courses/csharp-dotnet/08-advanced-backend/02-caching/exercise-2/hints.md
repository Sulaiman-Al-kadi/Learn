## Hint 1
Build a key per product, such as `$"price:{id}"`.

## Hint 2
`_cache.GetOrCreateAsync(key, entry => { ... })` does the check and the store. Set `entry.AbsoluteExpirationRelativeToNow` inside.

## Hint 3
`_cache.Remove(key)` invalidates.
