# Exercise — API key gate

Protect part of the app with middleware.

Any path starting with `/admin` needs the header `X-Api-Key: secret`. Without it, respond `401` and stop the pipeline. Every other path passes through.
