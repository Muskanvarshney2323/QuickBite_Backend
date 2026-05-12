# **QuickBite Backend** 🚀

> **Quick overview:** QuickBite is a Food Delivery Application built with a Microservices Architecture. Each service handles a specific feature, making the system scalable, maintainable, and easy to understand.
>
> **Designed for:** rapid backend development, service isolation, and independent deployment.

## 📋 What is This Project?

Think of QuickBite like a real food delivery app (like Zomato or Swiggy). The backend is divided into independent services, where each service is responsible for one specific job.

## 🏗️ Architecture Overview

```text
┌─────────────────────────────────────────────────────────────┐
│                   API Gateway (Entry Point)                  │
│            Routes all requests to correct service             │
└─────────┬──────────────────────────────────────────┬──────────┘
          │                                          │
    ┌─────▼─────┐  ┌──────────┐  ┌─────────┐  ┌─────▼──────┐
    │   Auth     │  │Restaurant│  │  Order  │  │  Delivery  │
    │  Service   │  │ Service  │  │Service  │  │  Service   │
    └───────────┘  └──────────┘  └─────────┘  └────────────┘

    ┌──────────┐  ┌────────────┐  ┌─────────┐  ┌──────────┐
    │  Payment │  │  Notification│  │  Menu │ │  Cart    │
    │ Service  │  │    Service   │  │Service │ │ Service  │
    └──────────┘  └────────────┘  └─────────┘  └──────────┘

    ┌──────────────┐
    │ Review Service│
    └──────────────┘
```

## 🔧 Microservices Explained (Simple Words)

### 1. Auth Service 🔐

- What it does: Handles login and registration for all users.
- In simple terms: When you open the app and enter your email/password, this service checks if you exist in the database. If yes, it gives you a token (like a special ID card) that proves you're logged in. Without this, you can't access other features.
- Key Functions:
  - User signup (register new account)
  - User login (verify email & password)
  - Generate authentication tokens
  - Password management
- Who uses it: Customers, Restaurant Owners, Delivery Partners

### 2. Restaurant Service 🍽️

- What it does: Manages all restaurant information and their profiles.
- In simple terms: This is like a yellow pages for restaurants. It stores all restaurant details — name, location, cuisine, open/closed status, photos, and ratings. When you search for restaurants in the app, this service provides the data.
- Key Functions:
  - Add/edit restaurant profile
  - View restaurant details
  - Filter restaurants by city and cuisine
  - Approve/reject restaurants (admin only)
  - Toggle restaurant open/closed status
- Who uses it: Restaurant Owners, Customers, Admin

### 3. Menu Service 📝

- What it does: Manages all food items and dishes for each restaurant.
- In simple terms: Every restaurant has a menu with different dishes, prices, and descriptions. This service stores that information.
- Key Functions:
  - Create/edit menu items
  - Set prices and availability
  - Categorize items (veg, non-veg, etc.)
  - Show items by restaurant
- Who uses it: Restaurant Owners, Customers

### 4. Cart Service 🛒

- What it does: Manages shopping carts for customers.
- In simple terms: Just like a physical shopping cart in a store, this service keeps track of items you want to order before checkout.
- Key Functions:
  - Add items to cart
  - Remove items from cart
  - Update item quantities
  - Calculate total price
  - Clear cart after order
- Who uses it: Customers

### 5. Order Service 📦

- What it does: Manages all orders from creation to delivery.
- In simple terms: Once you finish shopping and pay, this service creates an order and tracks it. You can see order status, history, and manage cancellations.
- Key Functions:
  - Create new orders
  - Track order status
  - View order history
  - Calculate bill with taxes/discounts
  - Manage order cancellations
- Who uses it: Customers, Restaurant Owners, Delivery Partners

### 6. Delivery Service 🚴

- What it does: Manages delivery partners and delivery tracking.
- In simple terms: This service handles delivery partners. It stores their information, tracks assigned orders, and manages availability.
- Key Functions:
  - Register delivery partners
  - Track delivery partner location
  - Assign orders to delivery partners
  - Update delivery status
  - Rate delivery partners
- Who uses it: Delivery Partners, Customers

### 7. Payment Service 💳

- What it does: Handles all payment transactions.
- In simple terms: When you pay for your order with card, UPI, or wallet, this service processes the payment and records the transaction.
- Key Functions:
  - Process payments (card, UPI, wallet)
  - Refund failed payments
  - Store transaction history
  - Generate invoices
  - Handle failed payment retries
- Who uses it: Customers

### 8. Notification Service 📢

- What it does: Sends notifications to users.
- In simple terms: This service sends updates like “Your order has been confirmed,” “Delivery partner is nearby,” and “Your delivery is complete.”
- Key Functions:
  - Send order confirmation messages
  - Send delivery updates
  - Send promotional messages
  - Track notification delivery
  - Manage user notification preferences
- Who uses it: All users

### 9. Review Service ⭐

- What it does: Manages customer reviews and ratings.
- In simple terms: After receiving an order, customers can rate and review the restaurant. This helps others decide where to order.
- Key Functions:
  - Create reviews and ratings
  - View restaurant reviews
  - Delete/edit reviews
  - Calculate average rating
  - Filter reviews by rating
- Who uses it: Customers

### 10. API Gateway 🚪

- What it does: The single entry point for all requests.
- In simple terms: Think of it as a security guard at the entrance of a building. All requests come here first, it checks authorization, then routes them to the correct service.
- Key Functions:
  - Route requests to the correct service
  - Authentication/Authorization checks
  - Rate limiting
  - Load balancing
  - API documentation (Swagger)
- Who uses it: Frontend apps (web/mobile)

## 🗂️ Project Structure

```text
QuickBite/
├── services/
│   ├── api-gateway/                    # Entry point service
│   │   └── QuickBite.ApiGateway/
│   ├── auth-service/                   # Login & Registration
│   │   ├── QuickBite.Auth.API/
│   │   ├── QuickBite.Auth.Application/
│   │   ├── QuickBite.Auth.Domain/
│   │   └── QuickBite.Auth.Infrastructure/
│   ├── restaurant-service/             # Restaurant Profiles
│   ├── menu-service/                   # Food Items & Menus
│   ├── cart-service/                   # Shopping Cart
│   ├── order-service/                  # Orders & Tracking
│   ├── delivery-agent-service/         # Delivery Partners
│   ├── payment-service/                # Payment Processing
│   ├── notification-service/           # Notifications & Alerts
│   ├── review-service/                 # Ratings & Reviews
│   └── Testing/                        # Unit Tests
├── QuickBite.sln                       # Main Solution File
└── README.md                           # This file
```

## 📚 Architecture Pattern: Clean Architecture

Each microservice follows a 4-layer architecture:

1. **API Layer (Controllers)**
   - Receives HTTP requests
   - Validates input
   - Returns responses
2. **Application Layer (Business Logic)**
   - Contains main business logic
   - Processes data
   - Handles workflows
3. **Domain Layer (Core Models)**
   - Defines core entities and models
   - Contains business rules
   - Has no dependencies on other layers
4. **Infrastructure Layer (Database & External Services)**
   - Database access (SQL Server)
   - External API calls
   - Data persistence

## 🚀 Quick Start

### Prerequisites

- .NET 7 or higher
- SQL Server
- Visual Studio or VS Code

### Setup & Run

```bash
git clone <repo-url>
cd QuickBite
```

Open the solution:

- In Visual Studio: `QuickBite.sln`
- Or in terminal: `dotnet sln list`

### Set up Database

> **⚠️ Important:** Each service uses its own database connection settings. Update `appsettings.json` separately for each service before running migrations.

- Update connection string in `appsettings.json` for each service
- Run migrations for each service

### Run individual services

```bash
cd services/api-gateway/QuickBite.ApiGateway
dotnet run
```

In another terminal:

```bash
cd services/auth-service/QuickBite.Auth.API
dotnet run
```

Repeat for other services as needed.

### Access API Documentation

Open your browser at: `http://localhost:5000/swagger` (check actual port)

## 🔗 How Services Communicate

Services communicate through REST APIs:

- Frontend → API Gateway
- API Gateway → Auth Service
- API Gateway → Restaurant Service
- Cart Service → Menu Service
- Order Service → Cart Service
- Order Service → Notification Service
- Payment Service → Order Service

## 🔐 Authentication Flow

1. User enters email & password
2. Auth Service validates credentials
3. Auth Service generates JWT token
4. Token sent to frontend
5. Frontend includes token in future requests
6. API Gateway verifies token
7. Request processed by the appropriate service

## 📊 Database Schema

Each service has its own database:

- Auth Service: Stores users, passwords, roles
- Restaurant Service: Stores restaurant details, profiles
- Menu Service: Stores food items, prices
- Cart Service: Stores cart items temporarily
- Order Service: Stores order details, history
- Delivery Service: Stores delivery partner info
- Payment Service: Stores transaction records
- Review Service: Stores reviews and ratings

## 🧪 Testing

Each service has a test project:

```text
services/Testing/
├── QuickBite.Auth.Tests/
├── QuickBite.Restaurant.Tests/
├── QuickBite.Menu.Tests/
├── QuickBite.Cart.Tests/
├── QuickBite.Order.Tests/
├── QuickBite.DeliveryAgent.Tests/
├── QuickBite.Payment.Tests/
├── QuickBite.Notification.Tests/
└── QuickBite.Review.Tests/
```

Run tests:

```bash
dotnet test
```

## 🐛 Common Issues & Solutions

> **🔧 Troubleshooting tip:** Start here if a service fails to launch or if requests return unexpected status codes.

| Problem             | Solution                                                  |
| ------------------- | --------------------------------------------------------- |
| Service won't start | Check port availability and update connection strings     |
| Database not found  | Ensure SQL Server is running and check `appsettings.json` |
| 401 Unauthorized    | Make sure JWT token is valid and included in headers      |
| Service timeout     | Check if dependent services are running                   |

> **✅ Pro tip:** Use separate terminal sessions for each microservice and validate individual startup logs first.

## 📝 API Example: Ordering Food

- User logs in → Auth Service returns token
- Browse restaurants → Restaurant Service returns list
- View menu → Menu Service returns items
- Add to cart → Cart Service stores items
- Place order → Order Service creates order
- Payment → Payment Service processes payment
- Order confirmation → Notification Service sends message
- Delivery assignment → Delivery Service assigns partner
- Tracking → Notification Service updates status
- Order delivered → Review Service allows rating

## 👥 User Roles

- Customer: Browse, order, pay, review, track delivery
- Restaurant Owner: Manage restaurant, add menu items, view orders
- Delivery Partner: Accept orders, update delivery status
- Admin: Approve restaurants, manage users, view reports

## 🔄 Development Workflow

- Create feature branch
- Make changes in relevant service
- Write tests
- Test locally (run all services)
- Create Pull Request
- Code review
- Merge to main branch
- Deploy

## 📞 Support

For issues or questions:

- Check service logs
- Review API documentation (Swagger)
- Check test cases for usage examples
- Refer to individual service README files

## 📜 License

This project is part of the QuickBite Food Delivery Application.

Happy Coding! 🎉
