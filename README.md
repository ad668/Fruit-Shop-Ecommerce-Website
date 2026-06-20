# Online Fruit Shop E-Commerce System

A complete Angular + ASP.NET Core Web API e-commerce platform for selling fruits online with cart, checkout, invoices, admin dashboard, JWT authentication, and SQL Server support.

## Project Structure

- `backend/OnlineFruitShop.API` - ASP.NET Core Web API project
- `backend/OnlineFruitShop.Core` - domain entities and interfaces
- `backend/OnlineFruitShop.Infrastructure` - EF Core, repositories, services
- `frontend` - Angular SPA using modern responsive UI

## Key Features

- User registration, login, JWT authentication
- Product listing, details, cart, checkout
- Order management, invoice generation, payment gateway hooks
- Admin product and category management
- Responsive UI for mobile, tablet, desktop
- SQL Server data persistence

## Getting Started

### Backend
1. Open `backend/OnlineFruitShop.API` in Visual Studio or VS Code.
2. Run `dotnet restore`.
3. Configure SQL Server connection in `appsettings.json`.
4. Run `dotnet ef database update` to create the database.
5. Start the API with `dotnet run`.

### Frontend
1. Open `frontend` in VS Code.
2. Run `npm install`.
3. Run `npm start`.

## Notes

- The backend uses clean architecture and repository pattern.
- The frontend is built with Angular, Angular Material, and responsive layouts.
- Payment gateway integration is prepared for Stripe/Razorpay/PayPal.
