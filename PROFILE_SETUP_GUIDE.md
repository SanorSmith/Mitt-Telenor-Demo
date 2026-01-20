# Profile Features Setup Guide

## Issue: Profile Updates and Image Upload Not Working

### Problem 1: Missing Database Columns
**Error:** `Could not find the 'date_of_birth' column of 'profiles' in the schema cache`

**Solution:** Run the migration script to add missing columns to your Supabase database.

### Problem 2: Profile Image Upload Fails
**Error:** Storage bucket 'profiles' doesn't exist or lacks proper permissions

**Solution:** Create storage bucket and set up policies in Supabase.

---

## Step-by-Step Fix

### 1. Run Database Migration

Go to your Supabase Dashboard:
1. Navigate to **SQL Editor**
2. Click **New Query**
3. Copy and paste the contents of `supabase_migration_profile_updates.sql`
4. Click **Run** or press `Ctrl+Enter`

This will:
- ✅ Add `date_of_birth` column to profiles table
- ✅ Add `is_unsubscribed` column to profiles table
- ✅ Add `unsubscribed_at` column to profiles table
- ✅ Create 'profiles' storage bucket
- ✅ Set up storage policies for profile images

### 2. Verify Storage Bucket

Go to **Storage** in Supabase Dashboard:
1. You should see a bucket named **"profiles"**
2. It should be marked as **Public**
3. Click on it to verify it's accessible

### 3. Test Profile Updates

1. Go to your app at `http://localhost:5173`
2. Navigate to **Profile** page
3. Try updating your profile information:
   - First Name
   - Last Name
   - Phone
   - Date of Birth
   - Address
4. Click **Save Changes**
5. You should see: "Profile updated successfully!"

### 4. Test Image Upload

1. On the Profile page, click **Upload Photo**
2. Select an image file (JPG, PNG, max 5MB)
3. Wait for upload to complete
4. You should see:
   - "Photo uploaded successfully!"
   - Your photo displayed in the profile picture area

---

## What the Migration Does

### Database Columns Added:
```sql
date_of_birth DATE                  -- User's date of birth
is_unsubscribed BOOLEAN DEFAULT FALSE  -- Account unsubscribe status
unsubscribed_at TIMESTAMPTZ         -- When account was unsubscribed
```

### Storage Bucket Created:
- **Name:** profiles
- **Public:** Yes (for viewing profile images)
- **Folder Structure:** `avatars/{user_id}/{filename}`

### Storage Policies:
1. **Upload:** Users can upload to their own folder
2. **Update:** Users can update their own images
3. **Delete:** Users can delete their own images
4. **View:** Anyone can view profile images (public)

---

## Troubleshooting

### If profile update still fails:
1. Check browser console for errors
2. Verify you're logged in
3. Check Supabase logs in Dashboard > Logs
4. Ensure RLS policies allow updates

### If image upload still fails:
1. Check file size (must be < 5MB)
2. Check file type (must be image/*)
3. Verify storage bucket exists
4. Check storage policies are active
5. Look at browser console for detailed error

### Common Errors:

**"Profile updated successfully!" but data doesn't save:**
- Clear browser cache
- Check if RLS policies are blocking updates
- Verify user is authenticated

**"Failed to upload photo: new row violates row-level security policy":**
- Storage policies not set up correctly
- Run the migration script again
- Check user is authenticated

**"Failed to upload photo: Bucket not found":**
- Storage bucket doesn't exist
- Create it manually in Supabase Dashboard > Storage
- Or run the migration script

---

## Manual Storage Bucket Setup (Alternative)

If the migration script doesn't create the bucket:

1. Go to **Supabase Dashboard > Storage**
2. Click **New Bucket**
3. Name: `profiles`
4. Public: **Yes** (checked)
5. Click **Create Bucket**

Then add policies:
1. Click on the **profiles** bucket
2. Go to **Policies** tab
3. Click **New Policy**
4. Use the SQL from `supabase_migration_profile_updates.sql`

---

## Verification Checklist

After running the migration, verify:

- [ ] `date_of_birth` column exists in profiles table
- [ ] `is_unsubscribed` column exists in profiles table
- [ ] `unsubscribed_at` column exists in profiles table
- [ ] Storage bucket 'profiles' exists
- [ ] Storage bucket is marked as Public
- [ ] Storage policies are active
- [ ] Profile updates work without errors
- [ ] Image upload works and displays photo
- [ ] Unsubscribe button appears on profile page
- [ ] Account reactivation prompt shows on login for unsubscribed users

---

## Need Help?

If issues persist:
1. Check Supabase Dashboard > Logs for errors
2. Open browser DevTools > Console for frontend errors
3. Verify all environment variables are set correctly in `.env`
4. Ensure you're using the correct Supabase project URL and anon key
