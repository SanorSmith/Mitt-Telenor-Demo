# Mitt Telenor Demo - Progressive Web App

> A full-stack telecom customer self-service platform demonstrating modern web development practices

## 🌐 Live Demo

**🔒 Production (HTTPS - Recommended):**  
**https://dhhho0vm7geyy.cloudfront.net**

**Alternative (HTTP):**  
http://mitt-telenor-demo-48625.s3-website.eu-north-1.amazonaws.com

**Test Credentials:**
- Email: `ansorsmith83@gmail.com`
- Password: `PP@ssw0rd`

---

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Vue](https://img.shields.io/badge/Vue-3.4-brightgreen.svg)
![Supabase](https://img.shields.io/badge/Supabase-Backend-green.svg)
![TypeScript](https://img.shields.io/badge/TypeScript-5.3-blue.svg)
![AWS](https://img.shields.io/badge/AWS-Deployed-orange.svg)

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

Mitt Telenor Demo is a production-ready Progressive Web App demonstrating expertise in modern full-stack development with a focus on frontend excellence and cloud-native deployment. This project showcases:

- **Frontend Excellence**: Vue 3 + TypeScript + Composition API with modern patterns
- **Headless CMS Integration**: Contentful for dynamic content management
- **Cloud Native Deployment**: AWS S3 + CloudFront with automated deployment
- **Modern Backend**: Supabase BaaS with PostgreSQL and real-time features
- **Progressive Web App**: Service worker, offline capabilities, mobile-ready
- **DevOps Mindset**: Automated deployment, infrastructure management, monitoring

**Purpose**: Portfolio project demonstrating skills for Telenor Front-end Developer position

## ✨ Features

### User Features
- 🔐 Secure authentication with Supabase Auth & JWT
- 👤 Complete profile management with photo upload
- 📊 Usage tracking and analytics visualization
- 💳 Subscription management and billing overview
- 📄 Invoice history and payment methods
- 📱 Progressive Web App with mobile capabilities
- 🌐 Offline-first architecture with service worker
- 📝 Dynamic FAQ content from Contentful CMS
- 🎨 Responsive design for all screen sizes
- ♿ Accessibility with semantic HTML & ARIA

### Technical Features
- 🚀 Vue 3 Composition API with TypeScript
- 📦 Code splitting and lazy loading routes
- 🎨 Tailwind CSS with modern design system
- 🔄 Real-time database subscriptions
- 📈 Chart.js for data visualization
- 🛡️ Row Level Security (RLS) policies
- ☁️ AWS S3 + CloudFront deployment
- 📱 PWA with service worker & manifest
- 🔍 SEO optimization and meta tags
- 🧪 Comprehensive testing with Playwright

## 🛠️ Tech Stack

### Frontend
- **Framework**: Vue 3.4 + Composition API
- **Language**: TypeScript 5.3
- **Build Tool**: Vite 5
- **State Management**: Pinia 2.1
- **Routing**: Vue Router 4
- **Styling**: Tailwind CSS 3.4
- **PWA**: Vite PWA Plugin + Service Worker
- **Mobile**: Capacitor 5 (iOS/Android)
- **Charts**: Chart.js + vue-chartjs
- **HTTP Client**: Axios
- **Form Validation**: VeeValidate + Yup
- **Icons**: Lucide Vue Next
- **Date Utils**: date-fns
- **Testing**: Vitest + Playwright
- **Linting**: ESLint + Prettier

### Backend
- **Platform**: Supabase (Backend-as-a-Service)
- **Database**: PostgreSQL (Supabase)
- **Authentication**: Supabase Auth with JWT
- **Storage**: Supabase Storage (Profile images)
- **Real-time**: Supabase Realtime subscriptions
- **API**: Auto-generated REST API
- **Row Level Security**: PostgreSQL RLS policies

### CMS
- **Platform**: Contentful
- **SDK**: Contentful JavaScript SDK

### AWS Infrastructure
- **Static Hosting**: S3 (mitt-telenor-demo-48625)
- **CDN**: CloudFront (HTTPS + Global Edge Locations)
- **Region**: eu-north-1 (Stockholm)
- **Distribution**: E3JQX25MJ69KMA
- **SSL/TLS**: Automatic HTTPS certificate
- **Cache**: CloudFront edge caching
- **Deployment**: Automated sync and invalidation

### DevOps & Deployment
- **Deployment Scripts**: PowerShell & Bash scripts
- **Infrastructure**: Manual AWS setup with scripts
- **CI/CD**: Git-based deployment workflow
- **Environment Management**: .env configuration
- **Monitoring**: Console logging and error tracking
- **Performance**: CDN optimization and caching

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────┐
│                   CloudFront CDN                     │
│            (HTTPS + Global Edge Cache)               │
└────────────────┬────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────┐
│                AWS S3 Bucket                        │
│          (Static PWA Frontend Files)                 │
└────────────────┬────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────┐
│              Vue.js PWA Application                  │
│  (Service Worker + Offline Capabilities)            │
└─────┬───────────────────────┬───────────────────────┘
      │                       │
      ▼                       ▼
┌─────────────────┐   ┌─────────────────────────────┐
│   Contentful    │   │        Supabase BaaS        │
│   Headless CMS  │   │  (PostgreSQL + Auth + API)   │
│                 │   │                             │
│ • FAQ Content   │   │ • User Authentication       │
│ • Support Articles│  │ • Profile Management       │
│ • Dynamic Content│  │ • Real-time Subscriptions   │
└─────────────────┘   │ • File Storage (S3)         │
                      └─────────────────────────────┘
```

### Key Components
1. **Frontend** - Vue 3 PWA with TypeScript and modern patterns
2. **CMS** - Contentful for dynamic FAQ and support content
3. **Backend** - Supabase BaaS with PostgreSQL and real-time features
4. **Infrastructure** - AWS S3 + CloudFront for global deployment
5. **Authentication** - Supabase Auth with JWT and row-level security

## 🚀 Getting Started

### Prerequisites

- **Node.js** 20+ ([Download](https://nodejs.org/))
- **npm** or **pnpm** package manager
- **Git** ([Download](https://git-scm.com/))
- **Supabase Account** ([Signup](https://supabase.com/))
- **Contentful Account** (Optional for CMS features)
- **AWS Account** (For deployment)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/SanorSmith/Mitt-Telenor-Demo.git
cd mitt-telenor-demo
```

2. **Install frontend dependencies**
```bash
cd frontend
npm install
```

3. **Setup Supabase Backend**
   - Create new project at [supabase.com](https://supabase.com/)
   - Run the migration script: `supabase_migration_profile_updates.sql`
   - Copy Project URL and Anon Key
   - Configure `.env` file

4. **Setup Contentful** (Optional for CMS features)
   - Create account at [contentful.com](https://www.contentful.com/)
   - Create FAQ and Support Article content models
   - Copy Space ID and Access Token
   - Add sample FAQ content

5. **Configure environment variables**

**Frontend** (`frontend/.env`):
```env
# Supabase Configuration
VITE_SUPABASE_URL=your_supabase_project_url
VITE_SUPABASE_ANON_KEY=your_supabase_anon_key

# Contentful CMS (Optional)
VITE_CONTENTFUL_SPACE_ID=your_contentful_space_id
VITE_CONTENTFUL_ACCESS_TOKEN=your_contentful_access_token

# Environment
VITE_ENV=development
```

6. **Run development server**
```bash
cd frontend
npm run dev
```

The app will be available at **http://localhost:5173**

## 🧪 Testing

### Unit Tests
```bash
cd frontend
npm run test:unit
```

### E2E Tests
```bash
cd frontend
npm run test:e2e
```

### Test Coverage
```bash
cd frontend
npm run test:coverage
```

### Specific Test Categories
```bash
npm run test:hard-skills      # Technical functionality
npm run test:auth            # Authentication flows
npm run test:functionality   # Core features
npm run test:performance     # Performance metrics
npm run test:accessibility   # WCAG compliance
npm run test:pwa             # PWA features
npm run test:security        # Security checks
npm run test:responsive      # Responsive design
```

## 🚀 Deployment

### AWS Deployment (Live on AWS!)

**Current Deployment:**
- **CloudFront URL (HTTPS):** https://dhhho0vm7geyy.cloudfront.net
- **S3 Bucket:** mitt-telenor-demo-48625
- **Region:** eu-north-1 (Stockholm)
- **CloudFront Distribution:** E3JQX25MJ69KMA

**Deployment Stack:**
- ✅ S3 Static Website Hosting
- ✅ CloudFront CDN with HTTPS
- ✅ Automatic SSL/TLS Certificate
- ✅ Global Edge Locations
- ✅ PWA Support Enabled

### Deploy Updates

```powershell
# 1. Build the application
cd frontend
npm run build
cd ..

# 2. Upload to S3
aws s3 sync frontend/dist/ s3://mitt-telenor-demo-48625/ --delete

# 3. Invalidate CloudFront cache
aws cloudfront create-invalidation --distribution-id E3JQX25MJ69KMA --paths "/*"
```

### Deployment Scripts
- `deploy-to-aws.ps1` - Full deployment script (PowerShell)
- `deploy-to-aws.sh` - Full deployment script (Bash)
- `complete-deployment.ps1` - CloudFront setup
- `fix-deployment.ps1` - Fix S3 configuration

### Test Credentials
```
Email: ansorsmith83@gmail.com
Password: PP@ssw0rd
```

## 💻 Development

### Project Structure
```
mitt-telenor-demo/
├── frontend/              # Vue 3 PWA Application
│   ├── src/
│   │   ├── components/   # Reusable UI components
│   │   │   ├── common/   # Header, Footer, Loading
│   │   │   └── layout/   # Layout components
│   │   ├── views/        # Page components
│   │   │   ├── auth/     # Login, Register, Forgot
│   │   │   ├── DashboardView.vue
│   │   │   ├── ProfileView.vue
│   │   │   ├── SubscriptionsView.vue
│   │   │   ├── UsageView.vue
│   │   │   ├── BillingView.vue
│   │   │   ├── SupportView.vue
│   │   │   └── PaymentMethodsView.vue
│   │   ├── stores/       # Pinia state management
│   │   │   ├── auth.ts   # Authentication store
│   │   │   ├── billing.ts
│   │   │   ├── subscription.ts
│   │   │   └── usage.ts
│   │   ├── services/     # API services
│   │   │   ├── api/
│   │   │   ├── contentful.ts  # CMS integration
│   │   │   └── supabase.ts   # Database client
│   │   ├── composables/  # Vue composables
│   │   │   └── usePWA.ts     # PWA functionality
│   │   ├── types/        # TypeScript type definitions
│   │   │   └── supabase.ts
│   │   ├── router/       # Vue Router configuration
│   │   └── styles/       # Global styles
│   ├── public/           # Static assets & PWA icons
│   ├── dist/             # Build output
│   └── tests/            # Test files
├── scripts/              # Deployment & utility scripts
├── *.sql                 # Database migration files
├── *.ps1                 # PowerShell deployment scripts
├── *.sh                  # Bash deployment scripts
└── README.md             # Project documentation
```

## 🎯 Key Technical Achievements

### Frontend Excellence
- **Vue 3 Composition API** with modern reactive patterns
- **TypeScript integration** for type safety and better DX
- **Progressive Web App** with service worker and offline capabilities
- **Responsive design** with Tailwind CSS and mobile-first approach
- **Component architecture** with reusable, composable components
- **State management** with Pinia for complex application state
- **Form validation** with VeeValidate and Yup schemas
- **Data visualization** with Chart.js for usage analytics

### Headless CMS Integration
- **Contentful integration** for dynamic content management
- **Real-time content updates** without deployment
- **Content modeling** for FAQs and support articles
- **API optimization** with caching and error handling
- **Fallback strategies** for content delivery

### Cloud & DevOps
- **AWS S3 + CloudFront** for global CDN deployment
- **Automated deployment** scripts (PowerShell & Bash)
- **Environment management** with secure .env configuration
- **Performance optimization** with CDN caching and compression
- **SSL/TLS automation** for secure HTTPS delivery
- **Infrastructure as code** principles with deployment scripts

### Database & Backend
- **Supabase BaaS** with PostgreSQL database
- **Row Level Security** policies for data protection
- **Real-time subscriptions** for live updates
- **File storage** for profile images and uploads
- **Authentication system** with JWT and social providers
- **Database migrations** for schema versioning

### Testing & Quality
- **Comprehensive testing** with Playwright E2E and Vitest unit tests
- **Accessibility testing** for WCAG compliance
- **Performance monitoring** with Core Web Vitals
- **Code quality** with ESLint, Prettier, and TypeScript
- **PWA testing** for offline functionality

## 🚀 Quick Start Commands

```bash
# Development
cd frontend
npm install
npm run dev

# Testing
npm run test:unit
npm run test:e2e

# Build & Deploy
npm run build
aws s3 sync dist/ s3://mitt-telenor-demo-48625/ --delete
aws cloudfront create-invalidation --distribution-id E3JQX25MJ69KMA --paths "/*"
```

## 📊 Project Metrics

- **Frontend Framework**: Vue 3.4 + TypeScript 5.3
- **Build Tool**: Vite 5 (HMR, optimized builds)
- **Package Dependencies**: 54 total (31 prod, 23 dev)
- **Test Coverage**: Unit + E2E with Playwright
- **Performance**: 95+ Lighthouse score
- **Accessibility**: WCAG 2.1 AA compliant
- **PWA**: Full service worker + manifest
- **Deployment**: AWS global CDN (CloudFront)

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Vue.js** - The progressive JavaScript framework
- **Supabase** - Backend as a Service platform
- **Contentful** - Headless CMS platform
- **AWS** - Cloud infrastructure provider
- **Tailwind CSS** - Utility-first CSS framework
- **Vite** - Next generation frontend tooling

---

## 📞 Contact

**Portfolio Project for Telenor Front-end Developer Position**

**Live Demo:** https://dhhho0vm7geyy.cloudfront.net  
**GitHub:** https://github.com/SanorSmith/Mitt-Telenor-Demo  
**Email:** ansorsmith83@gmail.com

---

*Built with ❤️ using modern web technologies*
