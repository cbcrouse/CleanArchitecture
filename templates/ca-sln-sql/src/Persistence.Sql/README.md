# Persistence.Sql Overview

This project uses a ['code-first'](https://www.tutorialspoint.com/entity_framework/entity_framework_code_first_approach.htm) approach. Below you will find helpful information to get started.

Make sure Docker Desktop ([Mac](https://docs.docker.com/docker-for-mac/install/)/[Windows](https://docs.docker.com/docker-for-windows/install/)) is installed locally before continuing.

---

## Getting Started with Sql Server on Docker

In order to get SqlServer running locally, run the following commands:

```powershell
# Make sure nothing else is running
docker rm $(docker ps -qa) -f; docker network prune -f; docker volume prune -f;

# Start up SqlServer
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=SqlSecret!' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2017-latest
```

To test the SqlServer connection, run the following command:

```powershell
# Run 'docker ps' to get the container ID or name.
docker exec -it <container_id|container_name> /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P SqlSecret!
```

---

## Update the Configuration

Within the Infrastructure project are several appsettings json files. The `appsettings.core.Development` file will already have the local connection string set, but you can change the database name here if needed.

---

## EF Core Migrations

Code-first migrations are simple thanks to [Entity Framework Core Tools](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet). You'll need to run a few commands to make sure the database is configured and up-to-date.

#### Add First Migration

```powershell
# If you do not see a "Migrations" folder in the project, an initial migration needs to be created.
dotnet ef migrations add InitialCreate --project <Path>/Persistence.Sql.csproj --startup-project <Path>/Presentation.API.csproj
```

#### Apply Existing Migrations

```powershell
# This will apply any migrations that have not been applied to the database yet.
dotnet ef database update --project <Path>/Persistence.Sql.csproj --startup-project <Path>/Presentation.API.csproj
```

#### Update to a Specific Migration

```powershell
# This will apply the necessary migrations to get to the 'InitialCreate' migration state.
dotnet ef database update 'InitialCreate' --project <Path>/Persistence.Sql.csproj --startup-project <Path>/Presentation.API.csproj
```

---