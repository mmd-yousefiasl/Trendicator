<!-- ─────────────────────────────────────────────────────────────────────────────
     Trendicator · README (English) · @mmd-yousefiasl
     Persian version: README.fa.md
     ───────────────────────────────────────────────────────────────────────────── -->

<img width="100%" src="https://capsule-render.vercel.app/api?type=waving&color=0:512BD4,1:8E7CF3&height=200&section=header&text=TRENDICATOR&fontSize=48&fontColor=ffffff&desc=Trader%20Tools%20eCommerce%20%C2%B7%20ASP.NET%20Core%20MVC%20%C2%B7%20Clean%20Architecture&descSize=16&descAlignY=72&animation=fadeIn" alt="Trendicator — Trader Tools eCommerce" />

<div align="center">

<b>English</b> | <a href="README.fa.md">فارسی</a>

<a href="https://github.com/mmd-yousefiasl/Trendicator">
  <img src="https://readme-typing-svg.demolab.com?font=Fira+Code&weight=600&size=18&pause=1200&color=A855F7&center=true&vCenter=true&width=620&height=44&lines=eCommerce+for+trading+tools+and+indicators;ASP.NET+Core+MVC+%C2%B7+Clean+Architecture;EF+Core+%C2%B7+PostgreSQL+%C2%B7+Razor+%C2%B7+DI" alt="eCommerce for trading tools and indicators" />
</a>

<br>

<img src="https://img.shields.io/badge/.NET-10-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET 10" />&nbsp;
<img src="https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt="ASP.NET Core MVC" />&nbsp;
<img src="https://img.shields.io/badge/C%23-512BD4?style=flat-square" alt="C#" />&nbsp;
<img src="https://img.shields.io/badge/PostgreSQL-4169E1?style=flat-square&logo=postgresql&logoColor=white" alt="PostgreSQL" />&nbsp;
<img src="https://img.shields.io/badge/EF%20Core-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt="EF Core" />

</div>

<img width="100%" src="https://capsule-render.vercel.app/api?type=rect&color=0:512BD4,1:8E7CF3&height=2&width=1000" alt="" />

<div align="center">
  <img src="https://img.shields.io/badge/OVERVIEW-2F1C6B?style=for-the-badge" alt="Overview" />
</div>

Trendicator is a server-rendered eCommerce website where users can browse trading tools and indicators, view product details, and place orders. An admin area lets an administrator manage the product catalog and sync product data with TradingView.

I built this project end-to-end as a **sole developer** — from architecture and database design to controllers, Razor views, and deployment setup. It was created as a production-style exercise in building a maintainable ASP.NET Core MVC application with a clear separation between business logic, data access, and presentation.

<div align="center">
  <img src="https://img.shields.io/badge/MY_ROLE-2F1C6B?style=for-the-badge" alt="My Role" />
</div>

**Sole developer — full project lifecycle**

- Designed the solution architecture and layer boundaries
- Modeled the domain (users, products, orders) and the PostgreSQL schema
- Implemented persistence with Entity Framework Core and migrations
- Built the application layer: use cases, DTOs, and ViewModels
- Developed MVC controllers and Razor views (layouts, partials, Tag Helpers)
- Integrated an external third-party service (TradingView)
- Configured dependency injection centrally to keep the web project thin

<div align="center">
  <img src="https://img.shields.io/badge/ARCHITECTURE-2F1C6B?style=for-the-badge" alt="Architecture" />
</div>

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

<div align="center">
  <img src="https://img.shields.io/badge/TECH_STACK-2F1C6B?style=for-the-badge" alt="Tech Stack" />
</div>

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

<div align="center">
  <img src="https://img.shields.io/badge/FEATURES-2F1C6B?style=for-the-badge" alt="Features" />
</div>

- **Product catalog** — browse trading tools and indicators with dedicated detail pages
- **User accounts** — registration, login, and profile management
- **Order flow** — place and review orders for digital products
- **Admin area** — create, edit, and manage products through MVC pages
- **TradingView sync** — pull product information from an external service
- **ViewModel-driven views** — Razor views bind to purpose-built ViewModels, never to entities
- **Two-level validation** — data annotations on input models plus domain invariants
- **Centralized DI** — service registration isolated in the IOC project

<div align="center">
  <img src="https://img.shields.io/badge/KEY_TECHNICAL_DECISIONS-2F1C6B?style=for-the-badge" alt="Key Technical Decisions" />
</div>

**Clean Architecture inside MVC.** MVC encourages fat controllers and logic in views. I deliberately kept controllers to four steps — validate, call service, map to ViewModel, return view — so business rules remain testable without the web server.

**ViewModels instead of entity binding.** Razor views never bind to domain entities. Each view receives a ViewModel shaped for its needs. This prevents over-posting, keeps views stable when the domain evolves, and stops persistence details from leaking into HTML.

**Migration from SQL Server to PostgreSQL.** The project started on SQL Server and moved to PostgreSQL for open-source licensing, Linux/container friendliness, and a strong EF Core provider. Because persistence was isolated behind interfaces, the change was limited to the provider package, connection string, and migrations.

**TradingView as an isolated adapter.** The domain knows nothing about TradingView. The Application layer declares an interface; the integration implements it. Third-party coupling stays contained and replaceable.

**Centralized IoC registration.** All service registrations live in `Trendicator.IOC`, which keeps `Program.cs` readable and makes lifetimes consistent across the solution.

<div align="center">
  <img src="https://img.shields.io/badge/SKILLS_DEMONSTRATED-2F1C6B?style=for-the-badge" alt="Skills Demonstrated" />
</div>

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

<div align="center">
  <img src="https://img.shields.io/badge/NEXT_STEPS-2F1C6B?style=for-the-badge" alt="Next Steps" />
</div>

- Add unit and integration test projects (xUnit, Moq)
- Introduce cookie-based authentication and role-based authorization
- Add pagination, filtering, and sorting to the catalog
- Containerize the app and database with Docker Compose
- Add structured logging and health checks

<div align="center">
  <img src="https://img.shields.io/badge/CONTACT-2F1C6B?style=for-the-badge" alt="Contact" />
</div>

<p align="center">
  <b>Mohammad Yousefi</b> — Backend Developer (.NET / ASP.NET Core)
</p>

<div align="center">

<a href="mailto:mmdyousefi95@gmail.com">
  <img src="https://img.shields.io/badge/Email-mmdyousefi95%40gmail.com-512BD4?style=for-the-badge&logo=gmail&logoColor=white" alt="Email" />
</a>
<a href="https://github.com/mmd-yousefiasl">
  <img src="https://img.shields.io/badge/GitHub-%40mmd--yousefiasl-512BD4?style=for-the-badge&logo=github&logoColor=white" alt="GitHub" />
</a>
<a href="https://t.me/mmd_u_a">
  <img src="https://img.shields.io/badge/Telegram-%40mmd__u__a-512BD4?style=for-the-badge&logo=telegram&logoColor=white" alt="Telegram" />
</a>

</div>

<br>

<img width="100%" src="https://capsule-render.vercel.app/api?type=waving&color=0:512BD4,1:8E7CF3&height=140&section=footer" alt="" />
