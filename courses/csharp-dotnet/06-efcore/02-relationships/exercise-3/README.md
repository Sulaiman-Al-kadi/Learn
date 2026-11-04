# Exercise — Delete with children

Decide what happens to children when a parent goes away. By default, deleting a `Customer` also deletes their `Order`s, which is a cascade delete.

1. `DeleteCustomerAsync`: delete the customer and return how many orders went with them, or -1 when the customer doesn't exist.
2. `BigSpendersAsync`: names of customers whose orders total more than `limit`, A to Z.
