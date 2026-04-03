# Project Folder Structure

```
PharmaWorkflowSystem/
│
├── README.md                          # Project overview and quick start
├── SETUP_INSTRUCTIONS.md              # Detailed setup guide
├── API_TESTING.md                     # Postman testing examples
├── FOLDER_STRUCTURE.md                # This file
│
├── Database/                          # SQL Server database scripts
│   ├── 01_CreateDatabase.sql         # Database creation script
│   ├── 02_CreateTables.sql           # Table creation with constraints
│   └── 03_SeedData.sql               # Sample data insertion
│
├── Backend/                           # .NET 8 Web API
│   └── PharmaWorkflowAPI/
│       ├── PharmaWorkflowAPI.csproj  # Project file with dependencies
│       ├── appsettings.json          # Configuration (connection strings, JWT)
│       ├── Program.cs                # Application entry point and configuration
│       │
│       ├── Models/                   # Entity models
│       │   ├── User.cs              # User entity
│       │   ├── MainTransaction.cs   # Main transaction entity
│       │   └── HistoryTransaction.cs # History/audit entity
│       │
│       ├── Data/                     # Database context
│       │   └── ApplicationDbContext.cs # EF Core DbContext
│       │
│       ├── DTOs/                     # Data Transfer Objects
│       │   ├── LoginRequest.cs      # Authentication DTOs
│       │   └── TransactionDTOs.cs   # Transaction request/response DTOs
│       │
│       ├── Services/                 # Business logic layer
│       │   ├── IAuthService.cs      # Auth service interface
│       │   ├── AuthService.cs       # Authentication implementation
│       │   ├── ITransactionService.cs # Transaction service interface
│       │   └── TransactionService.cs  # Transaction business logic
│       │
│       └── Controllers/              # API endpoints
│           ├── AuthController.cs    # Login endpoint
│           └── TransactionsController.cs # CRUD + workflow endpoints
│
└── Frontend/                         # Angular 17 application
    └── pharma-workflow-app/
        ├── package.json             # NPM dependencies
        ├── tsconfig.json            # TypeScript configuration
        │
        └── src/
            ├── index.html           # Main HTML file
            ├── main.ts              # Application bootstrap
            ├── styles.css           # Global styles
            │
            └── app/
                ├── app.component.ts # Root component
                ├── app.routes.ts    # Route configuration
                │
                ├── models/          # TypeScript interfaces
                │   ├── auth.model.ts        # Auth models
                │   └── transaction.model.ts # Transaction models
                │
                ├── services/        # API communication services
                │   ├── auth.service.ts       # Authentication service
                │   └── transaction.service.ts # Transaction API service
                │
                ├── guards/          # Route guards
                │   └── auth.guard.ts # Authentication & role guards
                │
                ├── interceptors/    # HTTP interceptors
                │   └── auth.interceptor.ts # JWT token interceptor
                │
                └── components/      # UI components
                    ├── login/
                    │   ├── login.component.ts
                    │   ├── login.component.html
                    │   └── login.component.css
                    │
                    ├── dashboard/
                    │   ├── dashboard.component.ts
                    │   ├── dashboard.component.html
                    │   └── dashboard.component.css
                    │
                    ├── transactions-list/
                    │   ├── transactions-list.component.ts
                    │   ├── transactions-list.component.html
                    │   └── transactions-list.component.css
                    │
                    ├── create-transaction/
                    │   ├── create-transaction.component.ts
                    │   ├── create-transaction.component.html
                    │   └── create-transaction.component.css
                    │
                    ├── approve-transaction/
                    │   ├── approve-transaction.component.ts
                    │   ├── approve-transaction.component.html
                    │   └── approve-transaction.component.css
                    │
                    └── transaction-history/
                        ├── transaction-history.component.ts
                        ├── transaction-history.component.html
                        └── transaction-history.component.css
```

## Key Components Description

### Database Layer
- **01_CreateDatabase.sql**: Creates PharmaWorkflowDB database
- **02_CreateTables.sql**: Creates Users, Main_Transactions, History_Transactions tables with indexes
- **03_SeedData.sql**: Inserts sample users and transactions for testing

### Backend Architecture

#### Models
- Entity classes matching database tables
- Include validation attributes and relationships

#### Data
- ApplicationDbContext: EF Core context with entity configurations
- Handles database operations and migrations

#### DTOs
- Separate request/response objects for API
- Prevents over-posting and controls data exposure

#### Services
- Business logic separated from controllers
- Implements ACID transactions using TransactionScope
- Handles optimistic concurrency with RowVersion

#### Controllers
- RESTful API endpoints
- JWT authentication with role-based authorization
- Proper error handling and logging

### Frontend Architecture

#### Models
- TypeScript interfaces for type safety
- Matches backend DTOs

#### Services
- AuthService: Login, logout, token management
- TransactionService: All transaction CRUD operations
- Uses RxJS Observables for async operations

#### Guards
- authGuard: Protects routes requiring authentication
- roleGuard: Restricts access based on user role

#### Interceptors
- authInterceptor: Automatically adds JWT token to requests
- Handles 401 errors and redirects to login

#### Components
- **Login**: User authentication
- **Dashboard**: Overview with stats and workflow visualization
- **Transactions List**: View all requests with filters
- **Create Transaction**: Initiate new requests
- **Approve Transaction**: Manager/Admin approval workflow
- **Transaction History**: Audit trail timeline

## Design Patterns Used

### Backend
- **Repository Pattern**: Service layer abstracts data access
- **Dependency Injection**: Services injected via constructor
- **DTO Pattern**: Separate models for API contracts
- **Unit of Work**: TransactionScope for ACID compliance

### Frontend
- **Component-Based Architecture**: Reusable, standalone components
- **Service Pattern**: Centralized API communication
- **Guard Pattern**: Route protection
- **Interceptor Pattern**: Cross-cutting concerns (auth)
- **Observable Pattern**: Reactive programming with RxJS

## Technology Stack

### Backend
- .NET 8 Web API
- Entity Framework Core 8
- SQL Server
- JWT Bearer Authentication
- Swagger/OpenAPI

### Frontend
- Angular 17 (Standalone Components)
- TypeScript 5.2
- RxJS 7.8
- Angular Router
- HttpClient

### Database
- SQL Server 2019+
- ACID transactions
- Optimistic concurrency (RowVersion)
- Foreign key constraints
- Indexes for performance

## Security Features

1. **JWT Authentication**: Secure token-based auth
2. **Role-Based Access Control**: User, Manager, Admin roles
3. **Password Hashing**: (Demo uses simple validation - implement BCrypt for production)
4. **CORS Policy**: Configured for Angular origin
5. **HTTPS**: Enforced in production
6. **SQL Injection Prevention**: EF Core parameterized queries
7. **Optimistic Concurrency**: Prevents lost updates

## Scalability Considerations

1. **Stateless API**: JWT tokens enable horizontal scaling
2. **Database Indexes**: Optimized queries
3. **Async/Await**: Non-blocking operations
4. **Connection Pooling**: EF Core manages connections
5. **Lazy Loading**: Components loaded on demand (Angular)
6. **Caching**: Can add Redis for session/data caching

## Future Enhancements

1. File upload functionality (PDF reports)
2. Email notifications
3. Advanced reporting and analytics
4. Batch operations
5. Export to Excel/PDF
6. Real-time updates (SignalR)
7. Mobile app (Ionic/React Native)
8. Multi-tenant support
