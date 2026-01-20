<template>
  <div class="flex items-center justify-center" :class="containerClass">
    <div
      class="animate-spin rounded-full border-t-2 border-b-2"
      :class="[sizeClass, colorClass]"
      role="status"
      aria-label="Loading"
    >
      <span class="sr-only">Loading...</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  size?: 'sm' | 'md' | 'lg'
  color?: 'primary' | 'white' | 'gray'
  fullScreen?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  size: 'md',
  color: 'primary',
  fullScreen: false
})

const sizeClass = computed(() => {
  const sizes = {
    sm: 'w-6 h-6',
    md: 'w-12 h-12',
    lg: 'w-16 h-16'
  }
  return sizes[props.size]
})

const colorClass = computed(() => {
  const colors = {
    primary: 'border-primary-500',
    white: 'border-white',
    gray: 'border-gray-500'
  }
  return colors[props.color]
})

const containerClass = computed(() => {
  return props.fullScreen ? 'min-h-screen' : 'py-8'
})
</script>
