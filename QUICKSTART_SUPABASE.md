# Quick Start with Supabase - 5 Minutes Setup

## 🚀 Fast Track Setup

### 1. Create Supabase Project (2 minutes)

1. Go to https://supabase.com and sign up/login
2. Click **"New Project"**
3. Enter:
   - Name: `telenor-demo`
   - Password: (choose any strong password)
   - Region: (closest to you)
4. Click **"Create new project"**
5. ⏰ Wait 2-3 minutes for setup

### 2. Get Your Credentials (30 seconds)

1. In Supabase dashboard, go to **Settings** → **API**
2. Copy these two values:
   - **Project URL** (looks like: `https://xxxxx.supabase.co`)
   - **anon public** key (long string under "Project API keys")

### 3. Configure Frontend (30 seconds)

Create `frontend/.env` file with:

```env
VITE_SUPABASE_URL=https://your-project-id.supabase.co
VITE_SUPABASE_ANON_KEY=your-anon-key-here
```

### 4. Create Database (1 minute)

1. In Supabase, go to **SQL Editor**
2. Click **"New query"**
3. Copy and paste this SQL:

```sql
-- Quick setup script
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Profiles table
CREATE TABLE profiles (
  id UUID PRIMARY KEY REFERENCES auth.users(id) ON DELETE CASCADE,
  email TEXT NOT NULL UNIQUE,
  first_name TEXT NOT NULL,
  last_name TEXT NOT NULL,
  phone TEXT,
  address TEXT,
  profile_picture_url TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Plans table
CREATE TABLE plans (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  name TEXT NOT NULL,
  type TEXT NOT NULL,
  price DECIMAL(10,2) NOT NULL,
  data_gb INTEGER,
  voice_minutes INTEGER,
  sms_count INTEGER,
  features TEXT[] DEFAULT '{}',
  is_popular BOOLEAN DEFAULT FALSE,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Subscriptions table
CREATE TABLE subscriptions (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
  plan_id UUID NOT NULL REFERENCES plans(id),
  status TEXT NOT NULL DEFAULT 'active',
  start_date TIMESTAMPTZ DEFAULT NOW(),
  end_date TIMESTAMPTZ,
  auto_renew BOOLEAN DEFAULT TRUE,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Enable RLS
ALTER TABLE profiles ENABLE ROW LEVEL SECURITY;
ALTER TABLE subscriptions ENABLE ROW LEVEL SECURITY;

-- Basic policies
CREATE POLICY "Users can view own profile" ON profiles FOR SELECT USING (auth.uid() = id);
CREATE POLICY "Users can insert own profile" ON profiles FOR INSERT WITH CHECK (auth.uid() = id);
CREATE POLICY "Users can update own profile" ON profiles FOR UPDATE USING (auth.uid() = id);
CREATE POLICY "Anyone can view plans" ON plans FOR SELECT USING (true);
CREATE POLICY "Users can view own subscriptions" ON subscriptions FOR SELECT USING (auth.uid() = user_id);

-- Sample data
INSERT INTO plans (name, type, price, data_gb, voice_minutes, sms_count, features, is_popular) VALUES
  ('Basic Mobile', 'mobile', 299, 5, 500, 1000, ARRAY['5GB Data', '500 Minutes', '1000 SMS'], false),
  ('Premium Mobile', 'mobile', 499, 20, 2000, 5000, ARRAY['20GB Data', '2000 Minutes', '5000 SMS', '5G Speed'], true),
  ('Unlimited', 'mobile', 699, NULL, NULL, NULL, ARRAY['Unlimited Everything', '5G Speed'], false);
```

4. Click **"Run"** or press `Ctrl+Enter`

### 5. Start the App (30 seconds)

```bash
cd frontend
pnpm dev
```

### 6. Test It! 🎉

1. Open http://localhost:5173
2. Click **"Register"**
3. Create an account with:
   - Email: test@example.com
   - Password: Test123!
   - Name: Test User
4. Login and explore!

## ✅ That's It!

You now have:
- ✅ Working authentication
- ✅ User profiles
- ✅ Database with plans
- ✅ Secure API access
- ✅ Real-time capabilities

## 🐛 Troubleshooting

**"Invalid API key"**
- Check your `.env` file
- Restart dev server: `pnpm dev`

**"Table doesn't exist"**
- Make sure SQL ran successfully
- Check **Table Editor** in Supabase

**Can't register**
- Check **Authentication** → **Providers** → Email is enabled
- Check browser console for errors

## 📚 Full Setup

For complete schema with invoices, payments, usage tracking, see `SUPABASE_SETUP.md`

## 🎯 Next Steps

1. Add more tables (see SUPABASE_SETUP.md)
2. Customize plans
3. Add profile pictures (Supabase Storage)
4. Enable real-time updates
5. Deploy to production!

---

**Need help?** Check the full documentation in `SUPABASE_SETUP.md`
