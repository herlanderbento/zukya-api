<center>
  <p align="center">
    <img src="https://user-images.githubusercontent.com/20674439/158480674-3b8895e7-420e-4025-bd78-8058ba255476.png"  width="150" />
  </p>  
  <h1 align="center">🚀 Zukya API with .NET</h1>
  <p align="center">
    Using Clean Architecture, DDD and the main current market best practices
  </p>
</center>
<br />

## About

Zukya is a REST API built with .NET 10, following **Clean Architecture** and **Domain-Driven Design**. It is the backend of a marketplace: sellers, products, orders, payments, wallet, and related flows. Business rules stay in the domain, use cases in the application layer, and PostgreSQL, file storage, email, and authentication are plugged in through infrastructure.

The solution applies CQRS (MediatR), FluentValidation, JWT, and SOLID so each layer stays testable and independent.

## Modules

The solution is split into four projects:

| Module | Responsibility |
| --- | --- |
| **Zukya.Domain** | Core of the business: entities, aggregate roots, value objects, domain events, validations, and repository contracts. No dependencies on other layers. |
| **Zukya.Application** | Use cases and application services: CQRS handlers, pagination, and ports for storage, mail, cryptography, and unit of work. |
| **Zukya.Infra** | External implementations: EF Core with PostgreSQL, AWS S3, JWT, and BCrypt. |
| **Zukya.Api** | ASP.NET Core host: HTTP endpoints, OpenAPI, auth configuration, and composition root that wires the modules together. |

### Backend modules

- [ ] Authentication
- [ ] Users
- [ ] Sellers / Stores
- [ ] Products
- [ ] Categories
- [ ] Orders
- [ ] Payments
- [ ] Wallet
- [ ] Chat
- [ ] Reviews
- [ ] Notifications
- [ ] Advertising

## How to run?

- Simply clone the Repository:

```sh
git clone https://github.com/herlanderbento/zukya-api.git
```

- Then run the solution file with Rider

<br />

## Tools needed

- Rider or Visual Studio 2022
- .NET 10 SDK installed
- Docker or Docker Desktop

## Running with Docker

To start the application and database using Docker, use the following command:

```sh
docker-compose up -d
```

This will spin up the necessary containers for the application and database.

## Database Migrations

### Create a Migration

To create a new migration, use the following command with the `--output-dir` parameter:

```sh
dotnet ef migrations add MigrationName --project ./src/Zukya.Infra --context DatabaseContext --output-dir EF/Migrations
```

**Note:** The `--output-dir EF/Migrations` parameter is required because Entity Framework Core doesn't support automatic directory configuration through `.csproj` properties.

### Apply Migrations to Database

To apply all pending migrations to the database:

```sh
dotnet ef database update --project ./src/Zukya.Infra --context DatabaseContext
```

### Drop Database

To drop the entire database:

```sh
dotnet ef database drop --project ./src/Zukya.Infra --context DatabaseContext
```

To drop the database without confirmation prompt:

```sh
dotnet ef database drop --project ./src/Zukya.Infra --context DatabaseContext --force
```