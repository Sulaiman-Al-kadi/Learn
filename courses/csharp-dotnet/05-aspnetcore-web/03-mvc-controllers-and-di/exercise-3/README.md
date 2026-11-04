# Exercise — Products controller

Write an MVC controller, `ProductsController`, at the route `/products`, with the injected `IProductStore`.

1. `GET /products`: all products.
2. `GET /products/{id}`: the product, or `404`.
3. `POST /products`: adds a product from the JSON body and responds `201`.

The service and `AddControllers` wiring are done. The controller goes at the bottom of the file.
