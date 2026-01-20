# Mitt Telenor Demo - Complete Setup Guide

This guide will walk you through setting up the entire Telenor Self-Service Platform from scratch.

## 📋 Table of Contents
1. [Prerequisites Installation](#prerequisites-installation)
2. [Project Setup](#project-setup)
3. [Database Setup](#database-setup)
4. [Backend Services Setup](#backend-services-setup)
5. [Frontend Setup](#frontend-setup)
6. [Contentful CMS Setup](#contentful-cms-setup)
7. [Testing the Application](#testing-the-application)
8. [Troubleshooting](#troubleshooting)

## 1. Prerequisites Installation

### Windows

**Node.js 20+**
```powershell
# Download and install from https://nodejs.org/
# Or use winget:
winget install OpenJS.NodeJS.LTS
```

**pnpm**
```powershell
npm install -g pnpm
```

**.NET 8 SDK**
```powershell
# Download from https://dotnet.microsoft.com/download
# Or use winget:
winget install Microsoft.DotNet.SDK.8
```

**Docker Desktop**
```powershell
# Download from https://www.docker.com/products/docker-desktop
# Or use winget:
winget install Docker.DockerDesktop
```

**Git**
```powershell
winget install Git.Git
```

### Verify Installations
```powershell
node --version    # Should show v20.x.x or higher
pnpm --version    # Should show 8.x.x or higher
dotnet --version  # Should show 8.0.x
docker --version  # Should show Docker version 20.x.x or higher
git --version     # Should show git version 2.x.x
```

## 2. Project Setup

### Clone Repository
```powershell
git clone https://github.com/yourusername/mitt-telenor-demo.git
cd mitt-telenor-demo
```

### Start Docker Infrastructure
```powershell
# Start all infrastructure services
docker-compose up -d

# Verify all services are running
docker-compose ps

# You should see:
# - telenor-postgres (healthy)
# - telenor-redis (healthy)
# - telenor-localstack (healthy)
```

### Check Docker Logs (if issues)
```powershell
docker-compose logs postgres
docker-compose logs redis
docker-compose logs localstack
```

## 3. Database Setup

### Wait for PostgreSQL to be Ready
```powershell
# Test PostgreSQL connection
docker exec -it telenor-postgres psql -U telenor -d telenor_db -c "SELECT version();"
```

### Verify Database Schemas
```powershell
docker exec -it telenor-postgres psql -U telenor -d telenor_db -c "\dn"

# You should see schemas: auth, users, subscriptions, billing
```

## 4. Backend Services Setup

### Install .NET Tools
```powershell
dotnet tool install --global dotnet-ef
```

### Auth Service Setup
```powershell
cd backend/src/AuthService

# Restore dependencies
dotnet restore

# Create initial migration
dotnet ef migrations add InitialCreate

# Apply migration
dotnet ef database update

# Run the service
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5001
```

Keep this terminal open. Open a new terminal for the next service.

### User Service Setup
```powershell
cd backend/src/UserService

dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5002
```

### Subscription Service Setup
```powershell
cd backend/src/SubscriptionService

dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5003
```

### Usage Service Setup
```powershell
cd backend/src/UsageService

dotnet restore
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5004
```

### Billing Service Setup
```powershell
cd backend/src/BillingService

dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5005
```

### Verify All Services
Open your browser and check Swagger UI for each service:
- Auth Service: http://localhost:5001/swagger
- User Service: http://localhost:5002/swagger
- Subscription Service: http://localhost:5003/swagger
- Usage Service: http://localhost:5004/swagger
- Billing Service: http://localhost:5005/swagger

## 5. Frontend Setup

### Install Dependencies
```powershell
cd frontend

# Install all dependencies
pnpm install
```

### Configure Environment
```powershell
# Copy example env file
cp .env.example .env

# Edit .env file with your settings
notepad .env
```

**.env Configuration:**
```env
VITE_API_BASE_URL=http://localhost:5001/api
VITE_CONTENTFUL_SPACE_ID=your_space_id_here
VITE_CONTENTFUL_ACCESS_TOKEN=your_token_here
VITE_ENV=development
```

### Start Development Server
```powershell
pnpm dev
```

**Expected Output:**
```
VITE v5.0.11  ready in 1234 ms

➜  Local:   http://localhost:5173/
➜  Network: use --host to expose
➜  press h to show help
```

### Open Application
Navigate to http://localhost:5173 in your browser.

## 6. Contentful CMS Setup

### Create Contentful Account
1. Go to https://www.contentful.com/
2. Sign up for a free account
3. Create a new space (e.g., "Telenor Demo")

### Create Content Models

#### Promotional Banner
1. Go to Content model → Add content type
2. Name: "Promotional Banner"
3. Add fields:
   - `title` (Short text, required)
   - `description` (Long text)
   - `imageUrl` (Media)
   - `ctaText` (Short text)
   - `ctaLink` (Short text)
   - `isActive` (Boolean)
   - `priority` (Number)
   - `startDate` (Date and time)
   - `endDate` (Date and time)

#### FAQ Item
1. Add content type: "FAQ Item"
2. Add fields:
   - `question` (Short text, required)
   - `answer` (Rich text, required)
   - `category` (Short text - Billing, Usage, Technical, Account)
   - `order` (Number)

#### Service Plan
1. Add content type: "Service Plan"
2. Add fields:
   - `name` (Short text, required)
   - `description` (Rich text)
   - `price` (Number, required)
   - `dataAllowance` (Short text)
   - `voiceMinutes` (Short text)
   - `smsCount` (Short text)
   - `features` (Short text, list)
   - `isPopular` (Boolean)

### Get API Credentials
1. Go to Settings → API keys
2. Click "Add API key"
3. Copy:
   - Space ID
   - Content Delivery API - access token

### Update Frontend .env
```env
VITE_CONTENTFUL_SPACE_ID=your_actual_space_id
VITE_CONTENTFUL_ACCESS_TOKEN=your_actual_access_token
```

### Create Sample Content
1. Go to Content
2. Add entries for each content type
3. Publish the entries

## 7. Testing the Application

### Test User Registration
1. Open http://localhost:5173
2. Click "Register"
3. Fill in the form:
   - Email: test@example.com
   - Password: Test123!@
   - First Name: Test
   - Last Name: User
   - Phone: +4712345678
4. Click "Register"
5. You should be redirected to the dashboard

### Test API Endpoints
```powershell
# Test Auth Service
curl -X POST http://localhost:5001/api/auth/register `
  -H "Content-Type: application/json" `
  -d '{
    "email": "demo@telenor.com",
    "password": "Demo123!@",
    "firstName": "Demo",
    "lastName": "User",
    "phone": "+4712345678"
  }'

# Test Login
curl -X POST http://localhost:5001/api/auth/login `
  -H "Content-Type: application/json" `
  -d '{
    "email": "demo@telenor.com",
    "password": "Demo123!@"
  }'
```

### Run Tests
```powershell
# Frontend unit tests
cd frontend
pnpm test:unit

# Backend tests
cd backend
dotnet test
```

## 8. Troubleshooting

### Docker Issues

**Problem: Docker containers not starting**
```powershell
# Stop all containers
docker-compose down

# Remove volumes
docker-compose down -v

# Rebuild and start
docker-compose up -d --build
```

**Problem: Port already in use**
```powershell
# Check what's using the port
netstat -ano | findstr :5432
netstat -ano | findstr :6379

# Kill the process or change ports in docker-compose.yml
```

### Database Issues

**Problem: Migration fails**
```powershell
# Drop and recreate database
docker exec -it telenor-postgres psql -U telenor -c "DROP DATABASE telenor_db;"
docker exec -it telenor-postgres psql -U telenor -c "CREATE DATABASE telenor_db;"

# Re-run migrations
cd backend/src/AuthService
dotnet ef database update
```

**Problem: Connection refused**
```powershell
# Check if PostgreSQL is running
docker-compose ps postgres

# Check logs
docker-compose logs postgres

# Restart PostgreSQL
docker-compose restart postgres
```

### Frontend Issues

**Problem: Module not found errors**
```powershell
# Clear node_modules and reinstall
cd frontend
Remove-Item -Recurse -Force node_modules
Remove-Item pnpm-lock.yaml
pnpm install
```

**Problem: Vite not starting**
```powershell
# Check if port 5173 is available
netstat -ano | findstr :5173

# Use different port
pnpm dev --port 3000
```

### Backend Issues

**Problem: Service won't start**
```powershell
# Clean and rebuild
dotnet clean
dotnet build

# Check for errors
dotnet run --verbosity detailed
```

**Problem: JWT errors**
- Verify JWT secret is at least 256 bits (32 characters)
- Check appsettings.json configuration
- Ensure Issuer and Audience match

### LocalStack Issues

**Problem: AWS services not available**
```powershell
# Check LocalStack health
curl http://localhost:4566/_localstack/health

# Restart LocalStack
docker-compose restart localstack

# Re-run initialization script
docker exec -it telenor-localstack sh /etc/localstack/init/ready.d/init.sh
```

## 🎉 Success!

If all services are running:
- ✅ Docker containers: postgres, redis, localstack
- ✅ Backend services: 5 microservices on ports 5001-5005
- ✅ Frontend: Running on http://localhost:5173
- ✅ Database: Migrations applied
- ✅ Contentful: Content models created

You're ready to develop!

## 📚 Next Steps

1. Read the [API Documentation](docs/api-reference.md)
2. Explore the [Architecture](docs/architecture.md)
3. Check out [Development Guide](docs/development-guide.md)
4. Review [Contributing Guidelines](CONTRIBUTING.md)

## 🆘 Need Help?

- Check the [FAQ](docs/faq.md)
- Open an issue on GitHub
- Contact: your.email@example.com
