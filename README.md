# Booking Modular Monolith Exploration


> 🚀 **An exploration of Modular Monolith architecture using .NET 9. This project demonstrates concepts like Vertical Slice Architecture, Event-Driven patterns with CAP, CQRS, DDD, and gRPC within a sample booking system.**



# Table of Contents

- [Booking Modular Monolith Exploration](#booking-modular-monolith-exploration)
- [Table of Contents](#table-of-contents)
  - [Project Aims](#project-aims)
  - [Technology Stack](#technology-stack)
  - [Domain Overview \& Module Boundaries](#domain-overview--module-boundaries)
  - [Architectural Approach](#architectural-approach)
  - [Getting Started](#getting-started)
    - [Required .NET Tools](#required-net-tools)
  - [Running the Application](#running-the-application)
    - [SSL Certificate Setup](#ssl-certificate-setup)
    - [Infrastructure Setup (Docker Compose)](#infrastructure-setup-docker-compose)
    - [Building the Code](#building-the-code)
    - [Running the API](#running-the-api)
    - [Running Tests](#running-tests)
  - [API Documentation](#api-documentation)

## Project Aims

This project aims to demonstrate and explore the implementation of:

- ✨ **Vertical Slice Architecture:** Structuring the application around features or use cases.
- ✨ **Domain-Driven Design (DDD):** Applying DDD principles within module boundaries.
- ✨ **Event-Driven Architecture:** Using an In-Memory Broker via `CAP` for inter-module communication.
- ✨ **Reliable Messaging:** Implementing Inbox/Outbox patterns with `CAP` for message idempotency and guaranteed delivery.
- ✨ **CQRS:** Separating command and query responsibilities using `MediatR`.
- ✨ **Data Persistence:** Utilizing `Postgres` for module-specific data storage.
- ✨ **API Documentation:** Generating `OpenAPI` specifications using built-in ASP.NET Core features.
- ✨ **Testing:** Incorporating Unit and Integration tests.
- ✨ **Input Validation:** Using `FluentValidation` within a `MediatR` pipeline behavior.
- ✨ **Deployment:** Using `Docker-Compose` for local infrastructure setup.
- ✨ **Observability:** Implementing distributed tracing with `OpenTelemetry`.
- ✨ **Authentication/Authorization:** Using `Duende IdentityServer` based on `OpenID Connect` and `OAuth2`.

## Technology Stack

- **Framework:** .NET 9, ASP.NET Core
- **API Versioning:** Asp.Versioning.Mvc
- **ORM:** Entity Framework Core
- **API Documentation:** AspNetCore OpenAPI, Scalar, Swashbuckle
- **Messaging/Event Bus:** CAP (with In-Memory Transport)
- **Mediation:** MediatR
- **Validation:** FluentValidation
- **Logging:** Serilog
- **Resilience:** Polly
- **DI Helpers:** Scrutor
- **Observability:** OpenTelemetry .NET
- **Identity:** Duende IdentityServer
- **Caching:** EasyCaching
- **Mapping:** Mapster
- **Error Handling:** Hellang.Middleware.ProblemDetails
- **ID Generation:** IdGen
- **RPC:** MagicOnion (gRPC based)

## Domain Overview & Module Boundaries

The application simulates a basic booking system divided into modules:

- **Identity Module:** Handles user authentication and authorization using Duende IdentityServer and ASP.NET Core Identity. Manages users, roles, and JWT generation.
- **Flight Module:** Manages flight information (basic CRUD operations).
- **Passenger Module:** Manages passenger details.
- **Booking Module:** Handles the core booking operations, likely coordinating between other modules.
- **API Host:** A single ASP.NET Core project that hosts all the modules. Modules are not deployed as separate microservices but run within this single process.

![Modular Monolith Diagram](./assets/modular-monolith-diagram.png)

## Architectural Approach

This project combines elements from **Clean Architecture** and **Vertical Slice Architecture**, organizing code primarily by feature using a **Feature Folder** structure.

**Authentication:** A dedicated Identity module handles authentication, issuing JWTs for signed-in users. Other modules validate these tokens to authorize requests.

**Inter-Module Communication:**
- **Synchronous:** Uses [MagicOnion](https://github.com/Cysharp/MagicOnion) (built on gRPC) for direct, request/response style communication between modules when needed. It uses C# interfaces for schema definition, simplifying code sharing.
- **Asynchronous:** Employs an **Event-Driven** approach using [CAP](https://github.com/dotnetcore/CAP) with an [In-Memory transport](https://github.com/yang-xiaodong/Savorboard.CAP.InMemoryMessageQueue). Modules publish events, and interested modules subscribe to them, promoting loose coupling and eventual consistency.

**Vertical Slices & REPR Pattern:**
Each feature or use case is treated as a distinct "vertical slice," encapsulating all necessary logic from API endpoint to data access. This minimizes coupling *between* slices and maximizes cohesion *within* a slice.

<div align="center">
  <img src="./assets/vertical-slice-architecture.png" alt="Vertical Slice Architecture Diagram" />
</div>

Instead of traditional controllers with multiple actions, the project leans towards the **REPR (Request-Endpoint-Response) pattern**. Each API action gets its own endpoint class containing:

1. The route definition.
2. An `IMediator` instance ([MediatR](https://github.com/jbogard/MediatR)).

Requests are sent via `IMediator` through a processing pipeline (for logging, validation, etc.) and handled by a specific `IRequestHandler`. This keeps endpoints focused and promotes the Single Responsibility Principle.

**CQRS (Command Query Responsibility Segregation):**
Features are further decomposed into Commands (actions that change state) and Queries (actions that retrieve data).

- **Benefits:** Can improve performance, scalability, and maintainability. Changes tend to be localized to specific command or query handlers.
- **Implementation:** Each command/query handler acts as a mini-slice, potentially even using tailored data access strategies if needed, offering flexibility beyond a strict layered approach.

## Getting Started

### Required .NET Tools

This project uses .NET local tools. Ensure you have the .NET SDK installed.

1. **Initialize the tool manifest** (if not already present):

   ```bash
   dotnet new tool-manifest
   ```

2. **Restore the tools** defined in `.config/dotnet-tools.json`:

   ```bash
   dotnet tool restore
   ```

## Running the Application

### SSL Certificate Setup

For local HTTPS development, configure a development certificate:

```bash
# Create and export the certificate (replace {password here} with a secure password)
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p {password here}

# Trust the certificate
dotnet dev-certs https --trust
```

*Note: For PowerShell, use `$env:USERPROFILE` instead of `%USERPROFILE%`.*

### Infrastructure Setup (Docker Compose)

The necessary backing services (like databases, potentially message queues if not in-memory) are defined in a Docker Compose file.

Start the infrastructure:

```bash
docker-compose -f ./deployments/docker-compose/infrastracture.yaml up -d
```

*(This starts services like Postgres defined in the compose file.)*

### Building the Code

Build the entire solution from the root directory:

```bash
dotnet build
```

### Running the API

Navigate to the API host project directory and run:

```bash
# cd src/Api/src
dotnet run
```

### Running Tests

Execute all tests from the root directory:

```bash
dotnet test
```

## API Documentation

Once the API is running, you can access the documentation endpoints:

- **Swagger UI:** `/swagger`
- **Scalar:** `/scalar/v1`

A [booking.rest](./booking.rest) file is included for use with the [REST Client](https://github.com/Huachao/vscode-restclient) VS Code extension for API testing.