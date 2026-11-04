# Exercise — Which lifetime?

See DI lifetimes behave differently. `Tracker` gives each instance a new id.

Register the two services so that:

- `SessionTracker` is **one instance for the whole app**
- `RequestTracker` is **a new instance for every request**

`GET /ids` is written for you. Then the tests compare two requests.
