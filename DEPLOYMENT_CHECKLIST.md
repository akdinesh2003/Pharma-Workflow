# Production Deployment Checklist

## Pre-Deployment

### Security
- [ ] Replace demo password verification with BCrypt/Argon2
- [ ] Move JWT secret to environment variables
- [ ] Configure HTTPS certificates
- [ ] Enable HSTS (HTTP Strict Transport Security)
- [ ] Configure Content Security Policy headers
- [ ] Enable CORS only for production domains
- [ ] Implement rate limiting on API endpoints
- [ ] Add API key authentication for service-to-service calls
- [ ] Enable SQL Server encryption at rest
- [ ] Configure firewall rules

### Database
- [ ] Backup development database
- [ ] Create production database
- [ ] Run migration scripts
- [ ] Set up automated backups
- [ ] Configure backup retention policy
- [ ] Test restore procedure
- [ ] Set up database monitoring
- [ ] Configure connection pooling
- [ ] Optimize indexes
- [ ] Set up read replicas (if needed)

### Backend
- [ ] Update appsettings.Production.json
- [ ] Configure production connection string
- [ ] Set up environment variables
- [ ] Enable detailed error logging
- [ ] Configure log aggregation (e.g., Serilog, ELK)
- [ ] Set up health check endpoints
- [ ] Configure application insights
- [ ] Build in Release mode
- [ ] Run security scan
- [ ] Test all API endpoints

### Frontend
- [ ] Update API URLs to production
- [ ] Build with production flag
- [ ] Minify and optimize assets
- [ ] Enable lazy loading
- [ ] Configure CDN for static assets
- [ ] Set up error tracking (e.g., Sentry)
- [ ] Test on all target browsers
- [ ] Optimize bundle size
- [ ] Enable service worker (PWA)
- [ ] Configure analytics

## Deployment Steps

### Database Deployment
```sql
-- 1. Create production database
CREATE DATABASE PharmaWorkflowDB_Prod;

-- 2. Run table creation scripts
-- Execute 02_CreateTables.sql

-- 3. Create production users (NOT demo users)
-- Use proper password hashing

-- 4. Set up maintenance plans
-- Configure backup jobs
-- Configure index maintenance
```

### Backend Deployment

#### Option 1: IIS Deployment
```bash
# Build
dotnet publish -c Release -o ./publish

# Deploy to IIS
# 1. Create application pool (.NET CLR Version: No Managed Code)
# 2. Create website pointing to publish folder
# 3. Configure bindings (HTTPS)
# 4. Set environment variables in IIS
```

#### Option 2: Azure App Service
```bash
# Using Azure CLI
az webapp up --name pharma-workflow-api --resource-group pharma-rg

# Or using Visual Studio
# Right-click project → Publish → Azure App Service
```

#### Option 3: Docker
```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY ./publish .
ENTRYPOINT ["dotnet", "PharmaWorkflowAPI.dll"]
```

```bash
docker build -t pharma-workflow-api .
docker run -d -p 443:443 pharma-workflow-api
```

### Frontend Deployment

#### Build for Production
```bash
cd Frontend/pharma-workflow-app
npm run build
# Output in dist/pharma-workflow-app
```

#### Option 1: IIS Deployment
```
1. Copy dist/pharma-workflow-app to IIS wwwroot
2. Create web.config for URL rewriting
3. Configure HTTPS binding
4. Test application
```

#### Option 2: Azure Static Web Apps
```bash
az staticwebapp create \
  --name pharma-workflow-app \
  --resource-group pharma-rg \
  --source dist/pharma-workflow-app
```

#### Option 3: Nginx
```nginx
server {
    listen 443 ssl;
    server_name pharma.example.com;
    
    root /var/www/pharma-workflow-app;
    index index.html;
    
    location / {
        try_files $uri $uri/ /index.html;
    }
    
    location /api {
        proxy_pass https://api.pharma.example.com;
    }
}
```

## Post-Deployment

### Verification
- [ ] Test login with all roles
- [ ] Create test transaction
- [ ] Approve/reject workflow
- [ ] View history
- [ ] Test search and filters
- [ ] Verify dashboard statistics
- [ ] Check API response times
- [ ] Verify HTTPS is enforced
- [ ] Test error handling
- [ ] Verify logging is working

### Monitoring Setup
- [ ] Configure application monitoring
- [ ] Set up performance monitoring
- [ ] Configure error alerting
- [ ] Set up uptime monitoring
- [ ] Configure database monitoring
- [ ] Set up log aggregation
- [ ] Create monitoring dashboard
- [ ] Configure alert thresholds
- [ ] Test alert notifications

### Documentation
- [ ] Update API documentation
- [ ] Document deployment process
- [ ] Create runbook for common issues
- [ ] Document backup/restore procedures
- [ ] Create user manual
- [ ] Document admin procedures
- [ ] Update architecture diagrams

### Performance Optimization
- [ ] Enable response compression
- [ ] Configure caching headers
- [ ] Set up Redis cache (if needed)
- [ ] Enable CDN for static assets
- [ ] Optimize database queries
- [ ] Configure connection pooling
- [ ] Enable output caching
- [ ] Implement lazy loading

### Security Hardening
- [ ] Run security scan (OWASP ZAP)
- [ ] Perform penetration testing
- [ ] Review and update CORS policy
- [ ] Configure CSP headers
- [ ] Enable request validation
- [ ] Set up WAF rules
- [ ] Configure DDoS protection
- [ ] Review and update firewall rules

## Production Configuration Files

### appsettings.Production.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-sql-server;Database=PharmaWorkflowDB;User Id=pharma_app;Password=***;Encrypt=True;TrustServerCertificate=False"
  },
  "JwtSettings": {
    "SecretKey": "*** FROM ENVIRONMENT VARIABLE ***",
    "Issuer": "https://api.pharma.example.com",
    "Audience": "https://pharma.example.com",
    "ExpiryMinutes": 480
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "pharma.example.com"
}
```

### web.config (IIS - Frontend)
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <rule name="Angular Routes" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
          </conditions>
          <action type="Rewrite" url="/" />
        </rule>
      </rules>
    </rewrite>
    <staticContent>
      <mimeMap fileExtension=".json" mimeType="application/json" />
    </staticContent>
  </system.webServer>
</configuration>
```

## Rollback Plan

### If Deployment Fails

#### Backend Rollback
```bash
# Stop new version
# Restore previous version
# Verify functionality
# Investigate issues
```

#### Database Rollback
```sql
-- Restore from backup
RESTORE DATABASE PharmaWorkflowDB
FROM DISK = 'C:\Backups\PharmaWorkflowDB_PreDeployment.bak'
WITH REPLACE;
```

#### Frontend Rollback
```bash
# Deploy previous version from backup
# Clear CDN cache
# Verify functionality
```

## Maintenance

### Daily
- [ ] Check error logs
- [ ] Monitor performance metrics
- [ ] Review security alerts

### Weekly
- [ ] Review application logs
- [ ] Check database performance
- [ ] Review user feedback
- [ ] Update documentation

### Monthly
- [ ] Security updates
- [ ] Performance review
- [ ] Backup verification
- [ ] Capacity planning
- [ ] Update dependencies

### Quarterly
- [ ] Security audit
- [ ] Performance optimization
- [ ] Disaster recovery test
- [ ] Review and update documentation

## Support Contacts

### Technical Support
- Database Admin: [email]
- DevOps Team: [email]
- Security Team: [email]

### Escalation
- Level 1: Application Support
- Level 2: Development Team
- Level 3: Architecture Team

## Emergency Procedures

### System Down
1. Check health endpoints
2. Review error logs
3. Check database connectivity
4. Verify external dependencies
5. Escalate if needed

### Data Breach
1. Isolate affected systems
2. Notify security team
3. Preserve evidence
4. Follow incident response plan
5. Notify stakeholders

### Performance Issues
1. Check application metrics
2. Review database performance
3. Check external dependencies
4. Scale resources if needed
5. Investigate root cause

## Success Criteria

- [ ] All users can login successfully
- [ ] All workflows function correctly
- [ ] API response time < 200ms
- [ ] Page load time < 2s
- [ ] Zero critical errors in logs
- [ ] Backup and restore tested
- [ ] Monitoring and alerts working
- [ ] Documentation complete
- [ ] Team trained on new system

---

**Deployment Complete! System is production-ready! 🚀**
