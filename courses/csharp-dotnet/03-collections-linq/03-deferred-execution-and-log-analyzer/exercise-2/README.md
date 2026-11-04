# Exercise — Live views and snapshots

See deferred execution for yourself.

1. `LiveEvens`: a query of the even numbers that sees later changes to the list. Don't materialize it.
2. `SnapshotEvens`: the even numbers right now, unaffected by later changes.
3. `CountCalls`: walk `LiveEvens`-style query twice, and return how many times the `Where` predicate ran in total. The method is written for you except for the query and the two walks.
