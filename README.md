<img align="left" width="135" height="115" src="./docs/media/architecture_layout.png" />

# CleanArchitecture Templates for .NET

![.NET Core](https://github.com/cbcrouse/CleanArchitecture/workflows/.NET%20Core/badge.svg) [![Build status](https://caseycrouse.visualstudio.com/Github/_apis/build/status/CleanArchitecture-CD)](https://caseycrouse.visualstudio.com/Github/_build/latest?definitionId=6) [![NuGet](https://img.shields.io/nuget/v/CleanArchitecture.Templates.svg)](https://www.nuget.org/packages/CleanArchitecture.Templates/) [![NuGet](https://img.shields.io/nuget/dt/CleanArchitecture.Templates.svg)](https://www.nuget.org/stats/packages/CleanArchitecture.Templates?groupby=Version) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=cbcrouse_CleanArchitecture&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=cbcrouse_CleanArchitecture)

_Keep your application code free of dependencies like persistence and presentation._

This repository provides several templates for creating .NET applications using the Clean Architecture pattern. The templates vary in complexity and include simple applications without data access layers, as well as more advanced applications with data access layers.

---

## What is Clean Architecture?

Clean Architecture is a software design pattern that separates the concerns of an application into layers, each with a specific responsibility. The pattern aims to make the application easier to maintain and test by reducing dependencies between the layers.

---

## Why use Clean Architecture?

Using Clean Architecture has several benefits, including:

- Improved maintainability: The separation of concerns makes it easier to modify individual parts of the application without affecting the rest.
- Better testability: The layers can be tested independently, making it easier to write unit tests and integration tests.
- Easier to understand: The clear separation of concerns makes it easier for developers to understand the application's structure and flow.

---

## What makes it different?

These Clean Architecture templates use [StartupOrchestration.NET](https://github.com/cbcrouse/StartupOrchestration.NET) to simplify the configuration and initialization of the application's dependencies and services.

When implementing Clean Architecture in an application, it's easy to have multiple ways to start the application. For example, the application can be started using any type of presentation layer, such as API, Console, or Azure Function. However, this can result in the same dependencies being registered in each startup sequence, which means the application may not behave consistently across different presentation layers.

To ensure the application always behaves the same, while allowing each presentation layer to register only the dependencies it needs, the StartupOrchestration&#46;NET library provides a way to manage service registrations in a structured and consistent manner. This means that each presentation layer can register only the services it requires, while the application can register its own dependencies independently of the presentation layer. As a result, the presentation layer only needs to be responsible for registering its own required services, rather than managing all dependencies for the application.

---

## ![Download](./docs/media/download_icon.png) Getting Started

### Installation

Run the command:

```powershell
dotnet new -i CleanArchitecture.Templates
```

### Templates

- **ca-sln** - A simple application with three separate presentations
- **ca-sln-sql** - Includes an entity framework core implementation using Sql Server with Docker support

### Create a template

```powershell
dotnet new ca-sln-sql --name MyNewApp --secure-port 44399 --port 34399
```

### Updating RootNamespace and AssemblyName

When you create a new project from one of the Clean Architecture templates, it will have a default RootNamespace and AssemblyName based on the template name. If you want to update these values to include your company name or a different namespace, follow these steps:

1. In Visual Studio, right-click on your project in the Solution Explorer and select "Properties".
1. In the properties window, click on the "Application" tab.
1. Update the "Assembly name" and "Default namespace" fields to the desired values.
1. Click "OK" to save the changes.

After updating the **RootNamespace** and **AssemblyName**, you will need to update any using statements in your code files that reference the old namespace. To do this, you can use Visual Studio's "Refactor" feature:

1. Right-click on the old namespace in your code file and select "Refactor" > "Rename".
1. In the rename dialog, enter the new namespace and click "Apply".
1. Visual Studio will update all the affected using statements and fully qualified type names in your code.

By updating the **RootNamespace** and **AssemblyName**, you can ensure that your project is properly named and organized for your company or personal use.

---

## ![Building Blocks](./docs/media/building_blocks.png) Clean Architecture, Domain-Driven Design, MediatR, & AutoMapper

Clean Architecture is a highly effective software development approach that provides a clear separation of concerns and emphasizes the importance of scalability, testability, and maintainability. [This article](https://pusher.com/tutorials/clean-architecture-introduction) provides an in-depth explanation of Clean Architecture. If you want to learn more, I highly recommend reading [Clean Architecture](https://www.amazon.com/Clean-Architecture-Craftsmans-Software-Structure/dp/0134494164) by Robert C. Martin.

[Domain-Driven Design](https://docs.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice) is an excellent choice for structuring solutions in conjunction with Clean Architecture, and it is well-suited for developing microservices. However, a monolithic application is still a viable solution for many problems.

[MediatR](https://github.com/jbogard/MediatR) is a third-party library that allows for clear separation of the presentation layer from the application layer. With MediatR, the controllers for an API or MVC presentation become much simpler, and it becomes easy to create custom functionality for maintenance or customer support tasks on the fly in a console application. For Azure Functions, you only need to decide which trigger you want to use.

[AutoMapper](https://github.com/AutoMapper/AutoMapper) is another useful tool that simplifies the translation of presentation models into the contract requirements for the application, domain, or persistence.

---

## ![Magnifying Glass](./docs/media/telescope.png) Solution Overview

Take a deeper [dive into the solution](./docs/solution_overview.md) to understand the dependencies, assembly responsibilities, and organization setup.

---

## ![Puzzle](./docs/media/puzzle.png) Contributing

Want to add a feature or fix a bug? Glad to have the help! There's just a couple of things to go over before you start submitting pull requests:

- [Commit Message Standards](./docs/commit_message_standards.md)

---

## ![Law](./docs/media/law.png) Licensing

These templates are licensed under the [MIT License](./LICENSE).
