-- Fix Missing Profiles for Existing Users
-- Run this in Supabase SQL Editor to create profiles for users who don't have them

-- First, let's see which users are missing profiles
SELECT 
  u.id,
  u.email,
  u.created_at,
  CASE WHEN p.id IS NULL THEN 'MISSING PROFILE' ELSE 'HAS PROFILE' END as status
FROM auth.users u
LEFT JOIN profiles p ON u.id = p.id
ORDER BY u.created_at DESC;

-- Create profiles for users who don't have them
INSERT INTO profiles (id, email, first_name, last_name, created_at, updated_at)
SELECT 
  u.id,
  u.email,
  COALESCE(u.raw_user_meta_data->>'first_name', ''),
  COALESCE(u.raw_user_meta_data->>'last_name', ''),
  u.created_at,
  NOW()
FROM auth.users u
LEFT JOIN profiles p ON u.id = p.id
WHERE p.id IS NULL
ON CONFLICT (id) DO NOTHING;

-- Verify all users now have profiles
SELECT 
  COUNT(*) as total_users,
  COUNT(p.id) as users_with_profiles,
  COUNT(*) - COUNT(p.id) as users_without_profiles
FROM auth.users u
LEFT JOIN profiles p ON u.id = p.id;

-- Show the created profiles
SELECT 
  id,
  email,
  first_name,
  last_name,
  created_at
FROM profiles
ORDER BY created_at DESC
LIMIT 10;
