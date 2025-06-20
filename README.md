# 🎂 Cake Production Management System – AI-Enhanced Edition

A smart, end-to-end bakery management solution for tracking production, managing inventory, handling sales, and now enhanced with AI and automated documentation!

---

## ✨ Features

### 🏭 Production Management
- Recipe & ingredient management
- Automated yield calculations
- Production scheduling and tracking

### 📦 Inventory Control
- Real-time stock levels
- Low-stock AI-based prediction & alerts (via email)
- Supplier tracking and restocking optimization

### 📈 AI-Powered Insights
- Demand forecasting based on historical data
- Smart stock suggestions using custom-trained models (experimental)
- Error detection and intelligent anomaly alerts (logs + UI)

### 📑 API Documentation
- RESTful APIs using ASP.NET Core
- Swagger/OpenAPI auto-generated docs (https://localhost:xxxx/swagger)
- DTO-based models for clean responses

### 👨‍🍳 User Management
- Role-based access: Admin, Baker, Manager
- ASP.NET Identity authentication
- Activity logging and audit trail

---

## 🛠️ Tech Stack

### Backend
- ASP.NET Core 7.0
- Entity Framework Core
- SQL Server 2022
- Hangfire (background job processing)
- Identity + Roles
- AI service (custom ML model or external integration)

### Frontend
- Razor Pages + Partial Views
- jQuery + AJAX
- Bootstrap 5.2
- Chart.js for reporting

---

## 🤖 AI Integration Details

- AI module is integrated as a background service
- Uses data from orders, production, and stock to suggest:
  - Stock levels
  - Production optimization
  - Demand trends
- Custom ML model (Python/.NET Interop or external API-based inference)
- Configurable thresholds (smart alerts)

---

## 📦 API Endpoints

- Fully documented with Swagger
- Supports GET, POST, PUT, DELETE for:
  - Products
  - Users
  - Inventory
  - Attendance & Production sessions
- Secured endpoints with JWT / Identity-based tokens (if enabled)

---

## 🚀 Installation Guide

### ✅ Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- SQL Server 2019+ or Azure SQL
- Visual Studio 2022

Upcoming Features
📱 Mobile version using Blazor Hybrid
📦 Barcode scanner integration for inventory
🔐 OAuth login for external users
🔄 Continuous retraining of AI models with new production data

📧 Contact
Maintained by Edon Berisha
LinkedIn: https://www.linkedin.com/in/edonberisha/
Github: https://www.github.com/edonberishaa/
Email: edonberisha52@gmail.com

### 📥 Steps

1. Clone the repo:
   ```bash
   git clone https://github.com/your-username/cake-production-ai.git
   cd cake-production-ai
Configure your appsettings.json:
Connection string
SMTP for email alerts
AI endpoint config (if external)

Run migrations:
   ```bash
   dotnet ef database update




