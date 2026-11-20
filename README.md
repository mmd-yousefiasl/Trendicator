# Trendicator

> A backend API for a trading-tools eCommerce platform — built with ASP.NET Core, Clean Architecture, and PostgreSQL.

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?style=flat&logo=dotnet&logoColor=white)](https://learn.microsoft.com/en-us/aspnet/core/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=flat&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-512BD4?style=flat&logo=dotnet&logoColor=white)](https://learn.microsoft.com/en-us/ef/core/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

---

## 📖 Table of Contents

- [About The Project](#-about-the-project)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Features](#-features)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [API Overview](#-api-overview)
- [Design Decisions](#-design-decisions)
- [Database & Migrations](#-database--migrations)
- [Testing](#-testing)
- [Roadmap](#-roadmap)
- [Contributing](#-contributing)
- [License](#-license)
- [Contact](#-contact)

---

## 📖 About The Project

**Trendicator** is a RESTful backend service designed to power an eCommerce platform for trading tools, indicators, and digital products. It provides a scalable, maintainable foundation for managing users, products, orders, and third-party integrations (such as TradingView), while adhering to **Clean Architecture** principles.

The project was built to demonstrate how a modern .NET backend can be structured for long-term maintainability — with clear separation of concerns, testable business logic, and a clean, versionable API layer.

### Why this project exists

- To serve as a production-style reference for **Clean Architecture in ASP.NET Core**
- To practice **DTO-based API contracts**, **dependency injection**, and **layered validation**
- To integrate with a real third-party service (TradingView) without coupling the core domain to it
- To work with **PostgreSQL** and **EF Core** in a realistic eCommerce domain

---

## 🏗 Architecture

The solution follows **Clean Architecture** and is split into five projects, each with a single responsibility:

```
Trendicator.sln
├── Trendicator.Domain          # Entities, enums, domain interfaces (no dependencies)
├── Trendicator.Application     # Use cases, DTOs, service interfaces
├── Trendicator.Data            # EF Core DbContext, repositories, migrations
├── Trendicator.IOC             # Dependency injection registration
└── Trendicator.Web             # ASP.NET Core Web API (controllers, middleware)
```

### Dependency direction

```
Web  ──►  Application  ──►  Domain
 │            ▲
 ▼            │
IOC ──►  Data ┘
```

- **Domain** has **no dependencies** on any other project or external package.
- **Application** depends only on **Domain**.
- **Data** implements interfaces defined in **Domain/Application**.
- **Web** depends on **Application** and **IOC**.
- **IOC** wires everything together and is referenced by **Web**.

### Layer responsibilities

| Layer | Responsibility |
|-------|----------------|
| **Domain** | Core business entities, enums, and repository/service contracts. Pure C#, no EF Core or ASP.NET references. |
| **Application** | Use cases, DTOs, mapping logic, and application-level service interfaces. Orchestrates domain objects. |
| **Data** | EF Core `DbContext`, entity configurations, repository implementations, and migrations. |
| **IOC** | Centralized service registration (`AddApplication`, `AddData`, etc.) to keep `Program.cs` clean. |
| **Web** | Controllers, middleware, filters, and API configuration. Handles HTTP concerns only. |

---

## 🛠 Tech Stack

| Category | Technologies |
|----------|--------------|
| **Runtime** | .NET 10 |
| **Framework** | ASP.NET Core Web API |
| **Language** | C# |
| **Architecture** | Clean Architecture, Layered Design, DTO pattern, Dependency Injection |
| **ORM** | Entity Framework Core |
| **Database** | PostgreSQL (primary), previously SQL Server |
| **External Integration** | TradingView API (product data) |
| **API Documentation** | Swagger / OpenAPI |
| **Version Control** | Git |

---

## ✨ Features

- **User Management** — User registration, profile management, and persistence via EF Core.
- **Product Catalog** — Manage trading tools, indicators, and digital products.
- **Order Processing** — Create and manage orders for digital products.
- **TradingView Integration** — Sync product information with TradingView's product flow.
- **DTO-Based API Contracts** — Request/response models are decoupled from domain entities.
- **Dependency Injection** — Configured centrally via the `IOC` project.
- **Layered Validation** — Domain invariants plus DTO-level validation for robust input handling.
- **PostgreSQL Persistence** — Fully migrated from SQL Server to PostgreSQL with EF Core migrations.
- **Swagger Documentation** — Interactive API exploration in development.

> **Note:** This project is under active development. Some features may be partially implemented or planned. See the [Roadmap](#-roadmap) for details.

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) (local installation or Docker)
- [Git](https://git-scm.com/)
- *(Optional)* [EF Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet):
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/mmd-yousefiasl/Trendicator.git
   cd Trendicator
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure the database connection**

   Open `Trendicator.Web/appsettings.json` and update the connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=Trendicator;Username=postgres;Password=yourpassword"
     }
   }
   ```

   For local development, consider using **User Secrets** instead of committing credentials:
   ```bash
   cd Trendicator.Web
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=Trendicator;Username=postgres;Password=yourpassword"
   ```

4. **Apply database migrations**
   ```bash
   dotnet ef database update \
     --project Trendicator.Data \
     --startup-project Trendicator.Web
   ```

5. **Run the application**
   ```bash
   dotnet run --project Trendicator.Web
   ```

6. **Explore the API**
   - Swagger UI (development): `https://localhost:5001/swagger`
   - Base API URL: `https://localhost:5001/api`

### Running with Docker (optional)

If you have a Docker setup for PostgreSQL, you can start the database quickly:

```bash
docker run --name trendicator-db \
  -e POSTGRES_PASSWORD=yourpassword \
  -e POSTGRES_DB=Trendicator \
  -p 5432:5432 \
  -d postgres:16
```

Then run the API with `dotnet run --project Trendicator.Web` as usual.

---

## 📁 Project Structure

```
Trendicator/
├── Trendicator.Domain/
│   ├── Entities/               # User, Product, Order, etc.
│   ├── Enums/                  # OrderStatus, ProductType, etc.
│   └── Interfaces/             # IRepository<T>, IUnitOfWork, etc.
│
├── Trendicator.Application/
│   ├── DTOs/                   # Request/response models
│   ├── Services/               # Use-case implementations
│   └── Interfaces/             # Application service contracts
│
├── Trendicator.Data/
│   ├── Context/                # AppDbContext
│   ├── Configurations/         # EF Core entity configurations
│   ├── Repositories/           # Repository implementations
│   └── Migrations/             # EF Core migrations
│
├── Trendicator.IOC/
│   └── DependencyInjection.cs  # Service registration extensions
│
├── Trendicator.Web/
│   ├── Controllers/            # API controllers
│   ├── Middleware/             # Exception handling, logging
│   ├── Extensions/             # Program.cs helper extensions
│   ├── appsettings.json
│   └── Program.cs
│
├── Trendicator.sln
└── README.md
```

---

## 🔌 API Overview

Below is a high-level overview of the API surface. Refer to **Swagger** (`/swagger`) for the full contract.

| Resource | Method | Endpoint | Description |
|----------|--------|----------|-------------|
| **Users** | `GET` | `/api/users` | List all users |
| | `GET` | `/api/users/{id}` | Get a user by ID |
| | `POST` | `/api/users` | Create a new user |
| | `PUT` | `/api/users/{id}` | Update user details |
| | `DELETE` | `/api/users/{id}` | Delete a user |
| **Products** | `GET` | `/api/products` | List all products |
| | `GET` | `/api/products/{id}` | Get a product by ID |
| | `POST` | `/api/products` | Create a product |
| | `PUT` | `/api/products/{id}` | Update a product |
| | `DELETE` | `/api/products/{id}` | Delete a product |
| **Orders** | `GET` | `/api/orders` | List orders |
| | `GET` | `/api/orders/{id}` | Get an order by ID |
| | `POST` | `/api/orders` | Create an order |
| | `PATCH` | `/api/orders/{id}/status` | Update order status |
| **TradingView** | `GET` | `/api/tradingview/products` | Fetch synced TradingView products |

> **Note:** Endpoint names and routes may differ. Update this table to reflect the actual controllers in `Trendicator.Web/Controllers`.

### Sample request — Create a product

```http
POST /api/products
Content-Type: application/json

{
  "name": "RSI Divergence Indicator",
  "description": "A TradingView indicator for RSI divergence detection.",
  "price": 49.99,
  "productType": "Indicator"
}
```

**Response**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "RSI Divergence Indicator",
  "price": 49.99,
  "createdAt": "2026-01-15T10:30:00Z"
}
```

---

## 🧠 Design Decisions

### 1. Clean Architecture over traditional N-tier

Traditional N-tier solutions often couple business logic to the database or web framework. Clean Architecture keeps **Domain** and **Application** free of infrastructure concerns, so business rules can be tested without a database, and infrastructure can be replaced without rewriting logic.

### 2. DTOs instead of exposing domain entities

The API never returns domain entities directly. Instead, **DTOs** in the Application layer define the request/response contracts. This:

- Prevents over-posting and mass-assignment vulnerabilities
- Decouples the public API from the database schema
- Allows the API contract to evolve independently of the domain model

### 3. PostgreSQL over SQL Server

The project was initially built on SQL Server and later migrated to **PostgreSQL**. Reasons:

- Open-source and free for any deployment target
- Strong EF Core provider with good migration support
- Better portability for Linux/container-based deployments
- Active community and broad hosting support

EF Core's provider abstraction made the migration a matter of updating the provider package and connection string, with minimal changes elsewhere.

### 4. Centralized IoC configuration

All dependency registrations live in `Trendicator.IOC`. This:

- Keeps `Program.cs` short and readable
- Makes service lifetimes consistent across the solution
- Allows new layers or services to be added with a single extension method

### 5. TradingView as an isolated integration

TradingView integration is treated as an **external adapter**. The core domain knows nothing about TradingView; the Application layer defines an interface, and the Data (or a dedicated integration) layer implements it. This keeps third-party coupling contained and replaceable.

### 6. Layered validation

Validation happens at two levels:

- **Domain level** — invariants that must always hold (e.g., a product must have a positive price)
- **Application/DTO level** — input shape and format validation (e.g., required fields, string lengths)

This separation ensures invalid data never reaches the domain, while domain rules remain authoritative.

---

## 🗄 Database & Migrations

### Adding a migration

```bash
dotnet ef migrations add <MigrationName> \
  --project Trendicator.Data \
  --startup-project Trendicator.Web
```

### Applying migrations

```bash
dotnet ef database update \
  --project Trendicator.Data \
  --startup-project Trendicator.Web
```

### Reverting to a previous migration

```bash
dotnet ef database update <PreviousMigrationName> \
  --project Trendicator.Data \
  --startup-project Trendicator.Web
```

### Entity configuration

Each entity has a dedicated configuration class in `Trendicator.Data/Configurations`, implementing `IEntityTypeConfiguration<T>`. This keeps `AppDbContext` clean and makes schema rules explicit and discoverable.

---

## 🧪 Testing

The layered structure is designed to be testable:

- **Domain** — pure unit tests for entity behavior and invariants
- **Application** — service tests with mocked repository interfaces
- **Data** — integration tests against a PostgreSQL test database (optional)
- **Web** — endpoint tests using `WebApplicationFactory<T>`

> Test projects are not yet included in the solution. A planned addition is `Trendicator.Tests` using **xUnit** and **Moq**.

---

## 🗺 Roadmap

- [ ] Add unit and integration test projects (`Trendicator.Tests`)
- [ ] Implement authentication and authorization (JWT)
- [ ] Add pagination, filtering, and sorting to list endpoints
- [ ] Introduce caching (Redis) for product catalog reads
- [ ] Add Docker Compose for API + PostgreSQL
- [ ] Set up CI/CD with GitHub Actions
- [ ] Add structured logging (Serilog) and health checks
- [ ] Generate API versioning strategy

---

## 🤝 Contributing

Contributions, issues, and feature requests are welcome.

1. Fork the project
2. Create your feature branch
   ```bash
   git checkout -b feature/AmazingFeature
   ```
3. Commit your changes
   ```bash
   git commit -m "Add some AmazingFeature"
   ```
4. Push to the branch
   ```bash
   git push origin feature/AmazingFeature
   ```
5. Open a Pull Request

Please make sure your code follows the existing layer boundaries and naming conventions.

---

## 📄 License

Distributed under the **MIT License**. See [`LICENSE`](LICENSE) for more information.

---

## 📬 Contact

**Mohammad Yousefi**

- Email: [mmdyousefi95@gmail.com](mailto:mmdyousefi95@gmail.com)
- GitHub: [@mmd-yousefiasl](https://github.com/mmd-yousefiasl)
- Telegram: [@mmd_u_a](https://t.me/mmd_u_a)

---

<p align="center">
  <sub>Built with ❤️ and ASP.NET Core</sub>
</p>
