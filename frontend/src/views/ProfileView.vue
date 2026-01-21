<template>
  <div class="space-y-6">
    <h1 class="text-3xl font-bold text-gray-900">My Profile</h1>

    <div v-if="loading" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <div class="lg:col-span-2 space-y-6">
        <div class="card">
          <h2 class="text-xl font-semibold text-gray-900 mb-4">Personal Information</h2>
          <form @submit.prevent="handleUpdateProfile" class="space-y-4">
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label for="firstName" class="block text-sm font-medium text-gray-700 mb-1">
                  First Name
                </label>
                <input
                  id="firstName"
                  v-model="formData.firstName"
                  type="text"
                  required
                  class="input"
                />
              </div>

              <div>
                <label for="lastName" class="block text-sm font-medium text-gray-700 mb-1">
                  Last Name
                </label>
                <input
                  id="lastName"
                  v-model="formData.lastName"
                  type="text"
                  required
                  class="input"
                />
              </div>
            </div>

            <div>
              <label for="email" class="block text-sm font-medium text-gray-700 mb-1">
                Email Address
              </label>
              <input
                id="email"
                v-model="formData.email"
                type="email"
                disabled
                class="input bg-gray-50"
              />
            </div>

            <div>
              <label for="phone" class="block text-sm font-medium text-gray-700 mb-1">
                Phone Number
              </label>
              <input
                id="phone"
                v-model="formData.phone"
                type="tel"
                class="input"
              />
            </div>

            <div>
              <label for="dateOfBirth" class="block text-sm font-medium text-gray-700 mb-1">
                Date of Birth
              </label>
              <input
                id="dateOfBirth"
                v-model="formData.dateOfBirth"
                type="date"
                class="input"
              />
            </div>

            <div>
              <label for="address" class="block text-sm font-medium text-gray-700 mb-1">
                Address
              </label>
              <textarea
                id="address"
                v-model="formData.address"
                rows="2"
                class="input"
                placeholder="Street address, city, postal code"
              ></textarea>
            </div>

            <div class="flex justify-end space-x-3">
              <button type="button" @click="resetForm" class="btn-secondary">
                Cancel
              </button>
              <button type="submit" :disabled="updating" class="btn-primary">
                <span v-if="!updating">Save Changes</span>
                <span v-else>Saving...</span>
              </button>
            </div>
          </form>
        </div>

        <div class="card">
          <h2 class="text-xl font-semibold text-gray-900 mb-4">Notifications</h2>
          <div v-if="notifications.length === 0" class="text-center py-8 text-gray-500">
            No notifications
          </div>
          <div v-else class="space-y-3">
            <div
              v-for="notification in notifications"
              :key="notification.id"
              class="flex items-start space-x-3 p-3 rounded-lg"
              :class="notification.isRead ? 'bg-gray-50' : 'bg-blue-50'"
            >
              <div class="flex-1">
                <p class="font-medium text-gray-900">{{ notification.title }}</p>
                <p class="text-sm text-gray-600">{{ notification.message }}</p>
                <p class="text-xs text-gray-500 mt-1">
                  {{ formatDate(notification.createdAt) }}
                </p>
              </div>
              <button
                v-if="!notification.isRead"
                @click="markAsRead(notification.id)"
                class="text-sm text-primary-500 hover:text-primary-600"
              >
                Mark as read
              </button>
            </div>
          </div>
        </div>
      </div>

      <div class="space-y-6">
        <div class="card">
          <h2 class="text-xl font-semibold text-gray-900 mb-4">Profile Picture</h2>
          <div class="flex flex-col items-center">
            <div class="w-32 h-32 rounded-full bg-primary-100 flex items-center justify-center mb-4 overflow-hidden">
              <img v-if="formData.profilePictureUrl" :src="formData.profilePictureUrl" alt="Profile" class="w-full h-full object-cover" />
              <User v-else :size="64" class="text-primary-500" />
            </div>
            <input
              type="file"
              id="photoUpload"
              accept="image/*"
              @change="handlePhotoUpload"
              class="hidden"
            />
            <label for="photoUpload" class="btn-secondary text-sm cursor-pointer">
              {{ formData.profilePictureUrl ? 'Change Photo' : 'Upload Photo' }}
            </label>
            <p class="text-xs text-gray-500 mt-2">Max 5MB, JPG/PNG</p>
          </div>
        </div>

        <div class="card">
          <h2 class="text-xl font-semibold text-gray-900 mb-4">Account Stats</h2>
          <div class="space-y-3">
            <div class="flex justify-between">
              <span class="text-gray-600">Member Since</span>
              <span class="font-medium">{{ accountStatus.memberSince }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600">Account Status</span>
              <span class="font-medium" :class="accountStatus.isUnsubscribed ? 'text-red-600' : 'text-green-600'">
                {{ accountStatus.isUnsubscribed ? 'Unsubscribed' : 'Active' }}
              </span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600">Total Invoices</span>
              <span class="font-medium">{{ accountStatus.totalInvoices }}</span>
            </div>
          </div>
        </div>

        <div v-if="!accountStatus.isUnsubscribed" class="card bg-red-50 border border-red-200">
          <h2 class="text-xl font-semibold text-red-900 mb-2">Danger Zone</h2>
          <p class="text-sm text-red-700 mb-4">
            Unsubscribing will deactivate your account. You can reactivate by logging in again.
          </p>
          <button
            @click="handleUnsubscribe"
            :disabled="updating"
            class="w-full bg-red-600 text-white px-4 py-2 rounded-lg font-medium hover:bg-red-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {{ updating ? 'Processing...' : 'Unsubscribe Account' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { User } from 'lucide-vue-next'
import { useAuthStore } from '@/stores/auth'
import { supabase } from '@/lib/supabase'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const authStore = useAuthStore()

const loading = ref(false)
const updating = ref(false)
const notifications = ref<any[]>([])

const formData = reactive({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  dateOfBirth: '',
  address: '',
  profilePictureUrl: ''
})

const accountStatus = ref({
  isUnsubscribed: false,
  memberSince: '',
  totalInvoices: 0
})

const loadProfile = async () => {
  if (!authStore.user) {
    console.error('No authenticated user')
    return
  }
  
  loading.value = true
  try {
    // Load profile data
    const { data: profileData, error: profileError } = await supabase
      .from('profiles')
      .select('*')
      .eq('id', authStore.user.id)
      .single()
    
    if (profileError) {
      console.error('Profile error:', profileError)
      // If profile doesn't exist, use auth user data
      formData.email = authStore.user.email || ''
    } else if (profileData) {
      formData.firstName = profileData.first_name || ''
      formData.lastName = profileData.last_name || ''
      formData.email = profileData.email || authStore.user.email || ''
      formData.phone = profileData.phone || ''
      formData.dateOfBirth = profileData.date_of_birth || ''
      formData.address = profileData.address || ''
      formData.profilePictureUrl = profileData.profile_picture_url || ''
      accountStatus.value.isUnsubscribed = profileData.is_unsubscribed || false
      accountStatus.value.memberSince = profileData.created_at ? new Date(profileData.created_at).toLocaleDateString('en-US', { year: 'numeric', month: 'short' }) : 'N/A'
    }
    
    // Load invoice count
    const { count } = await supabase
      .from('invoices')
      .select('*', { count: 'exact', head: true })
      .eq('user_id', authStore.user.id)
    
    accountStatus.value.totalInvoices = count || 0
  } catch (error) {
    console.error('Error loading profile:', error)
    // Ensure email is always set
    if (!formData.email && authStore.user?.email) {
      formData.email = authStore.user.email
    }
  } finally {
    loading.value = false
  }
}

const resetForm = () => {
  loadProfile()
}

const handleUpdateProfile = async () => {
  if (!authStore.user) {
    alert('You must be logged in to update your profile')
    return
  }
  
  updating.value = true
  try {
    const updateData = {
      first_name: formData.firstName.trim(),
      last_name: formData.lastName.trim(),
      phone: formData.phone.trim() || null,
      address: formData.address.trim() || null,
      date_of_birth: formData.dateOfBirth || null,
      profile_picture_url: formData.profilePictureUrl || null,
      updated_at: new Date().toISOString()
    }
    
    console.log('Attempting to update profile with data:', updateData)
    console.log('User ID:', authStore.user.id)
    
    const { data, error, count } = await supabase
      .from('profiles')
      .update(updateData)
      .eq('id', authStore.user.id)
      .select()
    
    console.log('Update response:', { data, error, count })
    
    if (error) {
      console.error('Supabase error details:', error)
      throw error
    }
    
    if (!data || data.length === 0) {
      console.warn('No rows were updated. Profile may not exist. Attempting to create profile...')
      
      // Profile doesn't exist, create it
      const { data: newProfile, error: createError } = await supabase
        .from('profiles')
        .insert({
          id: authStore.user.id,
          email: authStore.user.email || '',
          first_name: formData.firstName.trim(),
          last_name: formData.lastName.trim(),
          phone: formData.phone.trim() || null,
          address: formData.address.trim() || null,
          date_of_birth: formData.dateOfBirth || null,
          profile_picture_url: formData.profilePictureUrl || null
        })
        .select()
      
      if (createError) {
        console.error('Error creating profile:', createError)
        throw new Error(`Failed to create profile: ${createError.message}`)
      }
      
      console.log('Profile created successfully:', newProfile)
      alert('Profile created and saved successfully!')
      await loadProfile()
      return
    }
    
    console.log('Profile updated successfully:', data)
    alert('Profile updated successfully!')
    await loadProfile() // Reload to confirm changes
  } catch (error: any) {
    console.error('Error updating profile:', error)
    
    let errorMessage = error.message || 'Please try again.'
    
    // Provide helpful error messages
    if (error.message?.includes('column') && error.message?.includes('does not exist')) {
      errorMessage = 'Database schema is missing columns. Please run the migration script (supabase_migration_profile_updates.sql) in your Supabase SQL Editor.'
    } else if (error.message?.includes('row-level security')) {
      errorMessage = 'Permission denied. Please check Row Level Security policies in Supabase.'
    } else if (error.message?.includes('Profile not found')) {
      errorMessage = error.message
    }
    
    alert(`Failed to update profile: ${errorMessage}`)
  } finally {
    updating.value = false
  }
}

const markAsRead = (id: string) => {
  const notification = notifications.value.find(n => n.id === id)
  if (notification) {
    notification.isRead = true
  }
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

const handlePhotoUpload = async (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  
  if (!file) return
  
  if (!file.type.startsWith('image/')) {
    alert('Please select an image file (JPG, PNG, GIF, etc.)')
    return
  }
  
  if (file.size > 5 * 1024 * 1024) {
    alert('Image must be less than 5MB')
    return
  }
  
  updating.value = true
  try {
    // Upload to Supabase Storage
    const fileExt = file.name.split('.').pop()
    const fileName = `${authStore.user?.id}-${Date.now()}.${fileExt}`
    const filePath = `avatars/${authStore.user?.id}/${fileName}`
    
    const { error: uploadError } = await supabase.storage
      .from('profiles')
      .upload(filePath, file, { upsert: true })
    
    if (uploadError) {
      if (uploadError.message.includes('Bucket not found')) {
        throw new Error('Storage bucket not configured. Please run the database migration script (supabase_migration_profile_updates.sql) in your Supabase SQL Editor.')
      }
      throw uploadError
    }
    
    // Get public URL
    const { data: urlData } = supabase.storage
      .from('profiles')
      .getPublicUrl(filePath)
    
    formData.profilePictureUrl = urlData.publicUrl
    
    // Update profile in database
    const { error: updateError } = await supabase
      .from('profiles')
      .update({
        profile_picture_url: formData.profilePictureUrl,
        updated_at: new Date().toISOString()
      })
      .eq('id', authStore.user!.id)
    
    if (updateError) throw updateError
    
    alert('Photo uploaded successfully!')
    await loadProfile()
  } catch (error: any) {
    console.error('Error uploading photo:', error)
    let errorMessage = error.message || 'Please try again.'
    
    if (errorMessage.includes('row-level security')) {
      errorMessage = 'Storage permissions not configured. Please run the migration script to set up storage policies.'
    }
    
    alert(`Failed to upload photo: ${errorMessage}`)
  } finally {
    updating.value = false
    // Reset file input
    if (target) target.value = ''
  }
}

const handleUnsubscribe = async () => {
  if (!authStore.user) return
  
  const confirmed = confirm(
    'Are you sure you want to unsubscribe? Your account will be deactivated and you will lose access to all services. You can reactivate later by logging in again.'
  )
  
  if (!confirmed) return
  
  updating.value = true
  try {
    const { error } = await supabase
      .from('profiles')
      .update({
        is_unsubscribed: true,
        unsubscribed_at: new Date().toISOString(),
        updated_at: new Date().toISOString()
      })
      .eq('id', authStore.user.id)
    
    if (error) throw error
    
    alert('Your account has been unsubscribed. You will be logged out.')
    await authStore.logout()
  } catch (error: any) {
    console.error('Error unsubscribing:', error)
    alert(`Failed to unsubscribe: ${error.message || 'Please try again.'}`)
  } finally {
    updating.value = false
  }
}

onMounted(() => {
  loadProfile()
})
</script>
