# Exercise — Options class

Read settings through the Options pattern.

`Mailer` takes `IOptions<MailSettings>`.

1. `From`: the configured sender address.
2. `IsConfigured`: true when `Host` is not empty and `Port` is above 0.
3. `Describe`: `host:port`, or `not configured`.
