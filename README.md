# ProductsAPI

A RESTful ASP.NET Core Web API for managing Products, built for the LRQA Dev Challenge. Uses EF Core with SQLite, a repository pattern for data access, and custom middleware for request logging.

## Tech stack

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core (SQLite)
- Repository pattern + Dependency Injection
- xUnit + Moq (unit tests)
- Swagger / OpenAPI

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later

That's it — no database server, and no manual migration step required (see below).

## Getting started

1. Clone the repo and move into it:

   ```bash
   git clone <repo-url>
   cd LRQA-ProductsAPI
   ```

2. Restore dependencies:

   ```bash
   dotnet restore
   ```

3. Run the API:

   ```bash
   dotnet run --project LRQA-ProductsAPI
   ```

   By default this listens on `http://localhost:5159` (see [`launchSettings.json`](LRQA-ProductsAPI/Properties/launchSettings.json) for the `https` profile too). Open **http://localhost:5159/swagger** for interactive Swagger UI covering every endpoint.

   On startup, the app automatically applies any pending EF Core migrations (`dbContext.Database.Migrate()` in [`Program.cs`](LRQA-ProductsAPI/Program.cs)), creating a local SQLite `products.db` file (inside `LRQA-ProductsAPI/`) and seeding it with a few sample rows the first time it runs. The connection string lives in [`LRQA-ProductsAPI/appsettings.json`](LRQA-ProductsAPI/appsettings.json) under `ConnectionStrings:DefaultConnection`.

   If you'd rather apply migrations manually (e.g. to inspect the SQL, or run them independently of starting the app), install the EF Core CLI tool and run:

   ```bash
   dotnet tool install --global dotnet-ef
   dotnet ef database update --project LRQA-ProductsAPI
   ```

### Running the tests

```bash
dotnet test
```

Runs the unit tests in `LRQA-ProductsAPI.Tests`, which cover the controller's behaviour (success/not-found paths for each endpoint, and that `POST` always ignores a client-supplied `Id`) against a mocked repository.

## API endpoints

Base URL: `http://localhost:5159/api/products`

### `GET /api/products`

Returns all products.

```bash
curl http://localhost:5159/api/products
```

```json
[
  { "id": 1, "name": "Running Shoes", "price": 129.99, "stock": 150 },
  { "id": 2, "name": "Walking Boots", "price": 89.99, "stock": 75 }
]
```

### `GET /api/products/{id}`

Returns a single product, or `404` if the id doesn't exist.

```bash
curl http://localhost:5159/api/products/1
```

```json
{ "id": 1, "name": "Running Shoes", "price": 129.99, "stock": 150 }
```

### `POST /api/products`

Creates a new product. `id` is always database-generated — any `id` sent in the body is ignored. `name`, `price` and `stock` are validated (`name` required, `price` > 0, `stock` >= 0); an invalid body returns `400`.

```bash
curl -X POST http://localhost:5159/api/products \
  -H "Content-Type: application/json" \
  -d '{"name":"Flip Flops","price":19.99,"stock":45}'
```

Returns `201 Created` with the new product (including its generated `id`) and a `Location` header pointing at `GET /api/products/{id}`.

### `PUT /api/products/{id}`

Updates an existing product. Returns `204 No Content` on success, `404` if the id doesn't exist.

```bash
curl -X PUT http://localhost:5159/api/products/1 \
  -H "Content-Type: application/json" \
  -d '{"id":1,"name":"Running Shoes","price":139.99,"stock":120}'
```

### `DELETE /api/products/{id}`

Deletes a product. Returns `204 No Content` on success, `404` if the id doesn't exist.

```bash
curl -X DELETE http://localhost:5159/api/products/1
```

## Project structure

```
LRQA-ProductsAPI/
  Controllers/    ProductController - the 5 CRUD endpoints
  Data/           AppDbContext - EF Core context, model config, seed data
  Middleware/     RequestLoggingMiddleware - logs method/path/status/duration per request
                  ErrorHandlingMiddleware - catches unhandled exceptions, returns a consistent JSON error response
  Migrations/     EF Core code-first migrations
  Models/         Product entity, with validation attributes
  Repositories/   IProductRepository / ProductRepository - data access layer
LRQA-ProductsAPI.Tests/
  ProductControllerTests.cs - unit tests for the controller, against a mocked repository
```
