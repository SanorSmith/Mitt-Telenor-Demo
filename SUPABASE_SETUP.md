# Supabase Setup Guide

## Step 1: Create a Supabase Project

1. Go to [https://supabase.com](https://supabase.com)
2. Sign up or log in
3. Click "New Project"
4. Fill in:
   - **Name**: Telenor Demo
   - **Database Password**: (choose a strong password)
   - **Region**: Choose closest to you
5. Click "Create new project"
6. Wait 2-3 minutes for setup to complete

## Step 2: Get Your API Credentials

1. In your Supabase project dashboard, go to **Settings** → **API**
2. Copy these values:
   - **Project URL** (e.g., `https://xxxxx.supabase.co`)
   - **anon public** key (the long string under "Project API keys")

## Step 3: Configure Frontend

1. Create a `.env` file in the `frontend` folder:

```bash
cd frontend
cp .env.example .env
```

2. Edit `.env` and add your Supabase credentials:

```env
VITE_SUPABASE_URL=https://your-project.supabase.co
VITE_SUPABASE_ANON_KEY=your-anon-key-here
```

## Step 4: Create Database Schema

Go to **SQL Editor** in your Supabase dashboard and run this SQL:

```sql
-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Profiles table (extends auth.users)
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
  type TEXT NOT NULL CHECK (type IN ('mobile', 'broadband', 'tv')),
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
  status TEXT NOT NULL DEFAULT 'active' CHECK (status IN ('active', 'suspended', 'cancelled')),
  start_date TIMESTAMPTZ DEFAULT NOW(),
  end_date TIMESTAMPTZ,
  auto_renew BOOLEAN DEFAULT TRUE,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Add-ons table
CREATE TABLE addons (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  subscription_id UUID NOT NULL REFERENCES subscriptions(id) ON DELETE CASCADE,
  name TEXT NOT NULL,
  price DECIMAL(10,2) NOT NULL,
  data_gb INTEGER,
  voice_minutes INTEGER,
  sms_count INTEGER,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Usage records table
CREATE TABLE usage_records (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
  subscription_id UUID NOT NULL REFERENCES subscriptions(id) ON DELETE CASCADE,
  period TEXT NOT NULL,
  data_used_mb INTEGER DEFAULT 0,
  data_allowance_mb INTEGER NOT NULL,
  voice_used_minutes INTEGER DEFAULT 0,
  voice_allowance_minutes INTEGER NOT NULL,
  sms_used_count INTEGER DEFAULT 0,
  sms_allowance_count INTEGER NOT NULL,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW(),
  UNIQUE(user_id, period)
);

-- Invoices table
CREATE TABLE invoices (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
  invoice_number TEXT NOT NULL UNIQUE,
  amount DECIMAL(10,2) NOT NULL,
  tax DECIMAL(10,2) NOT NULL,
  total DECIMAL(10,2) NOT NULL,
  status TEXT NOT NULL DEFAULT 'pending' CHECK (status IN ('pending', 'paid', 'overdue', 'cancelled')),
  due_date TIMESTAMPTZ NOT NULL,
  paid_date TIMESTAMPTZ,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Invoice items table
CREATE TABLE invoice_items (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  invoice_id UUID NOT NULL REFERENCES invoices(id) ON DELETE CASCADE,
  description TEXT NOT NULL,
  quantity INTEGER NOT NULL DEFAULT 1,
  unit_price DECIMAL(10,2) NOT NULL,
  total DECIMAL(10,2) NOT NULL,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Payments table
CREATE TABLE payments (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
  invoice_id UUID NOT NULL REFERENCES invoices(id),
  amount DECIMAL(10,2) NOT NULL,
  payment_method TEXT NOT NULL,
  status TEXT NOT NULL DEFAULT 'pending' CHECK (status IN ('pending', 'completed', 'failed')),
  transaction_id TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Payment methods table
CREATE TABLE payment_methods (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
  type TEXT NOT NULL CHECK (type IN ('card', 'bank_account', 'paypal')),
  last_four TEXT NOT NULL,
  expiry_month INTEGER,
  expiry_year INTEGER,
  is_default BOOLEAN DEFAULT FALSE,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Notifications table
CREATE TABLE notifications (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
  title TEXT NOT NULL,
  message TEXT NOT NULL,
  type TEXT NOT NULL DEFAULT 'info' CHECK (type IN ('info', 'warning', 'error', 'success')),
  read BOOLEAN DEFAULT FALSE,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Create indexes for better performance
CREATE INDEX idx_subscriptions_user_id ON subscriptions(user_id);
CREATE INDEX idx_subscriptions_plan_id ON subscriptions(plan_id);
CREATE INDEX idx_addons_subscription_id ON addons(subscription_id);
CREATE INDEX idx_usage_records_user_id ON usage_records(user_id);
CREATE INDEX idx_usage_records_period ON usage_records(period);
CREATE INDEX idx_invoices_user_id ON invoices(user_id);
CREATE INDEX idx_invoices_status ON invoices(status);
CREATE INDEX idx_payments_user_id ON payments(user_id);
CREATE INDEX idx_payments_invoice_id ON payments(invoice_id);
CREATE INDEX idx_notifications_user_id ON notifications(user_id);
CREATE INDEX idx_notifications_read ON notifications(read);

-- Create updated_at trigger function
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
  NEW.updated_at = NOW();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Add triggers for updated_at
CREATE TRIGGER update_profiles_updated_at BEFORE UPDATE ON profiles
  FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_plans_updated_at BEFORE UPDATE ON plans
  FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_subscriptions_updated_at BEFORE UPDATE ON subscriptions
  FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_usage_records_updated_at BEFORE UPDATE ON usage_records
  FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
```

## Step 5: Set Up Row Level Security (RLS)

Run this SQL to enable security policies:

```sql
-- Enable RLS on all tables
ALTER TABLE profiles ENABLE ROW LEVEL SECURITY;
ALTER TABLE subscriptions ENABLE ROW LEVEL SECURITY;
ALTER TABLE addons ENABLE ROW LEVEL SECURITY;
ALTER TABLE usage_records ENABLE ROW LEVEL SECURITY;
ALTER TABLE invoices ENABLE ROW LEVEL SECURITY;
ALTER TABLE invoice_items ENABLE ROW LEVEL SECURITY;
ALTER TABLE payments ENABLE ROW LEVEL SECURITY;
ALTER TABLE payment_methods ENABLE ROW LEVEL SECURITY;
ALTER TABLE notifications ENABLE ROW LEVEL SECURITY;

-- Profiles policies
CREATE POLICY "Users can view own profile" ON profiles
  FOR SELECT USING (auth.uid() = id);

CREATE POLICY "Users can update own profile" ON profiles
  FOR UPDATE USING (auth.uid() = id);

CREATE POLICY "Users can insert own profile" ON profiles
  FOR INSERT WITH CHECK (auth.uid() = id);

-- Plans policies (public read)
CREATE POLICY "Anyone can view plans" ON plans
  FOR SELECT USING (true);

-- Subscriptions policies
CREATE POLICY "Users can view own subscriptions" ON subscriptions
  FOR SELECT USING (auth.uid() = user_id);

CREATE POLICY "Users can insert own subscriptions" ON subscriptions
  FOR INSERT WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Users can update own subscriptions" ON subscriptions
  FOR UPDATE USING (auth.uid() = user_id);

-- Add-ons policies
CREATE POLICY "Users can view own addons" ON addons
  FOR SELECT USING (
    EXISTS (
      SELECT 1 FROM subscriptions
      WHERE subscriptions.id = addons.subscription_id
      AND subscriptions.user_id = auth.uid()
    )
  );

CREATE POLICY "Users can manage own addons" ON addons
  FOR ALL USING (
    EXISTS (
      SELECT 1 FROM subscriptions
      WHERE subscriptions.id = addons.subscription_id
      AND subscriptions.user_id = auth.uid()
    )
  );

-- Usage records policies
CREATE POLICY "Users can view own usage" ON usage_records
  FOR SELECT USING (auth.uid() = user_id);

CREATE POLICY "Users can insert own usage" ON usage_records
  FOR INSERT WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Users can update own usage" ON usage_records
  FOR UPDATE USING (auth.uid() = user_id);

-- Invoices policies
CREATE POLICY "Users can view own invoices" ON invoices
  FOR SELECT USING (auth.uid() = user_id);

-- Invoice items policies
CREATE POLICY "Users can view own invoice items" ON invoice_items
  FOR SELECT USING (
    EXISTS (
      SELECT 1 FROM invoices
      WHERE invoices.id = invoice_items.invoice_id
      AND invoices.user_id = auth.uid()
    )
  );

-- Payments policies
CREATE POLICY "Users can view own payments" ON payments
  FOR SELECT USING (auth.uid() = user_id);

CREATE POLICY "Users can create own payments" ON payments
  FOR INSERT WITH CHECK (auth.uid() = user_id);

-- Payment methods policies
CREATE POLICY "Users can manage own payment methods" ON payment_methods
  FOR ALL USING (auth.uid() = user_id);

-- Notifications policies
CREATE POLICY "Users can view own notifications" ON notifications
  FOR SELECT USING (auth.uid() = user_id);

CREATE POLICY "Users can update own notifications" ON notifications
  FOR UPDATE USING (auth.uid() = user_id);
```

## Step 6: Insert Sample Data

```sql
-- Insert sample plans
INSERT INTO plans (name, type, price, data_gb, voice_minutes, sms_count, features, is_popular) VALUES
  ('Basic Mobile', 'mobile', 299, 5, 500, 1000, ARRAY['5GB Data', '500 Minutes', '1000 SMS', '4G Speed'], false),
  ('Premium Mobile', 'mobile', 499, 20, 2000, 5000, ARRAY['20GB Data', '2000 Minutes', '5000 SMS', '5G Speed', 'International Calls'], true),
  ('Unlimited Mobile', 'mobile', 699, NULL, NULL, NULL, ARRAY['Unlimited Data', 'Unlimited Calls', 'Unlimited SMS', '5G Speed', 'Roaming Included'], false),
  ('Home Broadband', 'broadband', 399, NULL, NULL, NULL, ARRAY['100 Mbps', 'Unlimited Data', 'Free Router', '24/7 Support'], true),
  ('Fiber 500', 'broadband', 599, NULL, NULL, NULL, ARRAY['500 Mbps', 'Unlimited Data', 'Free Installation', 'WiFi 6 Router'], false);
```

## Step 7: Configure Authentication

1. Go to **Authentication** → **Providers** in Supabase
2. Enable **Email** provider (should be enabled by default)
3. Optional: Enable social providers (Google, GitHub, etc.)

## Step 8: Test the Application

1. Start the frontend:
```bash
cd frontend
pnpm dev
```

2. Open http://localhost:5173
3. Register a new account
4. Login and explore!

## Troubleshooting

### "Invalid API key" error
- Double-check your `.env` file has the correct `VITE_SUPABASE_URL` and `VITE_SUPABASE_ANON_KEY`
- Restart the dev server after changing `.env`

### "Row Level Security" errors
- Make sure you ran all the RLS policies SQL
- Check that you're logged in (auth.uid() returns a value)

### Tables not found
- Verify you ran the schema SQL in the correct order
- Check the **Table Editor** in Supabase to see if tables exist

## Next Steps

- Add more sample data
- Customize the plans
- Set up email templates in Supabase Auth
- Configure storage buckets for profile pictures
- Add real-time subscriptions for live updates

## Production Checklist

- [ ] Change database password
- [ ] Enable 2FA on Supabase account
- [ ] Set up custom domain
- [ ] Configure email templates
- [ ] Set up backups
- [ ] Add monitoring
- [ ] Review RLS policies
- [ ] Load test the application
