# ElyraBd — E-Commerce Platform

ASP.NET Core 8 MVC e-commerce application using Clean Architecture.

## Solution Structure

```
ElyraBd/
├── ElyraBd.sln
├── ElyraBd.Core/              # Domain: entities, enums, interfaces
├── ElyraBd.Application/       # Use cases, DTOs, services (Step 2+)
├── ElyraBd.Infrastructure/    # EF Core, Identity, repositories (Step 2+)
└── ElyraBd.Web/               # MVC presentation layer (Step 2+)
```

## Step 1 — Completed

- Clean Architecture solution with 4 projects
- Domain entities aligned to [ER diagram](docs/ER-DIAGRAM.md)
- `ApplicationDbContext` with Fluent API configurations (PK column names match ER)
- Repository + Unit of Work interfaces (Core)

## Database Connection

Update `ElyraBd.Web/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ElyraBdDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

## Migrations (after Step 2 wiring)

```bash
dotnet ef migrations add InitialCreate -p ElyraBd.Infrastructure -s ElyraBd.Web
dotnet ef database update -p ElyraBd.Infrastructure -s ElyraBd.Web
```
