<img align="left" width="135" height="115" src="./docs/media/architecture_layout.png" />

# Clean Architecture Templates

![.NET Core](https://github.com/cbcrouse/CleanArchitecture/workflows/.NET%20Core/badge.svg) [![Build status](https://caseycrouse.visualstudio.com/Github/_apis/build/status/CleanArchitecture-CI)](https://caseycrouse.visualstudio.com/Github/_build/latest?definitionId=4) [![NuGet](https://img.shields.io/nuget/v/CleanArchitecture.Templates.svg)](https://www.nuget.org/packages/CleanArchitecture.Templates/) [![NuGet](https://img.shields.io/nuget/dt/CleanArchitecture.Templates.svg)](https://www.nuget.org/stats/packages/CleanArchitecture.Templates?groupby=Version)

_Keep your application code free of dependencies like persistence and presentation._

Welcome to my rendition of Jason Taylor's [Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)! I was first inspired by Jason's [first video](https://www.youtube.com/watch?v=_lwCVE_XgqI&t=4s) where Clean Architecture was presented in a .NET Core application which has been updated in his [most recent video](https://www.youtube.com/watch?v=5OtUm1BLmG0). I used this solution structure, patterns, and principles to build an entire suite of production microservices. What came of that, was the ability to **orchestrate an application's startup process** using any presentation, ensuring that an application's dependencies and behavior remains consistent.

---

## Startup Orchestration

#### What is the problem?

Applications that depend on the .NET Core framework have similar startup flows:

1. .NET Core hands your application some stuff like an IServiceCollection with some pre-populated dependencies such as ILoggerFactory.
1. You register some startup dependencies as well.
1. Your application builds that container and is ready for use.

When implementing _Clean Architecture_ in your application, it becomes very easy to have multiple ways to start your application. When your application is capable of starting up with any type of presentation layer (e.g. API, Console, or an Azure Function), this requires you to register the same dependencies in each startup sequence. **This means you cannot guarantee that your application will behave the same for each presentation, because your presentation controls your dependencies**.

What if your application could register its own dependencies, ensuring it always behaved the same, while the presentation layers only registered dependencies that they needed? For example, an API is no longer responsible for ensuring the 'EmailService' is registered, but is responsible for Swagger being registered. This is where the startup orchestration comes in!

#### How does it work?

The concept itself is simple enough, **the presentation layer needs to hand the responsibility of adding services to the IoC container to the application's infrastructure**.

This is exactly what has been done. Within the Infrastructure assembly, you will find a startup folder with several classes:

* **CoreStartupOrchestrator** - This contains the core functionality and is ultimately responsible for executing IoC container registrations.
* **AppStartupOrchestrator** - Inherits from core, but simply adds the dependency registrations that are needed by the application.
* **PresentationStartupOrchestrator** - This is where the pieces are connected together. See how it's been implemented in the [API Startup.cs](./templates/ca-sln-sql/src/Presentation.API/Startup.cs).

> The Azure Function and Console presentation layers have different ways of starting up, but the orchestration is accomplished in similar ways.

#### The Secret Sauce

You might have noticed that all of the service registrations are added to collections of [Expressions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/expressions). This allows the infrastructure to accept any number of IoC registrations without issue, orchestrate the order of execution, as well as dynamically log what exactly is being executed. Here's a sample of the log output from the API starting up:

```log
[2020:06:03 11:05:03.723 PM] [Verbose] [] '"value(Infrastructure.Startup.AppStartupOrchestrator).RegisterAutoMapper()"' was started...
[2020:06:03 11:05:03.724 PM] [Verbose] [] '"value(Infrastructure.Startup.AppStartupOrchestrator).RegisterAutoMapper()"' completed successfully!
[2020:06:03 11:05:03.724 PM] [Verbose] [] '"value(Infrastructure.Startup.AppStartupOrchestrator).ServiceCollection.AddSingleton()"' was started...
[2020:06:03 11:05:03.724 PM] [Verbose] [] '"value(Infrastructure.Startup.AppStartupOrchestrator).ServiceCollection.AddSingleton()"' completed successfully!
[2020:06:03 11:05:04.776 PM] [Verbose] [] '"value(Presentation.API.Startup).ServiceCollection.AddAuthorization()"' was started...
[2020:06:03 11:05:04.883 PM] [Verbose] [] '"value(Presentation.API.Startup).ServiceCollection.AddAuthorization()"' completed successfully!
[2020:06:03 11:05:04.883 PM] [Verbose] [] '"value(Presentation.API.Startup).ServiceCollection.AddHealthChecks()"' was started...
[2020:06:03 11:05:04.884 PM] [Verbose] [] '"value(Presentation.API.Startup).ServiceCollection.AddHealthChecks()"' completed successfully!
[2020:06:03 11:05:04.884 PM] [Verbose] [] '"value(Presentation.API.Startup).AddMvcCore()"' was started...
[2020:06:03 11:05:05.342 PM] [Verbose] [] '"value(Presentation.API.Startup).AddMvcCore()"' completed successfully!
[2020:06:03 11:05:05.343 PM] [Verbose] [] '"value(Presentation.API.Startup).AddSwagger()"' was started...
[2020:06:03 11:05:05.363 PM] [Verbose] [] '"value(Presentation.API.Startup).AddSwagger()"' completed successfully!
[2020:06:03 11:05:08.064 PM] [Information] [Microsoft.Hosting.Lifetime] Now listening on: "http://localhost:5000"
[2020:06:03 11:05:08.064 PM] [Information] [Microsoft.Hosting.Lifetime] Now listening on: "https://localhost:5001"
[2020:06:03 11:05:08.065 PM] [Information] [Microsoft.Hosting.Lifetime] Application started. Press Ctrl+C to shut down.
[2020:06:03 11:05:08.065 PM] [Information] [Microsoft.Hosting.Lifetime] Hosting environment: "Development"
```

---

## ![Download](./docs/media/download_icon.png) Getting Started

### Installation

Run the command:

```powershell
dotnet new -i CleanArchitecture.Templates
```

### Templates

* **ca-sln** - A simple application with three separate presentations
* **ca-sln-sql** - Includes an entity framework core implementation using Sql Server with Docker
* **ca-sln-mongo** (Coming soon!) - Includes a MongoDB implementation

### Create a template

```powershell
dotnet new ca-sln --app-name MyNewApp --secure-port 44399 --port 34399
```

---

## ![Building Blocks](./docs/media/building_blocks.png) Clean Architecture, Domain-Driven Design, MediatR, & AutoMapper

[This article](https://pusher.com/tutorials/clean-architecture-introduction) does a great job expanding upon Clean Architecture. If you haven't already, I highly recommend picking up a copy of [Clean Architecture](https://www.amazon.com/Clean-Architecture-Craftsmans-Software-Structure/dp/0134494164) by Robert C. Martin.

If you so choose, [Domain-Driven Design](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice) excels in this solution structure and makes for great microservices. Of course, a monolithic application is still a reasonable solution to many problems.

[MediatR](https://github.com/jbogard/MediatR) is a 3rd party library that allows you to truly separate your presentation layer from your application layer. For an API or MVC presentation, your controllers become wildly simplified. For a console application, it becomes very easy to do create custom functionality for maintenance or customer support tasks on-the-fly. For an Azure Function, the most you have to worry about is what trigger you are going to use.

[AutoMapper](https://github.com/AutoMapper/AutoMapper) adds a nice convenience to translating your presentation models into the contract requirements for the application, domain, or persistence.

---

## ![Magnifying Glass](./docs/media/telescope.png) Solution Overview

Take a deeper [dive into the solution](./docs/solution_overview.md) to understand the dependencies, assembly responsibilities, and organization setup.

---

## ![Puzzle](./docs/media/puzzle.png) Contributing

Want to add a feature or fix a bug? Glad to have the help! There's just a couple of things to go over before you start submitting pull requests:

* [Commit Message Standards](./docs/commit_message_standards.md)
* [Branching Strategies](./docs/branching_strategies.md)

---

## ![Law](./docs/media/law.png) Licensing

These templates are licensed under the [MIT License](./LICENSE).
