<template>
  <div class="min-h-screen bg-gray-50">
    <AppHeader />
    <main class="container mx-auto px-4 py-8 max-w-7xl">
      <RouterView />
    </main>
    <AppFooter />
    <MobileNavigation v-if="isMobile" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { RouterView } from 'vue-router'
import AppHeader from '@/components/common/AppHeader.vue'
import AppFooter from '@/components/common/AppFooter.vue'
import MobileNavigation from './MobileNavigation.vue'

const isMobile = ref(false)

const checkMobile = () => {
  isMobile.value = window.innerWidth < 768
}

onMounted(() => {
  checkMobile()
  window.addEventListener('resize', checkMobile)
})

onUnmounted(() => {
  window.removeEventListener('resize', checkMobile)
})
</script>
