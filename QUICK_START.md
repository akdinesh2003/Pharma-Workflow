# Quick Start Guide - 5 Minutes to Running System

## Prerequisites Check
- [ ] .NET 8 SDK installed
- [ ] Node.js 18+ installed
- [ ] SQL Server running
- [ ] Ports 7001 and 4200 available

## Step 1: Database (2 minutes)

Open SQL Server Management Studio and run these 3 scripts in order:

```sql
-- 1. Create database
USE master;
CREATE DATABASE PharmaWorkflowDB;

-- 2. Create tables (run Database/02_CreateTables.sql)

-- 3. Insert sample data (run Database/03_SeedData.sql)
```

## Step 2: Backend (1 minute)

```bash
cd Backend/PharmaWorkflowAPI
dotnet restore
dotnet run
```

✅ Backend running at: https://localhost:7001
✅ Swagger UI: https://localhost:7001/swagger

## Step 3: Frontend (2 minutes)

```bash
cd Frontend/pharma-workflow-app
npm install
npm start
```

✅ Frontend running at: http://localhost:4200

## Step 4: Login & Test

Open browser: http://localhost:4200

**Login with:**
- Username: `admin`
- Password: `Admin@123`

**Quick Test:**
1. Click "New Request"
2. Fill form:
   - Drug: Aspirin 100mg
   - Batch: ASP2026TEST
3. Submit
4. View in "All Requests"
5. Click "History" to see audit trail

## Default Users

| Username | Password     | Role    |
|----------|--------------|---------|
| admin    | Admin@123    | Admin   |
| manager  | Manager@123  | Manager |
| user     | User@123     | User    |

## Troubleshooting

### Backend won't start
```bash
# Check connection string in appsettings.json
# Update Server name to match your SQL Server instance
"Server=localhost;Database=PharmaWorkflowDB;..."
```

### Frontend won't start
```bash
# Clear cache and reinstall
npm cache clean --force
rm -rf node_modules package-lock.json
npm install
```

### Can't connect to API
- Ensure backend is running on https://localhost:7001
- Check browser console for CORS errors
- Verify firewall isn't blocking ports

## Next Steps

1. Read FEATURES.md for complete feature list
2. Check API_TESTING.md for Postman examples
3. Review SETUP_INSTRUCTIONS.md for detailed setup
4. Explore FOLDER_STRUCTURE.md to understand architecture

## Key Features to Try

✅ Create new pharmaceutical requests
✅ Approve/Reject workflow (login as manager)
✅ View complete audit trail
✅ Search and filter transactions
✅ Dashboard with statistics
✅ Role-based access control

## Production Deployment

For production deployment:
1. Update connection strings
2. Implement proper password hashing (BCrypt)
3. Configure HTTPS certificates
4. Set up environment variables
5. Enable logging and monitoring
6. Configure backup strategy

## Support

- Check README.md for overview
- Review documentation files
- Test API with Swagger UI
- Check browser console for errors
- Review backend logs

---

**System is ready! Start managing pharmaceutical workflows! 🏥**
