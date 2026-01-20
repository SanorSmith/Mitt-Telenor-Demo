# Mitt Telenor Demo - Supabase Edition

A modern, full-stack self-service platform for Telenor customers built with Vue 3, TypeScript, and Supabase.

## 🚀 Quick Start (5 Minutes)

### Prerequisites
- Node.js 20+
- pnpm 8+
- A Supabase account (free tier works!)

### Setup Steps

1. **Clone and Install**
```bash
cd frontend
pnpm install
```

2. **Create Supabase Project**
   - Go to https://supabase.com
   - Create a new project
   - Copy your Project URL and anon key

3. **Configure Environment**
```bash
# Edit frontend/.env with your Supabase credentials
VITE_SUPABASE_URL=https://your-project.supabase.co
VITE_SUPABASE_ANON_KEY=your-anon-key
```

4. **Setup Database**
   - Open Supabase SQL Editor
   - Run the SQL from `QUICKSTART_SUPABASE.md`

5. **Start the App**
```bash
pnpm dev
```

6. **Open & Test**
   - Visit http://localhost:5173
   - Register a new account
   - Explore the platform!

## 📚 Documentation

- **Quick Start**: `QUICKSTART_SUPABASE.md` - 5-minute setup
- **Full Setup**: `SUPABASE_SETUP.md` - Complete database schema and RLS policies
- **Deployment**: `DEPLOYMENT_GUIDE.md` - AWS deployment (optional)

## 🏗️ Architecture

### Tech Stack
- **Frontend**: Vue 3, TypeScript, Pinia, Tailwind CSS
- **Backend**: Supabase (PostgreSQL, Auth, Storage, Real-time)
- **Icons**: Lucide Vue
- **Charts**: Chart.js (for usage visualization)
- **Testing**: Vitest, Playwright

### Why Supabase?

✅ **No backend code needed** - Instant REST and GraphQL APIs  
✅ **Built-in authentication** - Email, OAuth, magic links  
✅ **Real-time subscriptions** - Live data updates  
✅ **Row Level Security** - Database-level authorization  
✅ **File storage** - Profile pictures, documents  
✅ **Easy deployment** - Just deploy the frontend  
✅ **Free tier** - Perfect for demos and prototypes  

## 🎯 Features

### Implemented
- ✅ User Authentication (Register, Login, Logout)
- ✅ User Profiles
- ✅ Subscription Management
- ✅ Plan Selection
- ✅ Usage Tracking
- ✅ Billing & Invoices
- ✅ Payment Methods
- ✅ Notifications
- ✅ Responsive Design
- ✅ PWA Support

### Database Schema
- **profiles** - User profile information
- **plans** - Available subscription plans
- **subscriptions** - User subscriptions
- **addons** - Subscription add-ons
- **usage_records** - Data/voice/SMS usage tracking
- **invoices** - Billing invoices
- **invoice_items** - Invoice line items
- **payments** - Payment history
- **payment_methods** - Saved payment methods
- **notifications** - User notifications

## 🔒 Security

- Row Level Security (RLS) enabled on all tables
- Users can only access their own data
- Secure authentication with JWT tokens
- API keys stored in environment variables
- HTTPS enforced in production

## 🧪 Testing

```bash
# Unit tests
pnpm test

# E2E tests
pnpm test:e2e

# Coverage
pnpm test:coverage
```

## 📱 PWA Features

- Offline support
- Install as app
- Push notifications (ready)
- Background sync (ready)

## 🎨 UI/UX

- Modern, clean design
- Fully responsive (mobile, tablet, desktop)
- Accessible (WCAG 2.1 AA compliant)
- Dark mode ready
- Loading states
- Error handling
- Toast notifications

## 🚀 Deployment

### Frontend (Netlify/Vercel)
```bash
pnpm build
# Deploy dist/ folder
```

### Environment Variables
Set these in your hosting platform:
- `VITE_SUPABASE_URL`
- `VITE_SUPABASE_ANON_KEY`

## 📊 Project Structure

```
frontend/
├── src/
│   ├── components/     # Reusable components
│   ├── layouts/        # Page layouts
│   ├── views/          # Page components
│   ├── stores/         # Pinia stores (state management)
│   ├── router/         # Vue Router configuration
│   ├── lib/            # Supabase client
│   ├── types/          # TypeScript types
│   └── assets/         # Static assets
├── public/             # Public assets
└── tests/              # Test files
```

## 🔧 Development

### Available Scripts

```bash
pnpm dev          # Start dev server
pnpm build        # Build for production
pnpm preview      # Preview production build
pnpm test         # Run unit tests
pnpm test:e2e     # Run E2E tests
pnpm lint         # Lint code
pnpm type-check   # TypeScript type checking
```

### Adding New Features

1. **Add database table** - Run SQL in Supabase
2. **Update types** - Add to `src/types/supabase.ts`
3. **Create store** - Add Pinia store in `src/stores/`
4. **Create view** - Add Vue component in `src/views/`
5. **Add route** - Update `src/router/index.ts`

## 🐛 Troubleshooting

### Common Issues

**"Invalid API key"**
- Check `.env` file has correct credentials
- Restart dev server after changing `.env`

**"Row Level Security" errors**
- Verify RLS policies are set up
- Check you're logged in (user session exists)

**"Table not found"**
- Run database setup SQL
- Check table names match exactly

**Build errors**
- Clear node_modules: `rm -rf node_modules && pnpm install`
- Clear cache: `rm -rf .vite`

## 📈 Performance

- Lighthouse Score: 90+ (target)
- First Contentful Paint: < 1.5s
- Time to Interactive: < 3.5s
- Bundle size: < 500KB (gzipped)

## 🤝 Contributing

This is a demo project for the Telenor Front-end Developer position.

## 📄 License

MIT

## 🙏 Acknowledgments

- Vue.js team for the amazing framework
- Supabase team for the excellent backend platform
- Telenor for the opportunity

## 📞 Support

For questions or issues:
1. Check `QUICKSTART_SUPABASE.md`
2. Check `SUPABASE_SETUP.md`
3. Review Supabase documentation
4. Check browser console for errors

---

**Built with ❤️ for Telenor**
