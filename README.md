🛠️ **HR.LeaveManagement.Clean**
> _A clean architecture-based Human Resources Leave Management system, built with .NET 8, MediatR, and AutoMapper, demonstrating best practices in API development and domain-driven design._

## 📖 Overview
> _This project implements a comprehensive HR Leave Management system following a Clean Architecture approach. It showcases a well-structured, maintainable, and testable application built with .NET 8. The system handles various aspects of leave management, including leave types, leave requests, and leave allocations, providing a robust foundation for enterprise-level HR solutions._

---

## 📋 Table of Contents
1. [✨ Features](#-features)
2. [🚀 Getting Started](#-getting-started)
    - [Prerequisites](#prerequisites)
    - [Installation](#installation)
3. [📁 Project Structure](#-project-structure)
4. [⚙️ Configuration](#-configuration)
5. [📖 API Documentation](#-api-documentation)
6. [🌐 Localization Support](#-localization-support)
7. [📊 Performance Optimization](#-performance-optimization)
8. [🔍 Observability](#-observability)
9. [📦 Design Patterns](#-design-patterns)
10. [🏢 Kubernetes Tools Overview](#-kubernetes-tools-overview)
11. [🛠️ Contributing](#-contributing)
12. [📜 License](#-license)
13. [🔗 Additional Resources](#-additional-resources)

---

## ✨ Features
- **Clean Architecture** — Organized into Domain, Application, Infrastructure, and API layers for clear separation of concerns.
- **MediatR Implementation** — Utilizes MediatR for handling commands, queries, and events, promoting a decoupled and testable codebase.
- **AutoMapper Integration** — Seamless object-to-object mapping to reduce boilerplate code and improve maintainability.
- **Centralized Exception Handling** — Robust error management through custom middleware for consistent API responses.
- **CRUD Operations for Leave Management** — Comprehensive functionalities for managing Leave Types, Leave Requests, and Leave Allocations.
- **API Development with .NET 8** — Modern RESTful API built on .NET 6, leveraging its latest features and performance improvements.
- **Unit and Integration Testing** — Dedicated test projects ensuring code quality and reliability.
- **Swagger/OpenAPI Documentation** — Automatically generated API documentation for easy consumption and testing.

<div align="center">
  <a href="#-table-of-contents" style="text-decoration: none; border: 1px solid #ddd; border-radius: 5px; padding: 8px 16px; transition: background-color 0.3s;">
    🔝 Back to Top
  </a>
</div>

---

## 🚀 Getting Started
### Prerequisites
Ensure you have the following installed:
- **.NET 8 SDK**
- **SQL Server** (or any compatible database) — For persistence layer.
- **Visual Studio 2022** (or VS Code with C# extensions) — Recommended IDE.

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/ahmadabdelbary2001/HR.LeaveManagement.Clean.git
   cd HR.LeaveManagement.Clean
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Configure Database:
   Update the connection string in `HR.LeaveManagement.Api/appsettings.json` and `HR.LeaveManagement.Api/appsettings.Development.json` to point to your SQL Server instance.

4. Apply Migrations:
   Navigate to the `HR.LeaveManagement.Persistence` project and apply database migrations:
   ```bash
   dotnet ef database update --project HR.LeaveManagement.Persistence
   ```

5. Build and run the API project:
    ```bash
    dotnet run --project HR.LeaveManagement.Api
    ```
    The API will typically run on `https://localhost:5296` (or a similar port).

<div align="center">
  <a href="#-table-of-contents" style="text-decoration: none; border: 1px solid #ddd; border-radius: 5px; padding: 8px 16px; transition: background-color 0.3s;">
    🔝 Back to Top
  </a>
</div>

## 📁 Project Structure
 ```
 HR.LeaveManagement.Clean/
├── HR.LeaveManagement.Api/               # ASP.NET Core Web API project
│   ├── Controllers/                      # API Endpoints
│   ├── Middleware/                       # Custom Middleware (e.g., Exception Handling)
│   ├── Models/                           # API-specific models (e.g., Email settings)
│   ├── Program.cs                        # Application startup and service registration
│   └── appsettings.json                  # Configuration files
├── HR.LeaveManagement.Application/       # Application layer (Core business logic)
│   ├── Contracts/                        # Interfaces for services and repositories
│   ├── Exceptions/                       # Custom exceptions
│   ├── Features/                         # MediatR handlers (Commands, Queries, Events)
│   ├── MappingProfiles/                  # AutoMapper profiles
│   ├── Models/                           # Application-specific models
│   └── ApplicationServiceRegistration.cs # Dependency Injection for Application services
├── HR.LeaveManagement.Domain/            # Domain layer (Enterprise business rules and entities)
│   ├── Common/                           # Base entities and common domain logic
│   ├── Entities/                         # Core domain entities (LeaveType, LeaveRequest, LeaveAllocation)
│   └── HR.LeaveManagement.Domain.csproj  # Project file
├── HR.LeaveManagement.Infrastructure/    # Infrastructure layer (External services, e.g., Email, Logging)
│   └── EmailService/                     # Email sending implementation
├── HR.LeaveManagement.Persistence/       # Persistence layer (Data access, e.g., Entity Framework Core)
│   ├── Configurations/                   # Entity Framework Core configurations
│   ├── Repositories/                     # Concrete implementations of domain repositories
│   ├── DatabaseContext/                  # DbContext for Entity Framework Core
│   └── PersistenceServiceRegistration.cs # Dependency Injection for Persistence services
├── HR.LeaveManagement.Application.UnitTests/ # Unit tests for the Application layer
├── HR.LeaveManagement.Persistence.IntegrationTests/ # Integration tests for the Persistence layer
└── HR.LeaveManagement.Clean.sln          # Solution file
 ```

<div align="center">
  <a href="#-table-of-contents" style="text-decoration: none; border: 1px solid #ddd; border-radius: 5px; padding: 8px 16px; transition: background-color 0.3s;">
    🔝 Back to Top
  </a>
</div>

## ⚙️ Configuration
### Environment Variables
Connection strings and other sensitive configurations should be managed securely. For local development, you can use `appsettings.Development.json` or user secrets.

### Settings
- **Connection Strings** — Located in `appsettings.json` and `appsettings.Development.json` for database connectivity.
- **CORS Policy** — Configured in `Program.cs` to allow all origins, headers, and methods for development purposes. Adjust as needed for production environments.
- **Swagger/OpenAPI** — Enabled in `Program.cs` for API documentation and testing, typically only in development environments.

<div align="center">
  <a href="#-table-of-contents" style="text-decoration: none; border: 1px solid #ddd; border-radius: 5px; padding: 8px 16px; transition: background-color 0.3s;">
    🔝 Back to Top
  </a>
</div>

## 📖 API Documentation
The API documentation is generated using Swagger/OpenAPI and can be accessed when the application is running (typically at `https://localhost:5296/swagger`).

### Authentication
This project currently does not implement explicit authentication/authorization mechanisms. All API endpoints are publicly accessible. For production use, consider integrating IdentityServer4 or ASP.NET Core Identity.

#### Example API Endpoint (Conceptual - based on project structure)
- **Endpoint:** `GET /api/LeaveType`
- **Description:** Retrieves a list of all leave types.
- **Response Codes:**
  - `200 OK` — _Success_ ✅
  - `500 Internal Server Error` — _Server error_ 🔥

<div align="center">
  <a href="#-table-of-contents" style="text-decoration: none; border: 1px solid #ddd; border-radius: 5px; padding: 8px 16px; transition: background-color 0.3s;">
    🔝 Back to Top
  </a>
</div>

## 📊 Performance Optimization
- **Asynchronous Processing** — Utilizes `async/await` for non-blocking I/O operations, improving responsiveness.
- **Caching** — While not explicitly implemented in the provided code, a caching mechanism (e.g., Redis, in-memory cache) can be integrated into the Infrastructure layer to reduce database load.
- **Connection Pooling** — Managed by Entity Framework Core for efficient database connection reuse.
- **Minimal API Surface** — The API is designed to expose only necessary endpoints, reducing overhead.

<div align="center">
  <a href="#-table-of-contents" style="text-decoration: none; border: 1px solid #ddd; border-radius: 5px; padding: 8px 16px; transition: background-color 0.3s;">
    🔝 Back to Top
  </a>
</div>

## 🔍 Observability
- **Centralized Exception Handling** — Custom middleware (`ExceptionMiddleware`) in the API layer captures and handles exceptions consistently.
- **Logging** — Can be integrated using popular logging frameworks like Serilog or NLog for structured logging across different environments.
- **Health Checks** — ASP.NET Core Health Checks can be added to monitor the health of various components (database, external services).

<div align="center">
  <a href="#-table-of-contents" style="text-decoration: none; border: 1px solid #ddd; border-radius: 5px; padding: 8px 16px; transition: background-color 0.3s;">
    🔝 Back to Top
  </a>
</div>

## 📦 Design Patterns
### List of implemented patterns with brief descriptions:
- **Clean Architecture** — Separates concerns into distinct layers (Domain, Application, Infrastructure, Presentation) to improve maintainability, testability, and scalability.
- **Mediator Pattern (MediatR)** — Decouples senders from receivers, allowing for a more flexible and testable command/query/event handling system.
- **Repository Pattern** — Abstracts the data access layer, providing a clean API for interacting with data sources.
- **Unit of Work Pattern** — Ensures atomicity of transactions by coordinating multiple repository operations within a single transaction.
- **Dependency Injection** — Manages dependencies between components, promoting loose coupling and testability.
- **AutoMapper** — Simplifies object-to-object mapping, reducing manual mapping code.

<div align="center">
  <a href="#-table-of-contents" style="text-decoration: none; border: 1px solid #ddd; border-radius: 5px; padding: 8px 16px; transition: background-color 0.3s;">
    🔝 Back to Top
  </a>
</div>

## 🛠️ Contributing
1. Fork the repository 🍴
2. Create your feature branch: `git checkout -b feature/my-feature`
3. Commit changes: `git commit -m 'Add my feature'`
4. Push to branch: `git push origin feature/my-feature`
5. Open a Pull Request

Please adhere to the existing coding style and run tests before submitting.

<div align="center">
  <a href="#-table-of-contents" style="text-decoration: none; border: 1px solid #ddd; border-radius: 5px; padding: 8px 16px; transition: background-color 0.3s;">
    🔝 Back to Top
  </a>
</div>

## 📜 License
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

This project is licensed under the MIT License. See the <a href="https://github.com/ahmadabdelbary2001/HR.LeaveManagement.Clean/blob/main/LICENSE" target="_blank" rel="noopener noreferrer">LICENSE</a> file for details.

## 🔗 Additional Resources
- <a href="https://github.com/ahmadabdelbary2001/HR.LeaveManagement.Clean/wiki" target="_blank" rel="noopener noreferrer">Project Wiki</a> 📚
- <a href="https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures" target="_blank" rel="noopener noreferrer">Microsoft .NET Microservices Architecture Guide</a> 🗺️
- <a href="https://github.com/jbogard/MediatR" target="_blank" rel="noopener noreferrer">MediatR GitHub Repository</a> 💬
- <a href="https://docs.automapper.org/en/stable/" target="_blank" rel="noopener noreferrer">AutoMapper Documentation</a>

<div align="center">
  <a href="#-table-of-contents" style="text-decoration: none; border: 1px solid #ddd; border-radius: 5px; padding: 8px 16px; transition: background-color 0.3s;">
    🔝 Back to Top
  </a>
</div>

<p align="center"> Made with ❤️ by <a href="https://github.com/ahmadabdelbary2001">@Ahmad Abdelbary</a> </p>
