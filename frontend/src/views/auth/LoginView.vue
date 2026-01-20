<template>
  <div>
    <h2 class="text-2xl font-bold text-gray-900 mb-6">Welcome Back</h2>

    <form @submit.prevent="handleSubmit" class="space-y-4">
      <div>
        <label for="email" class="block text-sm font-medium text-gray-700 mb-1">
          Email Address
        </label>
        <input
          id="email"
          v-model="formData.email"
          type="email"
          required
          class="input"
          :class="{ 'border-red-500': errors.email }"
          placeholder="your.email@example.com"
          autocomplete="email"
        />
        <p v-if="errors.email" class="mt-1 text-sm text-red-600">{{ errors.email }}</p>
      </div>

      <div>
        <label for="password" class="block text-sm font-medium text-gray-700 mb-1">
          Password
        </label>
        <input
          id="password"
          v-model="formData.password"
          type="password"
          required
          class="input"
          :class="{ 'border-red-500': errors.password }"
          placeholder="Enter your password"
          autocomplete="current-password"
        />
        <p v-if="errors.password" class="mt-1 text-sm text-red-600">{{ errors.password }}</p>
      </div>

      <div class="flex items-center justify-between">
        <label class="flex items-center">
          <input type="checkbox" class="rounded border-gray-300 text-primary-500 focus:ring-primary-500" />
          <span class="ml-2 text-sm text-gray-600">Remember me</span>
        </label>
        <RouterLink to="/auth/forgot-password" class="text-sm text-primary-500 hover:text-primary-600">
          Forgot password?
        </RouterLink>
      </div>

      <div v-if="errorMessage" class="p-3 bg-red-50 border border-red-200 rounded-lg">
        <p class="text-sm text-red-600">{{ errorMessage }}</p>
      </div>

      <button
        type="submit"
        :disabled="loading"
        class="btn-primary w-full"
      >
        <span v-if="!loading">Sign In</span>
        <span v-else class="flex items-center justify-center">
          <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          Signing in...
        </span>
      </button>
    </form>

    <p class="mt-6 text-center text-sm text-gray-600">
      Don't have an account?
      <RouterLink to="/auth/register" class="text-primary-500 hover:text-primary-600 font-medium">
        Sign up
      </RouterLink>
    </p>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter, RouterLink } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const loading = ref(false)
const errorMessage = ref('')

const formData = reactive({
  email: '',
  password: ''
})

const errors = reactive({
  email: '',
  password: ''
})

const validateForm = () => {
  errors.email = ''
  errors.password = ''

  if (!formData.email) {
    errors.email = 'Email is required'
    return false
  }

  if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
    errors.email = 'Please enter a valid email address'
    return false
  }

  if (!formData.password) {
    errors.password = 'Password is required'
    return false
  }

  if (formData.password.length < 8) {
    errors.password = 'Password must be at least 8 characters'
    return false
  }

  return true
}

const handleSubmit = async () => {
  if (!validateForm()) return

  loading.value = true
  errorMessage.value = ''

  const result = await authStore.login({
    email: formData.email,
    password: formData.password
  })

  loading.value = false

  if (result.success) {
    router.push({ name: 'dashboard' })
  } else {
    errorMessage.value = result.error || 'Login failed. Please try again.'
  }
}
</script>
