# Complete Full-Stack Workflow Management System - Build Guide

## 🎯 What You'll Build
A complete pharmaceutical workflow management system with:
- Angular 17 Frontend (User Interface)
- .NET 10 Web API Backend (Server)
- SQL Server Database
- JWT Authentication with Role-Based Access Control
- Complete Audit Trail System

---

## 📋 Prerequisites (Install These First)

### 1. Install Node.js
- Download from: https://nodejs.org/
- Version: 18 or higher
- Check: `node --version`

### 2. Install .NET SDK
- Download from: https://dotnet.microsoft.com/download
- Version: .NET 10
- Check: `dotnet --version`

### 3. Install SQL Server
- Download: SQL Server 2022 Express
- Link: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
- Choose "Express" edition (free)

### 4. Install SQL Server Management Studio (SSMS)
- Download from: https://aka.ms/ssmsfullsetup
- This is for managing your database

### 5. Install Angular CLI
```bash
npm install -g @angular/cli
```

### 6. Install Code Editor
- Visual Studio Code: https://code.visualstudio.com/

---

## 🚀 STEP-BY-STEP BUILD PROCESS

### STEP 1: Initial Prompt to AI (Kiro/Claude/ChatGPT)

Copy and paste this EXACT prompt:

```
Create a complete full-stack pharmaceutical workflow management system with the following requirements:

BACKEND (.NET 10 Web API):
- Create REST API with .NET 10
- Use SQL Server database
- Implement JWT authentication
- Role-based access control (3 roles: User, Manager, Admin)
- ACID transaction management using TransactionScope
- Complete audit trail in history table
- Soft delete functionality (IsDeleted flag)
- Optimistic concurrency control using RowVersion

DATABASE (SQL Server):
Create 3 tables:
1. Users table (Id, Username, Password, FullName, Email, Role, CreatedDate)
2. Main_Transactions table (Id, DrugName, BatchNo, RequestedBy, Status, CreatedDate, UpdatedDate, FilePath, Comments, IsDeleted, RowVersion)
3. History_Transactions table (Id, TransactionId, Action, ActionBy, ActionDate, PreviousStatus, NewStatus, Comments)

APIS TO CREATE (10 total):
1. POST /api/auth/login - Login with JWT token
2. POST /api/transactions - Create new transaction
3. GET /api/transactions - Get all transactions with filter
4. GET /api/transactions/{id} - Get single transaction
5. PUT /api/transactions/{id}/approve - Approve transaction
6. PUT /api/transactions/{id}/reject - Reject transaction
7. PUT /api/transactions/{id}/modify - Modify transaction
8. DELETE /api/transactions/{id} - Soft delete transaction
9. GET /api/transactions/{id}/history - Get transaction history
10. GET /api/transactions/dashboard/stats - Get dashboard statistics

FRONTEND (Angular 17):
- Login page with authentication
- Dashboard with statistics cards
- Transaction list with search and filter
- Create new transaction form
- Approve/Reject transaction page
- Transaction history modal
- Role-based button visibility
- Responsive design with modern UI

FEATURES:
- User role can only create transactions
- Manager role can create, approve, reject, modify
- Admin role can do everything including delete
- All actions must be recorded in history table
- Soft delete (data preserved in database)
- Complete audit trail for regulatory compliance
- JWT token expires in 8 hours

SAMPLE DATA:
- Create 3 users (admin/admin123, manager/manager123, user/user123)
- Create 10 sample pharmaceutical transactions
- Include various statuses: Initiated, Registered, Approved, Rejected

PROJECT STRUCTURE:
Backend/PharmaWorkflowAPI/
  - Controllers/
  - Services/
  - Models/
  - DTOs/
  - Data/
Frontend/pharma-workflow-app/
  - src/app/components/
  - src/app/services/
  - src/app/guards/
Database/
  - 01_CreateDatabase.sql
  - 02_CreateTables.sql
  - 03_SeedData.sql

Please create all files with complete working code.
```

---

### STEP 2: Setup Database

1. **Open SQL Server Management Studio (SSMS)**

2. **Connect to Server:**
   - Server name: `localhost\SQLEXPRESS`
   - Authentication: Windows Authentication
   - Click "Connect"

3. **Run SQL Scripts in Order:**

   **First Script (01_CreateDatabase.sql):**
   - Click "New Query"
   - Copy paste the database creation script
   - Click "Execute" (or press F5)

   **Second Script (02_CreateTables.sql):**
   - Click "New Query"
   - Copy paste the tables creation script
   - Click "Execute"

   **Third Script (03_SeedData.sql):**
   - Click "New Query"
   - Copy paste the seed data script
   - Click "Execute"

4. **Verify Database:**
   ```sql
   USE PharmaWorkflowDB;
   SELECT * FROM Users;
   SELECT * FROM Main_Transactions;
   SELECT * FROM History_Transactions;
   ```

---

### STEP 3: Setup Backend

1. **Navigate to Backend Folder:**
   ```bash
   cd Backend/PharmaWorkflowAPI
   ```

2. **Update Connection String:**
   Open `appsettings.json` and verify:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=PharmaWorkflowDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
   }
   ```

3. **Restore Packages:**
   ```bash
   dotnet restore
   ```

4. **Build Project:**
   ```bash
   dotnet build
   ```

5. **Run Backend:**
   ```bash
   dotnet run
   ```

   **Expected Output:**
   ```
   Now listening on: https://localhost:7001
   Now listening on: http://localhost:5000
   ```

6. **Test API:**
   - Open browser: https://localhost:7001/swagger
   - You should see Swagger UI with all APIs

---

### STEP 4: Setup Frontend

1. **Navigate to Frontend Folder:**
   ```bash
   cd Frontend/pharma-workflow-app
   ```

2. **Install Dependencies:**
   ```bash
   npm install
   ```
   (This takes 2-3 minutes)

3. **Start Frontend:**
   ```bash
   npm start
   ```

   **Expected Output:**
   ```
   ** Angular Live Development Server is listening on localhost:4200 **
   ✔ Compiled successfully.
   ```

4. **Open Browser:**
   - Go to: http://localhost:4200
   - You should see the login page

---

### STEP 5: Test the System

#### Test 1: Login as Admin
- Username: `admin`
- Password: `admin123`
- Should see dashboard with statistics

#### Test 2: View All Transactions
- Click "All Requests" in sidebar
- Should see list of transactions
- Try search and filter

#### Test 3: Create New Transaction
- Click "New Request"
- Fill form:
  - Drug Name: Aspirin
  - Batch No: BATCH001
  - Requested By: John Doe
  - Comments: Test request
- Click "Submit"
- Should see success message

#### Test 4: Register Transaction
- Find a transaction with "Initiated" status
- Click "Register" button
- Status should change to "Registered"

#### Test 5: Approve Transaction
- Click "Review" button on a Registered transaction
- Select "Approve"
- Add comments
- Click "Submit"
- Status should change to "Approved"

#### Test 6: View History
- Click "History" button on any transaction
- Should see popup with all actions

#### Test 7: Delete Transaction (Admin Only)
- Click "Delete" button
- Confirm deletion
- Transaction should disappear from list

#### Test 8: Test Different Roles
- Logout
- Login as Manager (manager/manager123)
  - Should NOT see Delete button
- Login as User (user/user123)
  - Should ONLY see "New Request" option

---

### STEP 6: Verify Database Updates

Run these queries in SSMS:

```sql
-- Check all transactions
SELECT * FROM Main_Transactions WHERE IsDeleted = 0;

-- Check deleted transactions
SELECT * FROM Main_Transactions WHERE IsDeleted = 1;

-- Check complete history
SELECT * FROM History_Transactions ORDER BY ActionDate DESC;

-- Check users
SELECT * FROM Users;
```

---

## 🎨 CUSTOMIZATION FOR YOUR FRIEND

To make it different, your friend can change:

### 1. Change Domain/Theme
Replace "Pharmaceutical" with:
- Inventory Management
- Employee Leave Management
- Purchase Order System
- Document Approval System
- Customer Support Tickets

### 2. Change Database Fields
In Main_Transactions table, replace:
- `DrugName` → `ProductName` / `EmployeeName` / `DocumentTitle`
- `BatchNo` → `OrderNo` / `TicketNo` / `ReferenceNo`
- `RequestedBy` → `CreatedBy` / `SubmittedBy`

### 3. Change Status Values
Replace workflow statuses:
- Initiated → Draft / Pending / Submitted
- Registered → In Review / Assigned
- Approved → Completed / Closed / Accepted
- Rejected → Cancelled / Declined

### 4. Change Colors/Design
In `src/styles.css`, modify:
- Primary color: `#2c3e50` → any color
- Success color: `#27ae60` → any color
- Danger color: `#e74c3c` → any color

### 5. Change Roles
Replace User/Manager/Admin with:
- Employee / Supervisor / Director
- Viewer / Editor / Owner
- Staff / Lead / Executive

---

## 🔧 COMMON ISSUES & SOLUTIONS

### Issue 1: Backend won't start
**Error:** "Framework not found"
**Solution:** Install correct .NET version
```bash
dotnet --list-sdks
```
Should show version 10.x.x

### Issue 2: Database connection fails
**Error:** "Cannot connect to SQL Server"
**Solution:** 
- Check SQL Server is running
- Verify connection string uses `localhost\SQLEXPRESS`
- Add `TrustServerCertificate=True`

### Issue 3: Frontend shows errors
**Error:** "angular.json not found" or corrupted
**Solution:** 
- Delete angular.json
- Recreate it with proper configuration

### Issue 4: JWT token errors
**Error:** "401 Unauthorized"
**Solution:**
- Check token is being sent in headers
- Verify JWT secret key matches in backend
- Check token hasn't expired

### Issue 5: CORS errors
**Error:** "CORS policy blocked"
**Solution:** In `Program.cs`, add:
```csharp
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", builder => {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

---

## 📚 KEY CONCEPTS EXPLAINED

### 1. JWT Authentication
- User logs in with username/password
- Server creates a token (like a digital key)
- Token is sent with every request
- Server verifies token before allowing access

### 2. Role-Based Access Control (RBAC)
- Each user has a role (User/Manager/Admin)
- Each API checks user's role
- Buttons show/hide based on role

### 3. ACID Transactions
- Atomic: All changes happen or none happen
- Consistent: Database stays valid
- Isolated: Changes don't interfere
- Durable: Changes are permanent

### 4. Audit Trail
- Every action is recorded in History table
- Shows who did what and when
- Required for regulatory compliance

### 5. Soft Delete
- Data is not actually deleted
- IsDeleted flag is set to 1
- Data can be recovered if needed

### 6. Optimistic Concurrency
- RowVersion tracks changes
- Prevents two users from editing same record
- Shows error if data was modified

---

## 📊 PROJECT STATISTICS

- **Total Files Created:** 50+
- **Lines of Code:** 3000+
- **APIs:** 10
- **Database Tables:** 3
- **Frontend Components:** 7
- **Backend Services:** 2
- **Time to Build:** 2-3 hours (with AI help)

---

## 🎓 WHAT YOU LEARNED

1. Full-stack development (Frontend + Backend + Database)
2. REST API design and implementation
3. JWT authentication and authorization
4. Database design and relationships
5. Transaction management
6. Audit trail implementation
7. Role-based access control
8. Angular component architecture
9. .NET Web API development
10. SQL Server database management

---

## 📝 PRESENTATION TIPS FOR MANAGER

When explaining to your manager, focus on:

1. **Business Value:**
   - Complete audit trail for compliance
   - Role-based security
   - Data integrity with ACID transactions

2. **Technical Highlights:**
   - Modern tech stack (Angular + .NET + SQL Server)
   - RESTful API architecture
   - JWT security
   - Scalable design

3. **Key Features:**
   - Dashboard with real-time statistics
   - Complete workflow management
   - History tracking for every action
   - Soft delete for data recovery

4. **Challenging Parts:**
   - ACID transaction management
   - JWT authentication implementation
   - Complete audit trail system
   - Optimistic concurrency control

---

## 🚀 NEXT STEPS (ENHANCEMENTS)

After basic system works, you can add:

1. **Email Notifications**
   - Send email when transaction is approved/rejected

2. **File Upload**
   - Attach documents to transactions

3. **Reports**
   - Generate PDF reports
   - Export to Excel

4. **Advanced Search**
   - Date range filter
   - Multiple field search

5. **User Management**
   - Admin can create/edit users
   - Password reset functionality

6. **Dashboard Charts**
   - Pie charts for status distribution
   - Line charts for trends

---

## 📞 SUPPORT

If you face issues:
1. Check error messages carefully
2. Verify all prerequisites are installed
3. Check database connection
4. Ensure both backend and frontend are running
5. Check browser console for errors (F12)

---

## ✅ FINAL CHECKLIST

Before showing to manager:

- [ ] Database has sample data
- [ ] Backend runs without errors
- [ ] Frontend runs without errors
- [ ] Can login with all 3 roles
- [ ] Can create new transaction
- [ ] Can approve/reject transaction
- [ ] Can view history
- [ ] Can delete (as admin)
- [ ] All buttons show/hide based on role
- [ ] Dashboard shows correct statistics

---

## 🎉 CONGRATULATIONS!

You've built a complete enterprise-level workflow management system!

**Share this guide with your friend and they can build their own version with different design and domain!**

---

**Created by:** Your Name
**Date:** April 2026
**Tech Stack:** Angular 17 + .NET 10 + SQL Server
**Total Development Time:** 2-3 hours with AI assistance
