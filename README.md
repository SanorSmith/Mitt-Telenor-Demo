# Mitt Telenor Demo - Progressive Web App

> A full-stack telecom customer self-service platform demonstrating modern web development practices

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Vue](https://img.shields.io/badge/Vue-3.4-brightgreen.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![TypeScript](https://img.shields.io/badge/TypeScript-5.3-blue.svg)

## 📋 Table of Contents
- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Development](#development)
- [Testing](#testing)
- [Deployment](#deployment)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)

## 🎯 Overview

Mitt Telenor Demo is a production-ready Progressive Web App built to demonstrate expertise in modern full-stack development. This project showcases:

- **Frontend Excellence**: Vue 3 + TypeScript + Capacitor for cross-platform PWA
- **Backend Mastery**: .NET 8 microservices with event-driven architecture
- **Cloud Native**: AWS infrastructure with serverless components
- **CMS Integration**: Contentful headless CMS for dynamic content
- **DevOps Best Practices**: Full CI/CD pipeline, containerization, IaC

**Purpose**: Portfolio project for Telenor Front-end Developer position

## ✨ Features

### User Features
- 🔐 Secure authentication with JWT
- 📊 Real-time usage tracking (data, voice, SMS)
- 💳 Subscription management and billing
- 📱 Native mobile capabilities (camera, notifications)
- 🌐 Offline-first PWA architecture
- ♿ WCAG 2.1 AA accessibility compliant
- 📈 Interactive usage charts and analytics
- 📄 Invoice generation and PDF download
- 🔔 Push notifications for usage alerts

### Technical Features
- 🚀 Progressive Web App with service worker
- 📦 Code splitting and lazy loading
- 🎨 Tailwind CSS with responsive design
- 🔄 Real-time updates via event-driven architecture
- 📈 Performance monitoring
- 🛡️ Enterprise-grade security
- 🐳 Docker containerization
- ☁️ AWS cloud-native deployment

## 🛠️ Tech Stack

### Frontend
- **Framework**: Vue 3.4 + Composition API
- **Language**: TypeScript 5.3
- **Build Tool**: Vite 5
- **State Management**: Pinia 2.1
- **Routing**: Vue Router 4
- **Styling**: Tailwind CSS 3.4
- **PWA**: Vite PWA Plugin
- **Mobile**: Capacitor 5
- **Charts**: Chart.js + vue-chartjs
- **HTTP Client**: Axios
- **Form Validation**: VeeValidate + Yup
- **Icons**: Lucide Vue Next

### Backend
- **Framework**: .NET 8 (ASP.NET Core Web API)
- **Language**: C# 12
- **ORM**: Entity Framework Core 8
- **Database**: PostgreSQL 16
- **NoSQL**: DynamoDB
- **Cache**: Redis 7
- **Authentication**: JWT Bearer tokens
- **Logging**: Serilog
- **API Docs**: Swagger/OpenAPI

### CMS
- **Platform**: Contentful
- **SDK**: Contentful JavaScript SDK

### AWS Infrastructure
- **Compute**: ECS Fargate / Lambda
- **Storage**: S3 + CloudFront
- **Database**: RDS PostgreSQL, DynamoDB
- **Cache**: ElastiCache Redis
- **Auth**: Cognito User Pools
- **API**: API Gateway
- **Messaging**: SNS/SQS
- **Monitoring**: CloudWatch
- **Secrets**: Secrets Manager

### DevOps
- **Containers**: Docker + Docker Compose
- **CI/CD**: GitHub Actions
- **IaC**: AWS CDK (TypeScript)
- **Local AWS**: LocalStack

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────┐
│                   CloudFront CDN                     │
│              (S3 + PWA Frontend)                     │
└────────────────┬────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────┐
│               API Gateway                            │
└─────┬──────┬──────┬──────┬──────┬───────────────────┘
      │      │      │      │      │
      ▼      ▼      ▼      ▼      ▼
   ┌────┐ ┌────┐ ┌────┐ ┌────┐ ┌────┐
   │Auth│ │User│ │Sub │ │Usg │ │Bill│  (Microservices)
   └─┬──┘ └─┬──┘ └─┬──┘ └─┬──┘ └─┬──┘
     │      │      │      │      │
     ▼      ▼      ▼      ▼      ▼
   ┌──────────────────────────────┐
   │    RDS PostgreSQL + Redis    │
   └──────────────────────────────┘
     
   ┌──────────────────────────────┐
   │       DynamoDB (Usage)        │
   └──────────────────────────────┘
```

### Microservices
1. **Auth Service** (Port 5001) - Authentication & JWT management
2. **User Service** (Port 5002) - User profiles & S3 uploads
3. **Subscription Service** (Port 5003) - Subscription management
4. **Usage Service** (Port 5004) - Usage tracking with DynamoDB
5. **Billing Service** (Port 5005) - Invoices & payments

## 🚀 Getting Started

### Prerequisites

- **Node.js** 20+ ([Download](https://nodejs.org/))
- **pnpm** 8+ (`npm install -g pnpm`)
- **.NET 8 SDK** ([Download](https://dotnet.microsoft.com/download))
- **Docker Desktop** ([Download](https://www.docker.com/products/docker-desktop))
- **Git** ([Download](https://git-scm.com/))

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/mitt-telenor-demo.git
cd mitt-telenor-demo
```

2. **Start infrastructure (Docker)**
```bash
docker-compose up -d
```

Wait for all services to be healthy:
```bash
docker-compose ps
```

3. **Setup Contentful** (Optional for CMS features)
   - Create account at [contentful.com](https://www.contentful.com/)
   - Create a new space
   - Copy Space ID and Access Token
   - See `docs/contentful-setup.md` for content model setup

4. **Configure environment variables**

**Frontend** (`frontend/.env`):
```env
VITE_API_BASE_URL=http://localhost:5001/api
VITE_CONTENTFUL_SPACE_ID=your_space_id
VITE_CONTENTFUL_ACCESS_TOKEN=your_access_token
```

**Backend** - Already configured in `appsettings.json` for local development

5. **Install frontend dependencies**
```bash
cd frontend
pnpm install
```

6. **Run database migrations**
```bash
cd backend/src/AuthService
dotnet ef database update

cd ../UserService
dotnet ef database update

cd ../SubscriptionService
dotnet ef database update

cd ../BillingService
dotnet ef database update
```

7. **Start backend services**

Open 5 terminal windows:

```bash
# Terminal 1 - Auth Service
cd backend/src/AuthService
dotnet run

# Terminal 2 - User Service
cd backend/src/UserService
dotnet run

# Terminal 3 - Subscription Service
cd backend/src/SubscriptionService
dotnet run

# Terminal 4 - Usage Service
cd backend/src/UsageService
dotnet run

# Terminal 5 - Billing Service
cd backend/src/BillingService
dotnet run
```

8. **Start frontend**
```bash
cd frontend
pnpm dev
```

9. **Access the application**
   - Frontend: http://localhost:5173
   - Auth Service Swagger: http://localhost:5001/swagger
   - User Service Swagger: http://localhost:5002/swagger

### Test Credentials
```
Email: demo@telenor.com
Password: Demo123!@
```

## 💻 Development

### Project Structure
```
mitt-telenor-demo/
├── frontend/              # Vue 3 PWA
│   ├── src/
│   │   ├── components/   # Reusable components
│   │   ├── views/        # Page components
│   │   ├── stores/       # Pinia stores
│   │   ├── services/     # API services
│   │   ├── composables/  # Vue composables
│   │   ├── types/        # TypeScript types
│   │   └── router/       # Vue Router config
│   └── public/           # Static assets
├── backend/              # .NET 8 Microservices
│   └── src/
│       ├── AuthService/
│       ├── UserService/
│       ├── SubscriptionService/
│       ├── UsageService/
│       ├── BillingService/
│       └── Shared/       # Common utilities
├── infrastructure/       # AWS CDK (coming soon)
├── docs/                 # Documentation
├── scripts/              # Setup scripts
└── docker-compose.yml    # Local infrastructure
```

### Available Scripts

**Frontend**
```bash
pnpm dev          # Start dev server
pnpm build        # Production build
pnpm preview      # Preview production build
pnpm test:unit    # Run unit tests
pnpm test:e2e     # Run E2E tests
pnpm lint         # Lint code
pnpm format       # Format code
pnpm type-check   # TypeScript check
```

**Backend**
```bash
dotnet run                    # Start service
dotnet test                   # Run tests
dotnet ef migrations add      # Create migration
dotnet ef database update     # Apply migrations
```

### Code Style

- **Frontend**: ESLint + Prettier (auto-format on save)
- **Backend**: .editorconfig + StyleCop
- **Commits**: Conventional Commits format

### Environment Variables

**Frontend** (`.env`):
- `VITE_API_BASE_URL` - Backend API URL
- `VITE_CONTENTFUL_SPACE_ID` - Contentful space ID
- `VITE_CONTENTFUL_ACCESS_TOKEN` - Contentful access token

**Backend** (`appsettings.json`):
- `ConnectionStrings:DefaultConnection` - PostgreSQL connection
- `Jwt:Secret` - JWT signing secret
- `Redis:ConnectionString` - Redis connection
- `AWS:*` - AWS service configurations

## 🧪 Testing

### Unit Tests
```bash
# Frontend
cd frontend
pnpm test:unit

# Backend
cd backend
dotnet test --filter Category=Unit
```

### Integration Tests
```bash
cd backend
dotnet test --filter Category=Integration
```

### E2E Tests
```bash
cd frontend
pnpm test:e2e
```

### Test Coverage
```bash
# Frontend
pnpm test:coverage

# Backend
dotnet test --collect:"XPlat Code Coverage"
```

**Coverage Targets**: 80%+ for all services

## 📈 Performance Metrics

### Target Lighthouse Scores
- **Performance**: 90+
- **Accessibility**: 95+
- **Best Practices**: 95+
- **SEO**: 100
- **PWA**: 100

### Performance Targets
- First Contentful Paint (FCP): < 1.8s
- Largest Contentful Paint (LCP): < 2.5s
- Time to Interactive (TTI): < 3.8s
- Cumulative Layout Shift (CLS): < 0.1

## 🚢 Deployment

### AWS Deployment (Coming Soon)

1. **Configure AWS credentials**
```bash
aws configure
```

2. **Deploy infrastructure**
```bash
cd infrastructure
npm install
cdk deploy --all
```

3. **Deploy application**
```bash
# Automated via GitHub Actions on push to main
# Or manually:
./scripts/deploy.sh
```

### Manual Deployment
See `docs/deployment.md` for detailed instructions

## 📖 API Documentation

- **Swagger UI**: http://localhost:5001/swagger (when running locally)
- **API Reference**: See `docs/api-reference.md`

### Key Endpoints

**Auth Service**
```
POST   /api/auth/register    - Register new user
POST   /api/auth/login       - Login user
POST   /api/auth/refresh     - Refresh access token
POST   /api/auth/logout      - Logout user
```

**Subscription Service**
```
GET    /api/subscriptions              - Get user subscriptions
PUT    /api/subscriptions/{id}/change-plan - Change subscription plan
POST   /api/subscriptions/{id}/add-ons     - Add subscription add-on
```

**Usage Service**
```
GET    /api/usage/current    - Get current usage
GET    /api/usage/history    - Get usage history
POST   /api/usage/alerts     - Set usage alert
```

**Billing Service**
```
GET    /api/billing/invoices        - List invoices
GET    /api/billing/invoices/{id}/pdf - Download invoice PDF
GET    /api/billing/payments        - Payment history
```

## 🔒 Security

- JWT with refresh tokens (15min access, 7 day refresh)
- Password hashing (BCrypt, work factor 12)
- HTTPS/TLS encryption
- CORS configuration
- Rate limiting
- Input validation
- SQL injection prevention (parameterized queries)
- XSS protection
- Security headers (CSP, HSTS, etc.)

See `SECURITY.md` for vulnerability reporting

## 📊 Monitoring

- **Application**: AWS CloudWatch
- **Logs**: Serilog + CloudWatch Logs
- **Metrics**: Custom CloudWatch metrics
- **Errors**: Structured logging

## 🤝 Contributing

Contributions welcome! Please read `CONTRIBUTING.md` first.

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open Pull Request

## 📝 License

This project is licensed under the MIT License - see `LICENSE` file.

## 👤 Author

**Your Name**
- Portfolio: [yourwebsite.com](https://yourwebsite.com)
- LinkedIn: [linkedin.com/in/yourprofile](https://linkedin.com/in/yourprofile)
- Email: your.email@example.com

## 🙏 Acknowledgments

- Telenor for job opportunity inspiration
- Vue.js community
- .NET community
- AWS documentation

## 📸 Screenshots

### Dashboard
![Dashboard](docs/images/dashboard.png)

### Mobile View
![Mobile](docs/images/mobile.png)

### Usage Tracking
![Usage](docs/images/usage.png)

---

**Built with ❤️ for Telenor**
