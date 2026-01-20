<template>
  <div class="min-h-screen flex items-center justify-center px-4">
    <div class="text-center">
      <h1 class="text-9xl font-bold text-primary-500">404</h1>
      <h2 class="text-3xl font-semibold text-gray-900 mt-4">Page Not Found</h2>
      <p class="text-gray-600 mt-2 mb-8">
        The page you're looking for doesn't exist or has been moved.
      </p>
      <RouterLink to="/" class="btn-primary">
        Go to Dashboard
      </RouterLink>
    </div>
  </div>
</template>

<script setup lang="ts">
import { RouterLink } from 'vue-router'
</script>
