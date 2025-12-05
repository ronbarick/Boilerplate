# Postman Collection Auto-Update

This folder contains a script to automatically generate and update your Postman collection from your Swagger/OpenAPI specification.

## Prerequisites

The `openapi-to-postmanv2` package must be installed globally:
```bash
npm install -g openapi-to-postmanv2
```

## Usage

### Step 1: Run Your Application
Start your application:
```bash
cd Project.WebApi
dotnet run
```

The application will run on `http://localhost:5197`.

### Step 2: Update Postman Collection
In a new terminal, run the update script:
```bash
cd Backend
.\update-postman.ps1
```

This will:
1. Download `swagger.json` from your running application
2. Convert it to `Project_API.postman_collection.json`

### Step 3: Import to Postman
Import the generated `Project_API.postman_collection.json` file into Postman.

## When to Run

Run `update-postman.ps1` whenever you:
- Add new API endpoints
- Modify existing endpoint signatures
- Change request/response models
- Update API documentation

## Troubleshooting

**Error: "Failed to download swagger.json"**
- Make sure your application is running
- Check that it's accessible at `http://localhost:5197/swagger/v1/swagger.json`
- Verify your firewall isn't blocking the connection

**Error: "openapi-to-postmanv2: command not found"**
- Run: `npm install -g openapi-to-postmanv2`
