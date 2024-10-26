# Goal

Proof of concept for a monorepo solving of the problems you might face in an ecommerce backend. The solution will have a high performance cart solution and product lookup. If time allows a simple promotion engine might be included.

## Implementation details

The solution will use the actor model for high performance throughput. Things that could be considered an actor are:

* A product
* A cart
* A purchase order
* A payment
* A fulfillment process

An actor should have a finite lifetime and also a timeout associated to it. If it is passive for x minutes it should be timed out, but you can wake it up with the next call.

## Features

[ ] - Get product item - need a product db and product actors for fast lookups in the cluster
[ ] - Add item to cart
[ ] - Increase quantity
[ ] - Decrease quantity
[ ] - Remove item from cart
[ ] - Create purchase order, going to checkout, referencing a cart - cart can still be changed
[ ] - Calculate prices based on items in cart
[ ] - Add contact information to purchase order
[ ] - Add payment - should lock referenced cart
[ ] - Authorize payment
[ ] - Create fulfillment order
[ ] - Simple admin UI showing active carts and purchase orders and where admins can modify the cart and orders.

## High-level tech stack

* A postgres database will be used to store the data, and marten db will be used for event sourcing of the different processes. The products will just be simple lookups.
* Frontend will be a simple next application in this same repo.
* Backend will be dotnet using Microsoft Orleans.
* Scalar, https://github.com/scalar/scalar/blob/main/packages/scalar.aspnetcore/README.md, will be used for create a nice looking OpenAPI documentation.

## Local development

The solution is utilizing https://github.com/dotnet/aspire to ease the local development. To run the application locally all you need to run is 

```
dotnet run --project src/aspire/MonoStore.AppHost/MonoStore.AppHost.csproj
```

This should run aspire and start all the different services and connect them together.

## Testing

Not at all completed yet, but when done test containers, https://dotnet.testcontainers.org, should be used for performant end to end testing over the APIs.

## Production

Another goal with the application is to explore how it is to host a distributed orleans application on a hosted platform. This means either Azure Web Apps or Azure Container Service will be used. This way there will be no need to maintain a kubernetes cluster. For production database Cosmos Postgres will be used.