<template>
  <div>
    <h2 class="text-2xl font-bold text-gray-900 mb-2">Reset Password</h2>
    <p class="text-gray-600 mb-6">
      Enter your email address and we'll send you a link to reset your password.
    </p>

    <form v-if="!submitted" @submit.prevent="handleSubmit" class="space-y-4">
      <div>
        <label for="email" class="block text-sm font-medium text-gray-700 mb-1">
          Email Address
        </label>
        <input
          id="email"
          v-model="email"
          type="email"
          required
          class="input"
          :class="{ 'border-red-500': error }"
          placeholder="your.email@example.com"
          autocomplete="email"
        />
        <p v-if="error" class="mt-1 text-sm text-red-600">{{ error }}</p>
      </div>

      <button
        type="submit"
        :disabled="loading"
        class="btn-primary w-full"
      >
        <span v-if="!loading">Send Reset Link</span>
        <span v-else>Sending...</span>
      </button>
    </form>

    <div v-else class="text-center py-4">
      <div class="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
        <svg class="w-8 h-8 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
        </svg>
      </div>
      <h3 class="text-lg font-semibold text-gray-900 mb-2">Check Your Email</h3>
      <p class="text-gray-600 mb-4">
        We've sent a password reset link to {{ email }}
      </p>
    </div>

    <p class="mt-6 text-center text-sm text-gray-600">
      Remember your password?
      <RouterLink to="/auth/login" class="text-primary-500 hover:text-primary-600 font-medium">
        Sign in
      </RouterLink>
    </p>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink } from 'vue-router'

const email = ref('')
const loading = ref(false)
const error = ref('')
const submitted = ref(false)

const handleSubmit = async () => {
  error.value = ''

  if (!email.value || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value)) {
    error.value = 'Please enter a valid email address'
    return
  }

  loading.value = true

  setTimeout(() => {
    loading.value = false
    submitted.value = true
  }, 1500)
}
</script>
