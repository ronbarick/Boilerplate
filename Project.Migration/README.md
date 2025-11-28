# Project.Migration

This project contains database migrations and seeding logic for the application.

## Purpose

The `Project.Migration` project is separated from `Project.Infrastructure` to:
- Keep migration history isolated and manageable
- Provide a dedicated location for database seeding logic
- Enable design-time DbContext creation for EF Core tools

## Structure

```
Project.Migration/
├── Migrations/              # EF Core migration files
├── Seeding/                 # Database seeding logic
│   └── HostDataSeeder.cs   # Seeds host admin user and permissions
├── AppDbContextFactory.cs   # Design-time DbContext factory
├── appsettings.json        # Configuration for migrations
└── Project.Migration.csproj # Project file
```

## Running Migrations

All migration commands should be run from the `Project.Migration` directory.

### Add a New Migration

```powershell
cd Project.Migration
dotnet ef migrations add MigrationName
```

### Update Database

```powershell
cd Project.Migration
dotnet ef database update
```

### Remove Last Migration

```powershell
cd Project.Migration
dotnet ef migrations remove
```

### List Migrations

```powershell
cd Project.Migration
dotnet ef migrations list
```

## Configuration

The `appsettings.json` file contains the connection string used during design-time operations:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ProjectDb;Username=postgres;Password=postgres"
  }
}
```

Update this connection string to match your PostgreSQL database configuration.

## Seeding

The `HostDataSeeder` class seeds the following data:
- Host admin user (username: `admin`, password: `Admin123!`)
- Admin role with full permissions
- Permission definitions for Tenants, Users, Roles, and Students

Seeding is automatically executed when the application starts (see `Program.cs` in `Project.WebApi`).

## Dependencies

- **Microsoft.EntityFrameworkCore.Design** - Design-time support for EF Core
- **Microsoft.EntityFrameworkCore.Tools** - EF Core CLI tools
- **Npgsql.EntityFrameworkCore.PostgreSQL** - PostgreSQL provider
- **Microsoft.Extensions.Configuration.Json** - JSON configuration support
- **Microsoft.Extensions.Configuration.EnvironmentVariables** - Environment variables support
- **BCrypt.Net-Next** - Password hashing for seeding

## Notes

- This project references both `Project.Core` and `Project.Infrastructure`
- The `AppDbContextFactory` provides dummy implementations of `ICurrentTenant` and `ICurrentUser` for design-time operations
- All migration files use the `Project.Migration.Migrations` namespace
