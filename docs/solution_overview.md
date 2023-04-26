# Solution Overview

Below is a diagram and several descriptions to help you understand the layout of the solution and why assemblies have certain dependencies and why other dependent relationships must not exist.

## 💠 Dependency Diagram

<p align="center">
    <img width="450" height="450" src="./media/dependency_diagram.png" />
</p>

* ![Green](https://via.placeholder.com/30x11/9FD383/9FD383) Presentation Layer - Depends only on Infrastructure
* ![Orange](https://via.placeholder.com/30x11/FFAC08/FFAC08) Infrastructure Layer - Depends on Application and Persistence
* ![Red](https://via.placeholder.com/30x11/D988AB/D988AB) Persistence Layer - Depends on Application
* ![Blue](https://via.placeholder.com/30x11/90B3E6/90B3E6) Business Layer - Application must ONLY depend on Domain and Events. Common is the only exception for shareable code.
* ![Yellow](https://via.placeholder.com/30x11/FFDE79/FFDE79) Common Layer - No direct dependencies

---

## 🧩 Assembly Responsibilities

* **API** - Provides presentation-specific dependency registrations through `StartupOrchestrator<TOrchestrator>`.
* **Azure Function** - Provides presentation-specific dependency registrations through `StartupOrchestrator<TOrchestrator>`.
* **Console** - Provides presentation-specific dependency registrations through `StartupOrchestrator<TOrchestrator>`.
* **Infrastructure** - Handles dependency registrations for the application and persistence as well as 3rd party integration implementations such as HTTP Clients.
* **Persistence** - Provides implementation details for persistence related interfaces in the application.
* **Application** - Provides the dependency-free core business logic.
* **Domain** - Provides the domain entities, enumerations, exceptions, constants, etc.
* **Events** - Provides event-specific models and produces a NuGet package that can be consumed by other services that want to subscribe to these events.
* **Common** - Provides shared code such as string or system extensions. This assembly can be referenced by all other assemblies.

---

## 📁 Folder Layouts

Below are examples of how files are organized within the projects. You can come up with your own ways to organize it, but this is a good place to start.

### Application

```csharp
configuration
// Contains configuration IOptions classes specific to application

entity
// User
    |---Commands
    // CreateUserCommand.cs
    |---Handlers
    // CreateUserHandler.cs
    // GetUserHandler.cs
    |---Queries
    // GetUserQuery.cs
    |---Validators
    // CreateUserValidator.cs
    // GetUserValidator.cs

interfaces
// Contains interfaces that the application depends on

mapping
// Contains 'request <-> domain' AutoMapper profiles or mapping actions
```

### Domain

```csharp
entities
// Contains the domain entity models

constants
// Contains enumerations and constants

exceptions
// Contains custom exceptions
```

### Events

```csharp
entity
// User
    |---Models
    // Address.cs
// UserCreated.cs
// UserUpdated.cs
// UserDeleted.cs
```

### Infrastructure

```csharp
configuration
// Contains configuration IOptions classes specific to infrastructure

extensions
// Contains infrastructure-specific extensions classes

services
// Contains non-persistence implementations for application interfaces

startup
// Startup orchestrator classes
```

### Persistence

```csharp
configuration
// Contains configuration IOptions classes specific to persistence

entity
// User - data access implementations

mapping
// Contains 'persistence <-> domain' AutoMapper profiles and mapping actions
```

### API

```csharp
configuration
// Contains configuration IOptions classes specific to the presentation

controllers
// Contains API controllers

mapping
// Contains 'presentation <-> request' AutoMapper profiles and mapping actions

```

> Console and Azure Function have similar folder layouts to API. However, Microsoft recommends that each function in the Function App exist in its own folder. See [Azure Function Best Practices](https://docs.microsoft.com/azure/azure-functions/functions-best-practices).

---
