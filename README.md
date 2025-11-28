# C# Namespace Usage Rule
When generating C# code:
- You must NEVER use fully-qualified namespaces like:
  System.Text.StringBuilder sb = new System.Text.StringBuilder();

- Instead, you MUST:
  1. Add required namespaces at the top using `using` statements.
  2. Use only class names inside the code body, for example:
     var sb = new StringBuilder();

- If a class exists in multiple namespaces:
   - Pick the most commonly used namespace.
   - Still add `using` for it.
   - Do NOT use fully-qualified names.

- If a namespace is unknown:
   - Do NOT guess a fully-qualified name.
   - Use only the class name.
   - Add a comment: // Add using for correct namespace

- All generated code MUST follow this rule strictly.




# ABP-Style Multi-Tenant Boilerplate

A clean, modular, multi-tenant ASP.NET Core application inspired by ABP Framework.

## Features

- **Multi-Tenancy**: Separate database per tenant + Shared database for host
- **Hierarchical Permissions**: User > Role > Definition with wildcard support (e.g., `Rooms.*` covers `Rooms.Create`)
- **Clean Architecture**: DDD layers (Core, Infrastructure, Application, WebApi)
- **Minimal APIs**: Modern ASP.NET Core 9 endpoints
- **JWT Authentication**: Secure token-based auth with refresh tokens
- **Auto Auditing**: Automatic tracking of creation, modification, and deletion
- **Soft Delete**: Entities are marked as deleted, not physically removed
- **PostgreSQL**: Production-ready database with EF Core

## Project Structure

```
Project.sln
├── Project.Core/              # Domain entities, interfaces
│   ├── Entities/
│   │   ├── Base/             # Entity, AuditedEntity, FullAuditedEntity
│   │   ├── Tenant.cs
│   │   ├── User.cs
│   │   ├── Role.cs
│   │   ├── Permission.cs
│   │   └── Student.cs
│   └── Interfaces/           # ICurrentUser, ICurrentTenant, IPermissionChecker
├── Project.Infrastructure/    # Data access, EF Core
│   ├── Data/
│   │   └── AppDbContext.cs
│   └── Services/
│       └── TenantProvisioningService.cs
├── Project.Application/       # Business logic, DTOs
│   ├── Services/
│   │   ├── AppServiceBase.cs
│   │   ├── PermissionChecker.cs
│   │   ├── TenantAppService.cs
│   │   ├── UserAppService.cs
│   │   └── StudentAppService.cs
│   └── Dtos/
└── Project.WebApi/            # API endpoints, middleware
    ├── Endpoints/
    │   ├── AuthEndpoints.cs
    │   ├── TenantEndpoints.cs
    │   ├── UserEndpoints.cs
    │   └── StudentEndpoints.cs
    ├── Middleware/
    │   └── TenantResolverMiddleware.cs
    ├── Services/
    │   ├── CurrentUser.cs
    │   ├── CurrentTenant.cs
    │   └── JwtTokenService.cs
    └── Program.cs
```

## Getting Started

### Prerequisites

- .NET 9 SDK
- PostgreSQL 12+

### Configuration

1. Update `appsettings.json` in `Project.WebApi`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ProjectDb;Username=postgres;Password=yourpassword"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "ProjectApi",
    "Audience": "ProjectClient"
  }
}
```

### Database Migrations

#### Shared Database (Host)

```powershell
# Create migration
dotnet ef migrations add Initial -p Project.Infrastructure -s Project.WebApi

# Apply migration
dotnet ef database update -p Project.Infrastructure -s Project.WebApi
```

#### Tenant Databases

Tenant databases are created programmatically when a new tenant is provisioned via the `/api/tenants` endpoint.

### Running the Application

```powershell
cd Project.WebApi
dotnet run
```

The API will be available at `https://localhost:5001` (or the port shown in the console).

## API Endpoints

### Authentication

- `POST /api/auth/login` - Login with username/password
- `POST /api/auth/register` - Register a new user

### Tenants (Requires `Tenants.Manage` permission)

- `GET /api/tenants` - List all tenants
- `POST /api/tenants` - Create a new tenant (provisions separate database)

### Users (Requires `Users.Manage` permission)

- `GET /api/users` - List all users
- `POST /api/users` - Create a new user

### Students (Requires `Students.Manage` permission)

- `GET /api/students` - List all students
- `POST /api/students` - Create a new student

## Permission System

### Hierarchical Permissions

Permissions support hierarchical wildcards:
- `Rooms.*` grants access to `Rooms.Create`, `Rooms.Update`, `Rooms.Delete`, etc.
- Specific permissions override wildcards

### Permission Resolution Order

1. **User Permissions** (highest priority) - Direct user overrides
2. **Role Permissions** - Permissions assigned to user's roles
3. **Default** - Returns `403 Forbidden` if not granted

### Example Usage

```csharp
// In AppService
await CheckPermissionAsync("Students.Manage");
```

## Multi-Tenancy

### Tenant Resolution

Tenants are resolved in the following order:

1. `X-Tenant-Id` header
2. `X-Tenant-Name` header
3. User claims (if authenticated)

### Tenant Isolation

- Each tenant has a separate database
- Global query filters ensure data isolation
- `TenantId` is automatically set on entity creation

## Authentication

### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "userName": "admin",
  "password": "yourpassword"
}
```

Response:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "a1b2c3d4...",
  "expiresIn": 28800
}
```

### Using the Token

```http
GET /api/students
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
X-Tenant-Name: tenant1
```

## Auditing

All entities inheriting from `FullAuditedEntity` automatically track:

- `CreationTime` - When the entity was created
- `CreatorId` - Who created the entity
- `LastModificationTime` - When last modified
- `LastModifierId` - Who last modified
- `IsDeleted` - Soft delete flag
- `DeletionTime` - When deleted
- `DeleterId` - Who deleted

## Development

### Adding a New Module

1. **Create Entity** in `Project.Core/Entities/`
2. **Add DbSet** in `AppDbContext`
3. **Create DTOs** in `Project.Application/Dtos/`
4. **Create AppService** in `Project.Application/Services/`
5. **Create Endpoints** in `Project.WebApi/Endpoints/`
6. **Map Endpoints** in `Program.cs`

### Adding Permissions

1. Seed permissions in `TenantProvisioningService.SeedTenantDataAsync()`
2. Use `CheckPermissionAsync("Permission.Name")` in AppServices
3. Permissions are checked via `PermissionChecker` with hierarchical support

## Swagger/OpenAPI

Swagger UI is available at `/swagger` in development mode.

## Architecture Principles

- **Clean Architecture**: Clear separation of concerns
- **Dependency Inversion**: Core has no dependencies
- **Domain-Driven Design**: Rich domain models
- **CQRS-lite**: Separate DTOs for commands/queries
- **Repository Pattern**: Via EF Core DbContext

## Technologies

- ASP.NET Core 9
- Entity Framework Core 9
- PostgreSQL (Npgsql)
- JWT Bearer Authentication
- BCrypt.NET for password hashing
- Swashbuckle (Swagger/OpenAPI)

## License

This project is provided as-is for educational and commercial use.

## Contributing

Contributions are welcome! Please follow clean code principles and maintain the existing architecture style.
