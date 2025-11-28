# Quick Start Guide

## 🚀 Getting Started

### 1. Database Setup

The database has been created and migrated. You're ready to go!

**Database**: `ProjectDb` on PostgreSQL 18 (localhost:5432)

### 2. Host Admin Credentials

Use these credentials to login as the host administrator:

```
Username: admin
Password: Admin123!
Email: admin@host.com
```

### 3. Start the Application

```powershell
cd Project.WebApi
dotnet run
```

The API will start at: `https://localhost:5001` (or check console output)

### 4. Access Swagger UI

Open your browser and navigate to:
```
https://localhost:5001/swagger
```

## 📝 First Steps

### Step 1: Login as Host Admin

```http
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "userName": "admin",
  "password": "Admin123!"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "a1b2c3...",
  "expiresIn": 28800
}
```

Copy the `accessToken` for use in subsequent requests.

### Step 2: Create Your First Tenant

```http
POST https://localhost:5001/api/tenants
Authorization: Bearer <your-access-token>
Content-Type: application/json

{
  "name": "tenant1",
  "adminPassword": "TenantAdmin123!"
}
```

This will:
- ✅ Create a new database `ProjectDb_tenant1`
- ✅ Apply all migrations
- ✅ Create tenant admin user
- ✅ Seed default roles and permissions

### Step 3: Login as Tenant Admin

```http
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "userName": "admin",
  "password": "TenantAdmin123!"
}
```

### Step 4: Access Tenant Data

```http
GET https://localhost:5001/api/students
Authorization: Bearer <tenant-admin-token>
X-Tenant-Name: tenant1
```

## 🔑 Host Admin Permissions

The host admin has full access to:

- ✅ `Tenants.*` - Create, manage, delete tenants
- ✅ `Users.*` - Manage all users (host and tenant)
- ✅ `Roles.*` - Manage roles
- ✅ `Permissions.*` - Manage permissions
- ✅ `Students.*` - Access all student data

## 🏢 Multi-Tenancy

### Tenant Resolution

Tenants are identified by:

1. **X-Tenant-Id** header (UUID)
2. **X-Tenant-Name** header (string)
3. **User JWT claims** (automatic)

### Example with Tenant Header

```http
GET https://localhost:5001/api/students
Authorization: Bearer <token>
X-Tenant-Name: tenant1
```

## 📊 Available Endpoints

### Authentication (No Auth Required)
- `POST /api/auth/login` - Login
- `POST /api/auth/register` - Register new user

### Tenants (Requires: Tenants.Manage)
- `GET /api/tenants` - List all tenants
- `POST /api/tenants` - Create new tenant

### Users (Requires: Users.Manage)
- `GET /api/users` - List users
- `POST /api/users` - Create user

### Students (Requires: Students.Manage)
- `GET /api/students` - List students
- `POST /api/students` - Create student

## 🛠️ Useful Commands

### View Migrations
```powershell
dotnet ef migrations list -p Project.Infrastructure -s Project.WebApi
```

### Create New Migration
```powershell
dotnet ef migrations add <MigrationName> -p Project.Infrastructure -s Project.WebApi
```

### Apply Migrations
```powershell
dotnet ef database update -p Project.Infrastructure -s Project.WebApi
```

## 📚 Documentation

- **README.md** - Full project documentation
- **MIGRATION_GUIDE.md** - Database migration guide
- **Swagger UI** - Interactive API documentation at `/swagger`

## 🔒 Security Notes

1. **Change default passwords** in production
2. **Update JWT secret** in `appsettings.json`
3. **Use HTTPS** in production
4. **Implement rate limiting** for auth endpoints
5. **Enable CORS** only for trusted origins

## 🎯 Next Steps

1. ✅ Login with host admin
2. ✅ Create your first tenant
3. ✅ Explore the API via Swagger
4. ✅ Add your custom business logic
5. ✅ Extend the Student module or create new modules

## 💡 Tips

- Use **Postman** or **Thunder Client** for API testing
- Check **Swagger UI** for request/response schemas
- Review **MIGRATION_GUIDE.md** for database operations
- Host admin can manage everything across all tenants
- Each tenant is completely isolated with its own database

## 🐛 Troubleshooting

### Can't connect to database?
- Verify PostgreSQL is running
- Check connection string in `appsettings.json`
- Ensure password is `123qwe`

### 403 Forbidden errors?
- Verify you're logged in with correct user
- Check you have the required permission
- Ensure tenant header is set for tenant operations

### Migration errors?
- See **MIGRATION_GUIDE.md** for detailed troubleshooting

---

**You're all set! Start building your multi-tenant application! 🚀**
