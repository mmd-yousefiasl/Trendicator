# Trendicator

> An eCommerce web application for trading tools and indicators — built with ASP.NET Core MVC, Clean Architecture, and PostgreSQL.

**English** | [فارسی](README.fa.md)

![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=flat&logo=dotnet&logoColor=white)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=flat&logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=flat&logo=postgresql&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-512BD4?style=flat&logo=dotnet&logoColor=white)

---

## Overview

Trendicator is a server-rendered eCommerce website where users can browse trading tools and indicators, view product details, and place orders. An admin area lets an administrator manage the product catalog and sync product data with TradingView.

I built this project end-to-end as a **sole developer** — from architecture and database design to controllers, Razor views, and deployment setup. It was created as a production-style exercise in building a maintainable ASP.NET Core MVC application with a clear separation between business logic, data access, and presentation.

---

## My Role

**Sole developer — full project lifecycle**

- Designed the solution architecture and layer boundaries
- Modeled the domain (users, products, orders) and the PostgreSQL schema
- Implemented persistence with Entity Framework Core and migrations
- Built the application layer: use cases, DTOs, and ViewModels
- Developed MVC controllers and Razor views (layouts, partials, Tag Helpers)
- Integrated an external third-party service (TradingView)
- Configured dependency injection centrally to keep the web project thin

---

## Architecture

The solution follows **Clean Architecture** across five projects, each with a single responsibility:

```
Trendicator.sln
├── Trendicator.Domain          # Entities, enums, domain interfaces (no dependencies)
├── Trendicator.Application     # Use cases, DTOs / ViewModels, service interfaces
├── Trendicator.Data            # EF Core DbContext, repositories, migrations
├── Trendicator.IOC             # Dependency injection registration
└── Trendicator.Web             # ASP.NET Core MVC (Controllers, Views, wwwroot)
```

**Dependency direction**

```
Web  ──►  Application  ──►  Domain
 │            ▲
 ▼            │
IOC ──►  Data ┘
```

**Request flow**

```
Browser → Controller → Application Service → Domain / Repository → EF Core → PostgreSQL
                ↓
           ViewModel / DTO
                ↓
            Razor View → HTML response
```

Controllers stay thin by design: they validate input, call an application service, map the result to a ViewModel, and return a view. Business rules live in the Domain and Application layers — never in controllers or `.cshtml` files.

| Layer | Responsibility |
|-------|----------------|
| **Domain** | Core entities, enums, and contracts. Pure C#, no EF Core or ASP.NET dependencies. |
| **Application** | Use cases, DTOs, ViewModels, and service interfaces. |
| **Data** | `DbContext`, entity configurations, repository implementations, migrations. |
| **IOC** | Centralized service registration; keeps `Program.cs` minimal. |
| **Web** | Controllers, Razor views, layouts, partials, middleware, and static assets. |

---

## Tech Stack

| Category | Technologies |
|----------|--------------|
| **Runtime** | .NET 10 |
| **Framework** | ASP.NET Core MVC |
| **View Engine** | Razor (`.cshtml`), Layouts, Partials, Tag Helpers |
| **Language** | C# |
| **Architecture** | Clean Architecture, DTO / ViewModel pattern, Dependency Injection |
| **ORM** | Entity Framework Core |
| **Database** | PostgreSQL (migrated from SQL Server) |
| **External Integration** | TradingView |
| **Frontend** | HTML, CSS, JavaScript in `wwwroot` |

---

## Features

- **Product catalog** — browse trading tools and indicators with dedicated detail pages
- **User accounts** — registration, login, and profile management
- **Order flow** — place and review orders for digital products
- **Admin area** — create, edit, and manage products through MVC pages
- **TradingView sync** — pull product information from an external service
- **ViewModel-driven views** — Razor views bind to purpose-built ViewModels, never to entities
- **Two-level validation** — data annotations on input models plus domain invariants
- **Centralized DI** — service registration isolated in the IOC project

---

## Key Technical Decisions

**Clean Architecture inside MVC.** MVC encourages fat controllers and logic in views. I deliberately kept controllers to four steps — validate, call service, map to ViewModel, return view — so business rules remain testable without the web server.

**ViewModels instead of entity binding.** Razor views never bind to domain entities. Each view receives a ViewModel shaped for its needs. This prevents over-posting, keeps views stable when the domain evolves, and stops persistence details from leaking into HTML.

**Migration from SQL Server to PostgreSQL.** The project started on SQL Server and moved to PostgreSQL for open-source licensing, Linux/container friendliness, and a strong EF Core provider. Because persistence was isolated behind interfaces, the change was limited to the provider package, connection string, and migrations.

**TradingView as an isolated adapter.** The domain knows nothing about TradingView. The Application layer declares an interface; the integration implements it. Third-party coupling stays contained and replaceable.

**Centralized IoC registration.** All service registrations live in `Trendicator.IOC`, which keeps `Program.cs` readable and makes lifetimes consistent across the solution.

---

## Skills Demonstrated

- ASP.NET Core MVC (controllers, routing, filters, middleware)
- Razor views: layouts, partials, Tag Helpers, strongly typed models
- C# and .NET 10
- Clean Architecture and layered solution design
- DTO and ViewModel patterns
- Dependency Injection and service lifetime management
- Entity Framework Core: `DbContext`, configurations, migrations
- PostgreSQL and SQL Server
- Domain modeling and validation at multiple levels
- Third-party service integration
- Git for version control

---

## Next Steps

- Add unit and integration test projects (xUnit, Moq)
- Introduce cookie-based authentication and role-based authorization
- Add pagination, filtering, and sorting to the catalog
- Containerize the app and database with Docker Compose
- Add structured logging and health checks

---

## Contact

**Mohammad Yousefi** — Backend Developer (.NET / ASP.NET Core)

- Email: [mmdyousefi95@gmail.com](mailto:mmdyousefi95@gmail.com)
- GitHub: [@mmd-yousefiasl](https://github.com/mmd-yousefiasl)
- Telegram: [@mmd_u_a](https://t.me/mmd_u_a)
