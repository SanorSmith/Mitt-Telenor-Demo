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
            <div class="w-32 h-32 rounded-full bg-primary-100 flex items-center justify-center mb-4">
              <User :size="64" class="text-primary-500" />
            </div>
            <button class="btn-secondary text-sm">
              Upload Photo
            </button>
          </div>
        </div>

        <div class="card">
          <h2 class="text-xl font-semibold text-gray-900 mb-4">Account Stats</h2>
          <div class="space-y-3">
            <div class="flex justify-between">
              <span class="text-gray-600">Member Since</span>
              <span class="font-medium">Jan 2024</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600">Account Status</span>
              <span class="font-medium text-green-600">Active</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600">Total Invoices</span>
              <span class="font-medium">12</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { User } from 'lucide-vue-next'
import { useAuthStore } from '@/stores/auth'
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
  dateOfBirth: ''
})

const loadProfile = () => {
  if (authStore.user) {
    formData.firstName = authStore.user.firstName
    formData.lastName = authStore.user.lastName
    formData.email = authStore.user.email
    formData.phone = authStore.user.phone || ''
  }
}

const resetForm = () => {
  loadProfile()
}

const handleUpdateProfile = async () => {
  updating.value = true
  setTimeout(() => {
    updating.value = false
  }, 1000)
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

onMounted(() => {
  loadProfile()
})
</script>
