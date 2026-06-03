# ElyraBd — Entity Relationship Model

This document mirrors the approved ER diagram and how it maps to the domain layer.

## AspNetUsers (`ApplicationUser`)

| ER Column     | Property / Source        |
|---------------|----------------------------|
| Id (PK)       | `Id` (Identity)            |
| FullName      | `FullName`                 |
| Email         | `Email` (Identity)         |
| PasswordHash  | `PasswordHash` (Identity)  |
| PhoneNumber   | `PhoneNumber` (Identity)   |
| CreatedAt     | `CreatedAt`                |

## Relationships from User

| Relationship              | Cardinality | Navigation                          |
|---------------------------|-------------|-------------------------------------|
| User → Cart               | 1 : 1       | `ApplicationUser.Cart`              |
| User → Order              | 1 : N       | `ApplicationUser.Orders`            |
| User → Review             | 1 : N       | `ApplicationUser.Reviews`             |
| User → ShippingAddress    | 1 : N       | `ApplicationUser.ShippingAddresses` |

## Commerce entities

| Table            | PK column (DB) | C# entity        |
|------------------|----------------|------------------|
| Categories       | CategoryId     | `Category`       |
| Products         | ProductId      | `Product`        |
| ProductImages    | ImageId        | `ProductImage`   |
| Carts            | CartId         | `Cart`           |
| CartItems        | CartItemId     | `CartItem`       |
| Orders           | OrderId        | `Order`          |
| OrderItems       | OrderItemId    | `OrderItem`      |
| ShippingAddresses| AddressId      | `ShippingAddress`|
| Payments         | PaymentId      | `Payment`        |
| Reviews          | ReviewId       | `Review`         |

## Shipping address behavior

- Every address belongs to a **User** (`UserId` required).
- When used at checkout, the same row is linked to an **Order** (`OrderId` optional → set on place order).
- Saved addresses for reuse keep `OrderId = null`.

## Order lifecycle fields

- `OrderDate` — when the order was placed
- `Status` — `OrderStatus` (Pending → Confirmed → Shipped → Delivered)
- `PaymentStatus` — summary on the order; detail in `Payment` entity

## Product module

- **Category** → **Product** (1:N)
- **Product** → **ProductImage** (1:N), `ImageUrl`, `IsPrimary`
- **Product** → **Review**, **CartItem**, **OrderItem** (1:N each)

## Payment (1:1 with Order)

- `PaymentMethod`, `TransactionId`, `Amount`, `PaymentStatus`, `PaidAt`
