<template>
  <header class="bg-white shadow-sm sticky top-0 z-40">
    <div class="container mx-auto px-4 max-w-7xl">
      <div class="flex items-center justify-between h-16">
        <div class="flex items-center space-x-8">
          <RouterLink to="/" class="flex items-center space-x-2">
            <div class="w-8 h-8 bg-primary-500 rounded-lg flex items-center justify-center">
              <span class="text-white font-bold text-lg">T</span>
            </div>
            <span class="text-xl font-bold text-gray-900 hidden sm:block">Mitt Telenor</span>
          </RouterLink>

          <nav class="hidden md:flex space-x-6" role="navigation" aria-label="Main navigation">
            <RouterLink
              v-for="item in navItems"
              :key="item.name"
              :to="item.path"
              class="text-gray-600 hover:text-primary-500 transition-colors font-medium"
              :class="{ 'text-primary-500': $route.path === item.path }"
              exact
            >
              {{ item.name }}
            </RouterLink>
          </nav>
        </div>

        <div class="flex items-center space-x-4">
          <button
            v-if="updateAvailable"
            @click="updateApp"
            class="text-sm text-primary-500 hover:text-primary-600 font-medium"
            aria-label="Update application"
          >
            Update Available
          </button>

          <RouterLink
            to="/profile"
            class="flex items-center space-x-2 text-gray-700 hover:text-primary-500 transition-colors"
            aria-label="User profile"
          >
            <User :size="20" />
            <span class="hidden sm:block">{{ user?.firstName }}</span>
          </RouterLink>

          <button
            @click="handleLogout"
            class="text-gray-600 hover:text-red-500 transition-colors"
            aria-label="Logout"
          >
            <LogOut :size="20" />
          </button>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { User, LogOut } from 'lucide-vue-next'
import { useAuthStore } from '@/stores/auth'
import { usePWA } from '@/composables/usePWA'

const router = useRouter()
const authStore = useAuthStore()
const { updateAvailable, updateApp } = usePWA()

const user = computed(() => authStore.user)

const navItems = [
  { name: 'Dashboard', path: '/' },
  { name: 'Subscriptions', path: '/subscriptions' },
  { name: 'Usage', path: '/usage' },
  { name: 'Billing', path: '/billing' },
  { name: 'Support', path: '/support' }
]

const handleLogout = async () => {
  await authStore.logout()
  router.push({ name: 'login' })
}
</script>
