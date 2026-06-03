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

## Progress

| Step | Status |
|------|--------|
| 1 — Entities + DbContext | Done |
| 2 — Identity + roles | Done |
| 3 — Products + Categories | Done |
| 4 — Cart | Next |
| 5 — Orders | Planned |
| 6 — Admin analytics | Dashboard + Chart.js done |
| 7 — Customer UI | Shop storefront done |
| 8 — Activity tracking | Login + product views done |

### Run the app

```bash
cd ElyraBd.Web
dotnet run
```

- **Store:** `/` or `/Shop`
- **Admin:** `/Admin/Dashboard` (login as `admin@elyrabd.com` / `Admin@12345`)
- **Auth:** `/Account/Login` (animated template preserved)

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
