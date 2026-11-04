## Hint 1
`Channel.CreateBounded<string>(capacity)` creates it. Add `using System.Threading.Channels;`.

## Hint 2
`_channel.Writer.TryWrite(message)` and `_channel.Reader.ReadAsync()` are the two ends. `ReadAsync` returns a `ValueTask`, so `await` it.
