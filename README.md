# QuickBite Backend 🚀

QuickBite is a **Food Delivery Application** built with a **Microservices Architecture**. Each service handles a specific feature, making the system scalable, maintainable, and easy to understand.

---

## 📋 What is This Project?

Think of QuickBite like a real food delivery app (like Zomato or Swiggy). The backend is divided into independent services, where each service is responsible for one specific job.

---

## 🏗️ Architecture Overview

```
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
    │  Payment │  │  Notification│  │  Menu  │  │  Cart    │
    │ Service  │  │    Service   │  │Service │  │ Service  │
    └──────────┘  └────────────┘  └─────────┘  └──────────┘

    ┌──────────────┐
    │ Review Service│
    └──────────────┘
```

---

## 🔧 Microservices Explained (Simple Words)

### 1. **Auth Service** 🔐

**What it does:** Handles login and registration for all users.

**In simple terms:** When you open the app and enter your email/password, this service checks if you exist in the database. If yes, it gives you a "token" (like a special ID card) that proves you're logged in. Without this, you can't access other features.

**Key Functions:**

- User signup (register new account)
- User login (verify email & password)
- Generate authentication tokens
- Password management

**Who uses it:** Customers, Restaurant Owners, Delivery Partners

---

### 2. **Restaurant Service** 🍽️

**What it does:** Manages all restaurant information and their profiles.

**In simple terms:** This is like a yellow pages for restaurants. It stores all restaurant details - their name, location, food types they serve, whether they're open/closed, photos, and ratings. When you search for restaurants in the app, this service provides the data.

**Key Functions:**

- Add/edit restaurant profile
- View restaurant details
- Filter restaurants by city, cuisine
- Approve/reject restaurants (admin only)
- Toggle restaurant open/closed status

**Who uses it:** Restaurant Owners (to manage their profile), Customers (to browse restaurants), Admin (to approve new restaurants)

---

### 3. **Menu Service** 📝

**What it does:** Manages all food items and dishes for each restaurant.

**In simple terms:** Every restaurant has a menu with different dishes, prices, and descriptions. This service stores all that information. When you tap on a restaurant in the app, the menu service shows you "Biryani - ₹250", "Butter Chicken - ₹300", etc.

**Key Functions:**

- Create/edit menu items
- Set prices and availability
- Categorize items (veg, non-veg, etc.)
- Show items by restaurant

**Who uses it:** Restaurant Owners (to add/update their menu), Customers (to see what they can order)

---

### 4. **Cart Service** 🛒

**What it does:** Manages shopping carts for customers.

**In simple terms:** Just like a physical shopping cart in a store, this service keeps track of items you want to order before you checkout. You add items, increase quantities, remove items - all managed here.

**Key Functions:**

- Add items to cart
- Remove items from cart
- Update item quantities
- Calculate total price
- Clear cart after order

**Who uses it:** Customers (to add items before ordering)

---

### 5. **Order Service** 📦

**What it does:** Manages all orders from creation to delivery.

**In simple terms:** Once you finish shopping and pay, this service creates an "order" and tracks it. You can see the order status (preparing, out for delivery, delivered), order history, and manage your orders.

**Key Functions:**

- Create new orders
- Track order status
- View order history
- Calculate bill with taxes/discounts
- Manage order cancellations

**Who uses it:** Customers (to place and track orders), Restaurant Owners (to see incoming orders), Delivery Partners (to pick up orders)

---

### 6. **Delivery Service** 🚴

**What it does:** Manages delivery partners and delivery tracking.

**In simple terms:** This service handles "delivery boys/girls". It stores their information, tracks which orders they're delivering, their location, and whether they're available to take new orders.

**Key Functions:**

- Register delivery partners
- Track delivery partner location
- Assign orders to delivery partners
- Update delivery status
- Rate delivery partners

**Who uses it:** Delivery Partners (to register and accept orders), Customers (to see who's delivering their order)

---

### 7. **Payment Service** 💳

**What it does:** Handles all payment transactions.

**In simple terms:** When you pay for your order using credit card, UPI, or wallet, this service processes that payment. It connects to payment gateways (like Stripe, Razorpay) and records the transaction.

**Key Functions:**

- Process payments (card, UPI, wallet)
- Refund failed payments
- Store transaction history
- Generate invoices
- Handle failed payment retries

**Who uses it:** Customers (to pay for orders)

---

### 8. **Notification Service** 📢

**What it does:** Sends notifications to users.

**In simple terms:** This service sends you updates like "Your order has been confirmed", "Delivery partner is nearby", "Your delivery is complete". It can send via SMS, Email, or In-App notifications.

**Key Functions:**

- Send order confirmation messages
- Send delivery updates
- Send promotional messages
- Track notification delivery
- User notification preferences

**Who uses it:** All users (to get updates about their orders/activities)

---

### 9. **Review Service** ⭐

**What it does:** Manages customer reviews and ratings.

**In simple terms:** After you receive your order, you can rate and review the restaurant. This service stores all reviews, ratings, and comments. It helps other customers decide which restaurants to try.

**Key Functions:**

- Create reviews and ratings
- View reviews for restaurants
- Delete/edit your reviews
- Calculate average rating
- Filter reviews by rating

**Who uses it:** Customers (to leave reviews and read reviews)

---

### 10. **API Gateway** 🚪

**What it does:** The single entry point for all requests.

**In simple terms:** Think of it as a security guard at the entrance of a building. All requests come here first, it checks if you're authorized, then routes you to the correct service. It also balances traffic load.

**Key Functions:**

- Route requests to correct service
- Authentication/Authorization check
- Rate limiting (prevent spam)
- Load balancing
- API documentation (Swagger)

**Who uses it:** Frontend apps (web/mobile) - they connect only to the gateway, not directly to services

---

## 🗂️ Project Structure

```
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

---

## 📚 Architecture Pattern: Clean Architecture

Each microservice follows a **4-Layer Architecture**:

### 1. **API Layer** (Controllers)

- Receives HTTP requests
- Validates input
- Returns responses

### 2. **Application Layer** (Business Logic)

- Contains main business logic
- Processes data
- Handles workflows

### 3. **Domain Layer** (Core Models)

- Defines core entities and models
- Business rules
- No dependencies on other layers

### 4. **Infrastructure Layer** (Database & External Services)

- Database access (SQL Server)
- External API calls
- Data persistence

---

## 🚀 Quick Start

### Prerequisites

- **.NET 7 or higher**
- **SQL Server** (for database)
- **Visual Studio** or **VS Code**

### Setup & Run

1. **Clone the repository**

   ```bash
   git clone <repo-url>
   cd QuickBite
   ```

2. **Open the solution**

   ```bash
   # Using Visual Studio
   QuickBite.sln

   # Or using terminal
   dotnet sln list
   ```

3. **Set up Database**
   - Update connection string in `appsettings.json` for each service
   - Run migrations for each service

4. **Run individual services**

   ```bash
   cd services/api-gateway/QuickBite.ApiGateway
   dotnet run

   # In another terminal
   cd services/auth-service/QuickBite.Auth.API
   dotnet run

   # Similarly for other services...
   ```

5. **Access API Documentation**
   - Open browser: `http://localhost:5000/swagger` (check actual port)
   - See all available endpoints and test them

---

## 🔗 How Services Communicate

Services communicate through **REST APIs**:

1. **Frontend** → **API Gateway** (single entry point)
2. **API Gateway** → **Auth Service** (verify user)
3. **API Gateway** → **Restaurant Service** (get restaurants)
4. **Cart Service** → **Menu Service** (validate items exist)
5. **Order Service** → **Cart Service** (fetch cart items)
6. **Order Service** → **Notification Service** (send confirmation)
7. **Payment Service** → **Order Service** (update payment status)

---

## 🔐 Authentication Flow

```
1. User enters email & password
        ↓
2. Auth Service validates credentials
        ↓
3. Auth Service generates JWT Token
        ↓
4. Token sent to frontend
        ↓
5. Frontend includes token in all future requests
        ↓
6. API Gateway verifies token
        ↓
7. Request processed by appropriate service
```

---

## 📊 Database Schema

Each service has its own database (following microservices principle):

- **Auth Service:** Stores users, passwords, roles
- **Restaurant Service:** Stores restaurant details, profiles
- **Menu Service:** Stores food items, prices
- **Cart Service:** Stores cart items temporarily
- **Order Service:** Stores order details, history
- **Delivery Service:** Stores delivery partner info
- **Payment Service:** Stores transaction records
- **Review Service:** Stores reviews and ratings

---

## 🧪 Testing

Each service has a test project:

```
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

---

## 🐛 Common Issues & Solutions

| Problem             | Solution                                             |
| ------------------- | ---------------------------------------------------- |
| Service won't start | Check port is available, update connection strings   |
| Database not found  | Ensure SQL Server is running, check appsettings.json |
| 401 Unauthorized    | Make sure JWT token is valid and included in headers |
| Service timeout     | Check if dependent services are running              |

---

## 📝 API Example: Ordering Food

1. **User logs in** → Auth Service returns token
2. **Browse restaurants** → Restaurant Service returns list
3. **View menu** → Menu Service returns items
4. **Add to cart** → Cart Service stores items
5. **Place order** → Order Service creates order
6. **Payment** → Payment Service processes payment
7. **Order confirmation** → Notification Service sends message
8. **Delivery assignment** → Delivery Service assigns partner
9. **Tracking** → Notification Service updates status
10. **Order delivered** → Review Service allows rating

---

## 👥 User Roles

- **Customer:** Can browse, order, pay, review, track delivery
- **Restaurant Owner:** Can manage restaurant, add menu items, view orders
- **Delivery Partner:** Can accept orders, update delivery status
- **Admin:** Can approve restaurants, manage users, view reports

---

## 🔄 Development Workflow

1. Create feature branch
2. Make changes in relevant service
3. Write tests
4. Test locally (run all services)
5. Create Pull Request
6. Code review
7. Merge to main branch
8. Deploy

---

## 📞 Support

For issues or questions:

- Check service logs
- Review API documentation (Swagger)
- Check test cases for usage examples
- Refer to individual service README files

---

## 📜 License

This project is part of QuickBite Food Delivery Application.

---

**Happy Coding! 🎉**
