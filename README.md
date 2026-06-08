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
# 🛒 ElyraBd – Full-Stack ASP.NET Core E-Commerce Platform

ElyraBd is a **scalable, modular, and production-style e-commerce web application** built with **ASP.NET Core 8 MVC** using **Clean Architecture principles**.  
It provides a complete online shopping system with **admin management, order processing, cart system, notifications, reviews, and analytics dashboard**.

---

# ✨ Key Highlights

- 🧱 Clean Architecture (Core / Application / Infrastructure / Web)
- 🛍️ Complete e-commerce workflow (Product → Cart → Checkout → Order)
- 🔐 Secure authentication using ASP.NET Core Identity
- 📦 Full order lifecycle management
- 📊 Admin dashboard with business insights
- 🔔 Notification system for users and admins
- ⭐ Product reviews & ratings system
- 📈 User activity tracking system
- 💳 Extensible payment architecture
- 🖼️ Product image upload support

---

# 🚀 Features

## 🛍️ Customer Side

- Browse products with categories and filtering
- Product detail pages with images, offers, and reviews
- Add/remove items from cart
- Update cart quantities dynamically
- Checkout with shipping address
- Order placement and confirmation
- Order history and invoice view
- Product rating and reviews system
- Notification updates (order status, system messages)

---

## 🛠️ Admin Panel

- 📊 Dashboard with key statistics (orders, users, revenue)
- 🛍️ Product management (CRUD + images + offers)
- 📂 Category management
- 🧾 Order management with status updates
- 👥 Customer management
- 🎟️ Coupon and discount system
- 📢 Notification broadcasting
- 📉 User activity tracking and analytics

---

## 🔐 Authentication & Authorization

- ASP.NET Core Identity integration
- Role-based access control:
  - Admin
  - Customer
- Secure login / registration system
- Password hashing & security policies
- Protected admin area using authorization filters

---

## 📦 Order Management System

- Multi-item order creation
- Shipping address capture
- Order status lifecycle:
  - Pending
  - Processing
  - Shipped
  - Delivered
  - Cancelled
- Order history tracking per user
- Admin-controlled order updates
- Invoice generation support

---

## 🔔 Notification System

- User notifications for:
  - Order placed
  - Order status changes
  - Admin messages
- Admin notifications for:
  - New orders
  - User activities
- Persistent notification storage

---

## ⭐ Reviews & Ratings

- Customers can:
  - Leave product reviews
  - Rate products (1–5 stars)
  - Edit or update feedback
- Admin moderation support (extendable)

---

## 📊 Admin Dashboard

- Total orders overview
- Revenue tracking (extendable)
- Product statistics
- Active users tracking
- Recent orders table
- System activity logs

---

# 🏗️ Architecture

This project follows **Clean Architecture design pattern**:

```
ElyraBd.Core
    └── Domain Layer
        - Entities (Product, Order, Cart, etc.)
        - Enums (OrderStatus, PaymentStatus)
        - Interfaces (Repositories contracts)

ElyraBd.Application
    └── Business Logic Layer
        - DTOs (Data Transfer Objects)
        - Services (CartService, OrderService, etc.)
        - Interfaces (Service contracts)
        - AutoMapper Profiles

ElyraBd.Infrastructure
    └── Data Access Layer
        - EF Core DbContext
        - Repositories (Generic + Specific)
        - Migrations
        - Identity implementation
        - External services (File storage)

ElyraBd.Web
    └── Presentation Layer
        - MVC Controllers
        - Razor Views
        - ViewModels
        - Admin Area
        - Frontend (Bootstrap, jQuery)
```

---

# 🧱 Tech Stack

### Backend
- ASP.NET Core 8 MVC
- Entity Framework Core
- LINQ
- ASP.NET Core Identity

### Frontend
- Razor Views
- Bootstrap 5
- JavaScript / jQuery
- HTML5 / CSS3

### Database
- SQL Server

### Libraries
- AutoMapper
- FluentValidation
- Identity Framework

---

# 📂 Project Modules

- 👤 Authentication & Authorization
- 🛍️ Product Catalog
- 📦 Cart System
- 🧾 Order Management
- 🎟️ Coupon System
- 📢 Notifications
- ⭐ Reviews & Ratings
- 📊 Admin Dashboard
- 📈 User Activity Tracking

---

# ⚙️ Setup Instructions

## 1. Clone Repository
```bash
git clone https://github.com/your-username/ElyraBd.git
cd ElyraBd
```

---

## 2. Configure Database
Update connection string in:

```
ElyraBd.Web/appsettings.json
```

---

## 3. Apply Migrations

```bash
dotnet ef database update
```

---

## 4. Run Application

```bash
dotnet run --project ElyraBd.Web
```

---

# 👥 User Roles

| Role     | Permissions |
|----------|-------------|
| Admin    | Full system access (products, orders, users, dashboard) |
| Customer | Browse products, place orders, reviews |

---

# 📈 System Design Strengths

- Modular layered architecture
- Repository + Unit of Work pattern
- DTO-based communication (clean separation)
- Scalable service-based business logic
- Extensible payment & notification design
- Separation of Admin and Customer UI

---

# 🚀 Future Improvements

- 🔴 Real-time notifications (SignalR)
- 📱 REST API for mobile applications
- 💳 Payment gateway integration (Stripe / SSLCommerz)
- 🔍 Advanced search & filtering (ElasticSearch)
- 📊 Advanced analytics dashboard
- ☁️ Cloud file storage (Azure / AWS S3)
- 🧠 Recommendation system (AI-based suggestions)

---

# 📸 Screenshots (Add later)

> Add images of:
- Home page
- Product details
- Cart page
- Checkout
- Admin dashboard

---

# 🧑‍💻 Author

**ElyraBd Development Team**

---

# 📜 License

This project is intended for educational and commercial use (update license as required).

---

# ⭐ Final Note

This project is built as a **real-world scalable e-commerce system** demonstrating enterprise-level ASP.NET Core architecture and best practices.
## Migrations (after Step 2 wiring)

```bash
dotnet ef migrations add InitialCreate -p ElyraBd.Infrastructure -s ElyraBd.Web
dotnet ef database update -p ElyraBd.Infrastructure -s ElyraBd.Web
```
