<template>
  <div>
    <h2 class="text-2xl font-bold text-gray-900 mb-6">Create Account</h2>

    <form @submit.prevent="handleSubmit" class="space-y-4">
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
            :class="{ 'border-red-500': errors.firstName }"
            placeholder="John"
            autocomplete="given-name"
          />
          <p v-if="errors.firstName" class="mt-1 text-sm text-red-600">{{ errors.firstName }}</p>
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
            :class="{ 'border-red-500': errors.lastName }"
            placeholder="Doe"
            autocomplete="family-name"
          />
          <p v-if="errors.lastName" class="mt-1 text-sm text-red-600">{{ errors.lastName }}</p>
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
          required
          class="input"
          :class="{ 'border-red-500': errors.email }"
          placeholder="your.email@example.com"
          autocomplete="email"
        />
        <p v-if="errors.email" class="mt-1 text-sm text-red-600">{{ errors.email }}</p>
      </div>

      <div>
        <label for="phone" class="block text-sm font-medium text-gray-700 mb-1">
          Phone Number (Optional)
        </label>
        <input
          id="phone"
          v-model="formData.phone"
          type="tel"
          class="input"
          :class="{ 'border-red-500': errors.phone }"
          placeholder="+47 123 45 678"
          autocomplete="tel"
        />
        <p v-if="errors.phone" class="mt-1 text-sm text-red-600">{{ errors.phone }}</p>
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
          placeholder="Create a strong password"
          autocomplete="new-password"
        />
        <p class="mt-1 text-xs text-gray-500">
          Must be at least 8 characters with uppercase, lowercase, number, and special character
        </p>
        <p v-if="errors.password" class="mt-1 text-sm text-red-600">{{ errors.password }}</p>
      </div>

      <div v-if="errorMessage" class="p-3 bg-red-50 border border-red-200 rounded-lg">
        <p class="text-sm text-red-600">{{ errorMessage }}</p>
      </div>

      <button
        type="submit"
        :disabled="loading"
        class="btn-primary w-full"
      >
        <span v-if="!loading">Create Account</span>
        <span v-else class="flex items-center justify-center">
          <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          Creating account...
        </span>
      </button>
    </form>

    <p class="mt-6 text-center text-sm text-gray-600">
      Already have an account?
      <RouterLink to="/auth/login" class="text-primary-500 hover:text-primary-600 font-medium">
        Sign in
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
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  password: ''
})

const errors = reactive({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  password: ''
})

const validateForm = () => {
  Object.keys(errors).forEach(key => {
    errors[key as keyof typeof errors] = ''
  })

  let isValid = true

  if (!formData.firstName || formData.firstName.length < 2) {
    errors.firstName = 'First name must be at least 2 characters'
    isValid = false
  }

  if (!formData.lastName || formData.lastName.length < 2) {
    errors.lastName = 'Last name must be at least 2 characters'
    isValid = false
  }

  if (!formData.email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
    errors.email = 'Please enter a valid email address'
    isValid = false
  }

  const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/
  if (!formData.password || !passwordRegex.test(formData.password)) {
    errors.password = 'Password must meet all requirements'
    isValid = false
  }

  return isValid
}

const handleSubmit = async () => {
  if (!validateForm()) return

  loading.value = true
  errorMessage.value = ''

  const result = await authStore.register({
    email: formData.email,
    password: formData.password,
    firstName: formData.firstName,
    lastName: formData.lastName,
    phone: formData.phone || undefined
  })

  loading.value = false

  if (result.success) {
    router.push({ name: 'dashboard' })
  } else {
    errorMessage.value = result.error || 'Registration failed. Please try again.'
  }
}
</script>
