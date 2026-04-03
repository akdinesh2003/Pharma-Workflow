# Pharma Workflow Management System - Setup Instructions

## Prerequisites

Before you begin, ensure you have the following installed:

- **Node.js** 18.x or higher
- **.NET 8 SDK**
- **SQL Server 2019** or higher (Express Edition is fine)
- **Visual Studio Code** or **Visual Studio 2022**
- **SQL Server Management Studio (SSMS)** (optional but recommended)

## Step 1: Database Setup

### 1.1 Create Database

1. Open SQL Server Management Studio (SSMS)
2. Connect to your SQL Server instance
3. Open and execute `Database/01_CreateDatabase.sql`
4. Open and execute `Database/02_CreateTables.sql`
5. Open and execute `Database/03_SeedData.sql`

### 1.2 Verify Database

Run this query to verify:
```sql
USE PharmaWorkflowDB;
SELECT * FROM Users;
SELECT * FROM Main_Transactions;
SELECT * FROM History_Transactions;
```

## Step 2: Backend Setup (.NET Web API)

### 2.1 Update Connection String

1. Navigate to `Backend/PharmaWorkflowAPI/`
2. Open `appsettings.json`
3. Update the connection string to match your SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=PharmaWorkflowDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

Common server names:
- `localhost` or `(localdb)\\MSSQLLocalDB` for LocalDB
- `.\\SQLEXPRESS` for SQL Server Express
- `localhost,1433` for Docker SQL Server

### 2.2 Restore and Run

Open terminal in `Backend/PharmaWorkflowAPI/` and run:

```bash
dotnet restore
dotnet build
dotnet run
```

The API will start on: `https://localhost:7001`

### 2.3 Test API

Open browser and navigate to: `https://localhost:7001/swagger`

You should see the Swagger UI with all API endpoints.

## Step 3: Frontend Setup (Angular)

### 3.1 Install Dependencies

Open terminal in `Frontend/pharma-workflow-app/` and run:

```bash
npm install
```

### 3.2 Update API URL (if needed)

If your backend runs on a different port, update the API URL in:
- `src/app/services/auth.service.ts`
- `src/app/services/transaction.service.ts`

Change `https://localhost:7001/api` to your backend URL.

### 3.3 Run Application

```bash
npm start
```

The application will start on: `http://localhost:4200`

## Step 4: Login and Test

### Default User Credentials

| Username | Password     | Role    | Permissions                    |
|----------|--------------|---------|--------------------------------|
| admin    | Admin@123    | Admin   | Full access (all operations)   |
| manager  | Manager@123  | Manager | Approve/Reject requests        |
| user     | User@123     | User    | Create requests only           |

### Test Workflow

1. Login as **user** (user / User@123)
2. Create a new request:
   - Drug Name: Aspirin 100mg
   - Batch No: ASP2026B10
   - Comments: Test batch
3. Logout and login as **manager** (manager / Manager@123)
4. Go to "All Requests"
5. Click "Review" on the new request
6. Approve or Reject with comments
7. View the history to see audit trail

## Troubleshooting

### Backend Issues

**Error: Cannot connect to database**
- Verify SQL Server is running
- Check connection string in appsettings.json
- Ensure database exists and tables are created

**Error: Port 7001 already in use**
- Change port in `Properties/launchSettings.json`
- Update frontend API URLs accordingly

### Frontend Issues

**Error: Cannot connect to API**
- Ensure backend is running on https://localhost:7001
- Check browser console for CORS errors
- Verify API URLs in service files

**Error: npm install fails**
- Clear npm cache: `npm cache clean --force`
- Delete node_modules and package-lock.json
- Run `npm install` again

### CORS Issues

If you see CORS errors in browser console:
1. Verify backend CORS policy in `Program.cs`
2. Ensure frontend URL matches the allowed origin
3. Restart both backend and frontend

## Production Deployment

### Backend

1. Update `appsettings.Production.json` with production connection string
2. Build for production: `dotnet publish -c Release`
3. Deploy to IIS, Azure App Service, or Docker

### Frontend

1. Build for production: `npm run build`
2. Deploy `dist/pharma-workflow-app` folder to web server
3. Update API URLs to production backend

### Database

1. Backup development database
2. Restore to production SQL Server
3. Update connection strings
4. Implement proper password hashing (replace demo authentication)

## Security Notes

⚠️ **Important for Production:**

1. Replace demo password verification with proper hashing (BCrypt, Argon2)
2. Use environment variables for sensitive configuration
3. Implement rate limiting on API endpoints
4. Enable HTTPS only
5. Add input validation and sanitization
6. Implement proper logging and monitoring
7. Regular security audits

## Support

For issues or questions:
- Check the README.md for overview
- Review API documentation at /swagger endpoint
- Check browser console and backend logs for errors
