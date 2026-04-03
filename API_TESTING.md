# API Testing Guide - Postman Examples

## Base URL
```
https://localhost:7001/api
```

## 1. Authentication

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin@123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "role": "Admin",
  "fullName": "System Administrator",
  "expiresAt": "2026-04-03T08:00:00Z"
}
```

**Save the token** - You'll need it for all subsequent requests.

## 2. Transactions API

### 2.1 Create Transaction

```http
POST /api/transactions
Authorization: Bearer YOUR_TOKEN_HERE
Content-Type: application/json

{
  "drugName": "Paracetamol 500mg",
  "batchNo": "PCM2026B10",
  "requestedBy": "Lab Technician",
  "comments": "New batch for Q2 2026"
}
```

**Response:**
```json
{
  "id": 6,
  "drugName": "Paracetamol 500mg",
  "batchNo": "PCM2026B10",
  "requestedBy": "Lab Technician",
  "status": "Initiated",
  "createdDate": "2026-04-02T10:30:00",
  "updatedDate": "2026-04-02T10:30:00",
  "comments": "New batch for Q2 2026",
  "rowVersion": [0, 0, 0, 0, 0, 0, 0, 1]
}
```

### 2.2 Get All Transactions

```http
GET /api/transactions
Authorization: Bearer YOUR_TOKEN_HERE
```

**With filters:**
```http
GET /api/transactions?status=Approved&search=Paracetamol
Authorization: Bearer YOUR_TOKEN_HERE
```

### 2.3 Get Single Transaction

```http
GET /api/transactions/1
Authorization: Bearer YOUR_TOKEN_HERE
```

### 2.4 Approve Transaction

```http
PUT /api/transactions/1/approve
Authorization: Bearer YOUR_TOKEN_HERE
Content-Type: application/json

{
  "actionBy": "QA Manager",
  "comments": "Quality standards met. Approved for production.",
  "rowVersion": [0, 0, 0, 0, 0, 0, 0, 1]
}
```

**Note:** Only Manager and Admin roles can approve.

### 2.5 Reject Transaction

```http
PUT /api/transactions/2/reject
Authorization: Bearer YOUR_TOKEN_HERE
Content-Type: application/json

{
  "actionBy": "QA Manager",
  "comments": "Quality parameters not met. Rejected.",
  "rowVersion": [0, 0, 0, 0, 0, 0, 0, 1]
}
```

### 2.6 Modify Transaction

```http
PUT /api/transactions/1/modify
Authorization: Bearer YOUR_TOKEN_HERE
Content-Type: application/json

{
  "drugName": "Paracetamol 650mg",
  "batchNo": "PCM2026B01",
  "comments": "Updated dosage information",
  "rowVersion": [0, 0, 0, 0, 0, 0, 0, 2]
}
```

### 2.7 Get Transaction History

```http
GET /api/transactions/1/history
Authorization: Bearer YOUR_TOKEN_HERE
```

**Response:**
```json
[
  {
    "historyId": 1,
    "transactionId": 1,
    "action": "Created",
    "actionBy": "Lab Technician",
    "comments": "Request initiated",
    "actionDate": "2026-04-01T09:00:00",
    "previousStatus": null,
    "newStatus": "Initiated"
  },
  {
    "historyId": 2,
    "transactionId": 1,
    "action": "Approved",
    "actionBy": "QA Manager",
    "comments": "Quality standards met",
    "actionDate": "2026-04-01T14:30:00",
    "previousStatus": "Initiated",
    "newStatus": "Approved"
  }
]
```

### 2.8 Delete Transaction (Soft Delete)

```http
DELETE /api/transactions/5
Authorization: Bearer YOUR_TOKEN_HERE
```

**Note:** Only Admin role can delete.

### 2.9 Get Dashboard Stats

```http
GET /api/transactions/dashboard/stats
Authorization: Bearer YOUR_TOKEN_HERE
```

**Response:**
```json
{
  "totalRequests": 10,
  "approved": 4,
  "pending": 3,
  "rejected": 2,
  "active": 1,
  "initiated": 2
}
```

## 3. Postman Collection Setup

### Step 1: Create Environment

1. Click "Environments" in Postman
2. Create new environment "Pharma Workflow"
3. Add variables:
   - `baseUrl`: `https://localhost:7001/api`
   - `token`: (leave empty, will be set after login)

### Step 2: Set Authorization

1. Create a new request
2. Go to "Authorization" tab
3. Type: Bearer Token
4. Token: `{{token}}`

### Step 3: Login Script

Add this to the "Tests" tab of your login request:

```javascript
if (pm.response.code === 200) {
    var jsonData = pm.response.json();
    pm.environment.set("token", jsonData.token);
    console.log("Token saved:", jsonData.token);
}
```

## 4. Testing Scenarios

### Scenario 1: Complete Workflow

1. **Login as User**
   ```
   POST /api/auth/login
   Body: { "username": "user", "password": "User@123" }
   ```

2. **Create Request**
   ```
   POST /api/transactions
   Body: { "drugName": "Aspirin 100mg", "batchNo": "ASP2026B20", ... }
   ```

3. **Login as Manager**
   ```
   POST /api/auth/login
   Body: { "username": "manager", "password": "Manager@123" }
   ```

4. **Approve Request**
   ```
   PUT /api/transactions/{id}/approve
   Body: { "actionBy": "QA Manager", "comments": "Approved" }
   ```

5. **View History**
   ```
   GET /api/transactions/{id}/history
   ```

### Scenario 2: Concurrency Test

1. Get transaction with rowVersion
2. Update transaction in another session
3. Try to update with old rowVersion
4. Should receive 409 Conflict error

### Scenario 3: Role-Based Access

1. Login as User
2. Try to approve transaction
3. Should receive 403 Forbidden error

## 5. Common Error Responses

### 401 Unauthorized
```json
{
  "message": "Invalid username or password"
}
```

### 403 Forbidden
```json
{
  "message": "Access denied"
}
```

### 404 Not Found
```json
{
  "message": "Transaction not found"
}
```

### 409 Conflict (Concurrency)
```json
{
  "message": "Transaction was modified by another user. Please refresh and try again."
}
```

### 500 Internal Server Error
```json
{
  "message": "An error occurred",
  "error": "Detailed error message"
}
```

## 6. Sample Data for Testing

### Drug Names
- Paracetamol 500mg
- Amoxicillin 250mg
- Ibuprofen 400mg
- Aspirin 75mg
- Metformin 500mg
- Omeprazole 20mg
- Atorvastatin 10mg

### Batch Number Format
- PCM2026B01 (Paracetamol)
- AMX2026B02 (Amoxicillin)
- IBU2026B03 (Ibuprofen)
- ASP2026B04 (Aspirin)
- MET2026B05 (Metformin)

## 7. Performance Testing

Use these endpoints for load testing:

1. **Read-heavy**: GET /api/transactions
2. **Write-heavy**: POST /api/transactions
3. **Complex**: GET /api/transactions/{id}/history

Recommended tools:
- Apache JMeter
- k6
- Artillery
