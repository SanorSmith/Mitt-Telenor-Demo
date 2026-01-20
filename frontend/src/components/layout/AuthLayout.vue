<template>
  <div class="min-h-screen bg-gradient-to-br from-primary-500 to-primary-700 flex items-center justify-center px-4">
    <div class="max-w-md w-full">
      <div class="text-center mb-8">
        <h1 class="text-4xl font-bold text-white mb-2">Mitt Telenor</h1>
        <p class="text-primary-100">Self-Service Platform</p>
      </div>
      <div class="bg-white rounded-lg shadow-xl p-8">
        <RouterView />
      </div>
      <p class="text-center text-primary-100 text-sm mt-6">
        &copy; 2024 Telenor. All rights reserved.
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { RouterView } from 'vue-router'
</script>
