# Database Migration Guide

## Overview

This project uses Entity Framework Core migrations for database schema management. The shared (host) database contains tenant information and host admin data, while each tenant gets its own separate database.

## Connection String

**PostgreSQL 18**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ProjectDb;Username=postgres;Password=123qwe"
  }
}
```

## Initial Migration

The initial migration has been created and applied. It includes:

### Tables Created

1. **Tenants** - Multi-tenant configuration
2. **Users** - User accounts (host and tenant)
3. **Roles** - Role definitions
4. **Permissions** - Permission assignments
5. **Students** - Sample module entity
6. **__EFMigrationsHistory** - EF Core migration tracking

### Host Admin Seeding

On application startup, the following host admin data is automatically seeded:

#### Host Admin User
- **Username**: `admin`
- **Password**: `Admin123!`
- **Email**: `admin@host.com`
- **TenantId**: `null` (Host user)

#### Host Admin Role
- **Name**: `Admin`
- **IsStatic**: `true`
- **TenantId**: `null` (Host role)

#### Host Admin Permissions
- `Tenants.*` - Full tenant management
- `Users.*` - Full user management
- `Roles.*` - Full role management
- `Permissions.*` - Full permission management
- `Students.*` - Full student management

## Migration Commands

### Create a New Migration

```powershell
dotnet ef migrations add <MigrationName> -p Project.Infrastructure -s Project.WebApi
```

Example:
```powershell
dotnet ef migrations add AddUserProfileTable -p Project.Infrastructure -s Project.WebApi
```

### Apply Migrations

```powershell
dotnet ef database update -p Project.Infrastructure -s Project.WebApi
```

### Rollback to Specific Migration

```powershell
dotnet ef database update <MigrationName> -p Project.Infrastructure -s Project.WebApi
```

Example:
```powershell
dotnet ef database update InitialCreate -p Project.Infrastructure -s Project.WebApi
```

### Remove Last Migration (if not applied)

```powershell
dotnet ef migrations remove -p Project.Infrastructure -s Project.WebApi
```

### List All Migrations

```powershell
dotnet ef migrations list -p Project.Infrastructure -s Project.WebApi
```

### Generate SQL Script

```powershell
dotnet ef migrations script -p Project.Infrastructure -s Project.WebApi -o migration.sql
```

## Tenant Database Provisioning

Tenant databases are created programmatically when you create a new tenant via the API:

```http
POST /api/tenants
Authorization: Bearer <token>
Content-Type: application/json

{
  "name": "tenant1",
  "adminPassword": "TenantAdmin123!"
}
```

This will:
1. Create a new database named `ProjectDb_tenant1`
2. Apply all migrations to the new database
3. Seed tenant-specific admin user and permissions

## Seeding Data

### Host Data Seeding

Host data is seeded automatically on application startup via `HostDataSeeder.SeedHostData()` in `Program.cs`.

Location: `Project.Infrastructure/Seeding/HostDataSeeder.cs`

### Tenant Data Seeding

Tenant data is seeded during tenant provisioning via `TenantProvisioningService.SeedTenantDataAsync()`.

Location: `Project.Infrastructure/Services/TenantProvisioningService.cs`

## Login with Host Admin

After running the application for the first time:

```http
POST /api/auth/login
Content-Type: application/json

{
  "userName": "admin",
  "password": "Admin123!"
}
```

Response:
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "a1b2c3...",
  "expiresIn": 28800
}
```

## Database Schema

### Entity Relationships

```
Tenants (1) ──── (N) Users
                      │
                      └─── (N) Permissions (User-level)
                      
Roles (1) ──── (N) Permissions (Role-level)
```

### Tenant Isolation

- **Host Data**: `TenantId = NULL`
- **Tenant Data**: `TenantId = <tenant-guid>`

Global query filters ensure automatic tenant isolation for all queries.

## Troubleshooting

### Connection Issues

If you get connection errors:

1. Verify PostgreSQL is running on port 5432
2. Check username/password in `appsettings.json`
3. Ensure database `ProjectDb` exists or can be created

### Migration Conflicts

If you have migration conflicts:

```powershell
# Remove the last migration
dotnet ef migrations remove -p Project.Infrastructure -s Project.WebApi

# Recreate it
dotnet ef migrations add <MigrationName> -p Project.Infrastructure -s Project.WebApi
```

### Reset Database

To completely reset the database:

```powershell
# Drop the database (in PostgreSQL)
DROP DATABASE "ProjectDb";

# Recreate and apply migrations
dotnet ef database update -p Project.Infrastructure -s Project.WebApi
```

## Production Deployment

For production deployments:

1. **Generate SQL Script**:
   ```powershell
   dotnet ef migrations script -p Project.Infrastructure -s Project.WebApi -o deploy.sql
   ```

2. **Review the script** before applying to production

3. **Apply manually** or via CI/CD pipeline

4. **Never use `database update`** directly in production

## Best Practices

1. **Always create migrations** for schema changes
2. **Test migrations** in development before production
3. **Keep migrations small** and focused
4. **Never modify applied migrations** - create new ones instead
5. **Backup database** before applying migrations in production
6. **Use transactions** for data migrations (EF Core does this automatically)

## Migration History

| Migration | Date | Description |
|-----------|------|-------------|
| InitialCreate | 2025-11-24 | Initial database schema with all entities |

## Next Steps

1. Run the application: `dotnet run --project Project.WebApi`
2. Login with host admin credentials
3. Create your first tenant via `/api/tenants`
4. Start building your application!
