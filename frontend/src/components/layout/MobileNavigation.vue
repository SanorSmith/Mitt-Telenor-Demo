<template>
  <nav class="fixed bottom-0 left-0 right-0 bg-white border-t border-gray-200 md:hidden z-50">
    <div class="flex justify-around items-center h-16">
      <RouterLink
        v-for="item in navItems"
        :key="item.name"
        :to="item.path"
        class="flex flex-col items-center justify-center flex-1 h-full transition-colors"
        :class="isActive(item.path) ? 'text-primary-500' : 'text-gray-600'"
        active-class="text-primary-500"
      >
        <component :is="item.icon" :size="24" />
        <span class="text-xs mt-1">{{ item.name }}</span>
      </RouterLink>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router'
import { Home, CreditCard, BarChart3, Receipt, HelpCircle } from 'lucide-vue-next'

const route = useRoute()

const navItems = [
  { name: 'Home', path: '/', icon: Home },
  { name: 'Subscriptions', path: '/subscriptions', icon: CreditCard },
  { name: 'Usage', path: '/usage', icon: BarChart3 },
  { name: 'Billing', path: '/billing', icon: Receipt },
  { name: 'Support', path: '/support', icon: HelpCircle }
]

const isActive = (path: string) => {
  return route.path === path
}
</script>
