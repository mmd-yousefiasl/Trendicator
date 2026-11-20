# Trendicator

> An ASP.NET Core MVC web application for a trading-tools eCommerce platform — built with Clean Architecture, Razor views, and PostgreSQL.

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=flat&logo=dotnet&logoColor=white)](https://learn.microsoft.com/en-us/aspnet/core/mvc/overview)
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
- [Pages & Routes](#-pages--routes)
- [Design Decisions](#-design-decisions)
- [Database & Migrations](#-database--migrations)
- [Testing](#-testing)
- [Roadmap](#-roadmap)
- [Contributing](#-contributing)
- [License](#-license)
- [Contact](#-contact)

---

## 📖 About The Project

**Trendicator** is a server-rendered **ASP.NET Core MVC** web application for an eCommerce platform that sells trading tools, indicators, and digital products. Visitors can browse the catalog, view product details, and place orders, while an admin area handles product management and syncing with **TradingView**.

The project is built with **Clean Architecture** so that business rules stay independent of the web framework and database. Razor views render the UI on the server, controllers coordinate requests, and the Application layer holds the use cases and DTOs/ViewModels that drive each page.

### Why this project exists

- To serve as a production-style reference for **Clean Architecture in an ASP.NET Core MVC application**
- To practice **controller → application service → view** flow with clean separation of concerns
- To apply **DTO / ViewModel patterns** so Razor views never bind directly to domain entities
- To integrate with a real third-party service (TradingView) without coupling the core domain to it
- To work with **PostgreSQL** and **EF Core** in a realistic eCommerce domain

---

## 🏗 Architecture

The solution follows **Clean Architecture** and is split into five projects, each with a single responsibility:

```
Trendicator.sln
├── Trendicator.Domain          # Entities, enums, domain interfaces (no dependencies)
├── Trendicator.Application     # Use cases, DTOs/ViewModels, service interfaces
├── Trendicator.Data            # EF Core DbContext, repositories, migrations
├── Trendicator.IOC             # Dependency injection registration
└── Trendicator.Web             # ASP.NET Core MVC (Controllers, Views, wwwroot)
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
| **Application** | Use cases, DTOs/ViewModels, mapping logic, and application-level service interfaces. Orchestrates domain objects for the UI. |
| **Data** | EF Core `DbContext`, entity configurations, repository implementations, and migrations. |
| **IOC** | Centralized service registration (`AddApplication`, `AddData`, etc.) to keep `Program.cs` clean. |
| **Web** | MVC controllers, Razor views, layouts, partials, Tag Helpers, middleware, and static assets in `wwwroot`. Handles HTTP and rendering concerns only. |

### Request flow

```
Browser → Controller → Application Service → Domain / Repository → EF Core → PostgreSQL
                ↓
           ViewModel / DTO
                ↓
            Razor View → HTML response
```

Controllers are thin: they validate input, call an application service, map the result to a ViewModel, and return a view. Business logic lives in the Application and Domain layers — not in controllers or views.

---

## 🛠 Tech Stack

| Category | Technologies |
|----------|--------------|
| **Runtime** | .NET 10 |
| **Framework** | ASP.NET Core MVC |
| **View Engine** | Razor (`.cshtml`), Layouts, Partials, Tag Helpers |
| **Language** | C# |
| **Architecture** | Clean Architecture, Layered Design, DTO / ViewModel pattern, Dependency Injection |
| **ORM** | Entity Framework Core |
| **Database** | PostgreSQL (primary), previously SQL Server |
| **External Integration** | TradingView (product data) |
| **Frontend** | HTML5, CSS, JavaScript (in `wwwroot`) |
| **Version Control** | Git |

---

## ✨ Features

- **Product Catalog** — Browse trading tools, indicators, and digital products.
- **Product Details** — Dedicated pages for each product with descriptions and pricing.
- **User Accounts** — Registration, login, and profile management.
- **Shopping / Order Flow** — Add products and place orders for digital goods.
- **Admin Area** — Create, edit, and manage products through MVC pages.
- **TradingView Integration** — Sync product information with TradingView's product flow.
- **ViewModel / DTO Separation** — Razor views bind to ViewModels, never to domain entities.
- **Server-Side Validation** — Data annotations plus domain invariants for robust form handling.
- **Dependency Injection** — Configured centrally via the `IOC` project.
- **PostgreSQL Persistence** — Fully migrated from SQL Server to PostgreSQL with EF Core migrations.

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

   For local development, prefer **User Secrets** over committing credentials:
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

6. **Open the site**
   - Home page: `https://localhost:5001/`
   - Product catalog: `https://localhost:5001/products`
   - Admin area: `https://localhost:5001/admin` *(if enabled)*

   Adjust the port to match your `launchSettings.json`.

### Running with Docker (optional)

If you have a Docker setup for PostgreSQL, you can start the database quickly:

```bash
docker run --name trendicator-db \
  -e POSTGRES_PASSWORD=yourpassword \
  -e POSTGRES_DB=Trendicator \
  -p 5432:5432 \
  -d postgres:16
```

Then run the web app with `dotnet run --project Trendicator.Web` as usual.

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
│   ├── DTOs/                   # Data transfer objects
│   ├── ViewModels/             # View-specific models for Razor pages
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
│   ├── Controllers/            # MVC controllers
│   ├── Views/                  # Razor views (.cshtml)
│   │   ├── Shared/             # _Layout.cshtml, partials
│   │   ├── Home/
│   │   ├── Products/
│   │   ├── Account/
│   │   └── Admin/
│   ├── wwwroot/                # CSS, JS, images, libraries
│   ├── Middleware/             # Exception handling, logging
│   ├── Extensions/             # Program.cs helper extensions
│   ├── appsettings.json
│   └── Program.cs
│
├── Trendicator.sln
└── README.md
```

---

## 🧭 Pages & Routes

Trendicator uses conventional MVC routing (`/{controller}/{action}/{id?}`). Below is a high-level overview of the main pages. Update this table to match the actual controllers in `Trendicator.Web/Controllers`.

| Page | Route | Controller / Action | Description |
|------|-------|---------------------|-------------|
| **Home** | `/` | `Home / Index` | Landing page with featured products |
| **Product Catalog** | `/products` | `Products / Index` | Lists all available trading tools |
| **Product Details** | `/products/details/{id}` | `Products / Details` | Single product view |
| **Register** | `/account/register` | `Account / Register` | User registration form |
| **Login** | `/account/login` | `Account / Login` | User login form |
| **Profile** | `/account/profile` | `Account / Profile` | Authenticated user profile |
| **Orders** | `/orders` | `Orders / Index` | User's order history |
| **Order Details** | `/orders/details/{id}` | `Orders / Details` | Single order view |
| **Admin – Products** | `/admin/products` | `Admin / Products` | Manage products (CRUD) |
| **Admin – Product Edit** | `/admin/products/edit/{id}` | `Admin / EditProduct` | Edit a product |
| **Error** | `/home/error` | `Home / Error` | Generic error page |

> **Note:** Route names and actions may differ. Keep this table in sync with your controllers.

---

## 🧠 Design Decisions

### 1. Clean Architecture inside an MVC application

MVC makes it easy to put business logic in controllers or even views. This project deliberately avoids that. Controllers only:

1. Validate the incoming model
2. Call an Application service
3. Map the result to a ViewModel
4. Return a view

All business rules live in **Domain** and **Application**, so they can be tested without spinning up the web server.

### 2. ViewModels and DTOs instead of domain entities

Razor views never bind to domain entities. Instead:

- **DTOs** carry data between layers.
- **ViewModels** are shaped specifically for each view (e.g., `ProductDetailsViewModel` includes a product plus related items).

This prevents over-posting, keeps views stable when the domain changes, and avoids leaking persistence details into HTML.

### 3. PostgreSQL over SQL Server

The project was initially built on SQL Server and later migrated to **PostgreSQL**. Reasons:

- Open-source and free for any deployment target
- Strong EF Core provider with good migration support
- Better portability for Linux/container-based deployments
- Active community and broad hosting support

EF Core's provider abstraction made the migration a matter of updating the provider package and connection string.

### 4. Centralized IoC configuration

All dependency registrations live in `Trendicator.IOC`. This keeps `Program.cs` short, makes service lifetimes consistent, and allows new services to be added with a single extension method.

### 5. TradingView as an isolated integration

TradingView integration is treated as an **external adapter**. The core domain knows nothing about TradingView; the Application layer defines an interface, and the integration layer implements it. This keeps third-party coupling contained and replaceable.

### 6. Server-side validation at two levels

Validation happens at two levels:

- **ViewModel level** — data annotations for form input (required fields, string lengths, ranges)
- **Domain level** — invariants that must always hold (e.g., a product must have a positive price)

Invalid input never reaches the domain, and domain rules stay authoritative regardless of which UI calls them.

### 7. Razor views with layouts and partials

Shared UI lives in `Views/Shared/_Layout.cshtml`, with reusable pieces as partial views (e.g., product cards, navigation). This keeps views small, consistent, and easy to maintain.

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
- **Web** — controller tests plus integration tests using `WebApplicationFactory<T>`

> Test projects are not yet included in the solution. A planned addition is `Trendicator.Tests` using **xUnit** and **Moq**.

---

## 🗺 Roadmap

- [ ] Add unit and integration test projects (`Trendicator.Tests`)
- [ ] Implement authentication and authorization (cookie-based auth)
- [ ] Add pagination, filtering, and sorting to the product catalog
- [ ] Introduce caching for product catalog pages
- [ ] Add Docker Compose for the web app + PostgreSQL
- [ ] Set up CI/CD with GitHub Actions
- [ ] Add structured logging (Serilog) and health checks
- [ ] Improve admin area with bulk product management

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

Please make sure your code follows the existing layer boundaries and naming conventions. Business logic belongs in the Application or Domain layers — not in controllers or views.

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
  <sub>Built with ❤️ and ASP.NET Core MVC</sub>
</p>
