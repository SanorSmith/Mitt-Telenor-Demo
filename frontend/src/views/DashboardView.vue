<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-3xl font-bold text-gray-900">Welcome back, {{ user?.firstName }}!</h1>
        <p class="text-gray-600 mt-1">Here's your account overview</p>
      </div>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-3 gap-6" role="region" aria-label="Usage statistics">
      <div class="card">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900">Data Usage</h3>
          <Smartphone class="text-primary-500" :size="24" aria-hidden="true" />
        </div>
        <div class="space-y-2">
          <div class="flex justify-between items-end">
            <span class="text-3xl font-bold text-gray-900">5.2 GB</span>
            <span class="text-sm text-gray-600">of 10 GB</span>
          </div>
          <div class="w-full bg-gray-200 rounded-full h-2" role="progressbar" aria-valuenow="52" aria-valuemin="0" aria-valuemax="100" aria-label="Data usage: 52% of 10 GB used">
            <div class="bg-primary-500 h-2 rounded-full" style="width: 52%"></div>
          </div>
          <p class="text-sm text-gray-600">52% used</p>
        </div>
      </div>

      <div class="card">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900">Voice Minutes</h3>
          <Phone class="text-green-500" :size="24" aria-hidden="true" />
        </div>
        <div class="space-y-2">
          <div class="flex justify-between items-end">
            <span class="text-3xl font-bold text-gray-900">120</span>
            <span class="text-sm text-gray-600">of 300 min</span>
          </div>
          <div class="w-full bg-gray-200 rounded-full h-2" role="progressbar" aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" aria-label="Voice minutes: 40% of 300 minutes used">
            <div class="bg-green-500 h-2 rounded-full" style="width: 40%"></div>
          </div>
          <p class="text-sm text-gray-600">40% used</p>
        </div>
      </div>

      <div class="card">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900">SMS</h3>
          <MessageSquare class="text-blue-500" :size="24" aria-hidden="true" />
        </div>
        <div class="space-y-2">
          <div class="flex justify-between items-end">
            <span class="text-3xl font-bold text-gray-900">45</span>
            <span class="text-sm text-gray-600">of 100</span>
          </div>
          <div class="w-full bg-gray-200 rounded-full h-2" role="progressbar" aria-valuenow="45" aria-valuemin="0" aria-valuemax="100" aria-label="SMS messages: 45% of 100 messages used">
            <div class="bg-blue-500 h-2 rounded-full" style="width: 45%"></div>
          </div>
          <p class="text-sm text-gray-600">45% used</p>
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <div class="card">
        <h3 class="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h3>
        <nav class="grid grid-cols-2 gap-4" aria-label="Quick actions">
          <RouterLink
            to="/subscriptions"
            class="flex flex-col items-center justify-center p-4 border-2 border-gray-200 rounded-lg hover:border-primary-500 hover:bg-primary-50 transition-colors focus-visible-ring"
            aria-label="Manage subscription plans"
          >
            <CreditCard class="text-primary-500 mb-2" :size="32" aria-hidden="true" />
            <span class="text-sm font-medium text-gray-900">Manage Plans</span>
          </RouterLink>

          <RouterLink
            to="/billing"
            class="flex flex-col items-center justify-center p-4 border-2 border-gray-200 rounded-lg hover:border-primary-500 hover:bg-primary-50 transition-colors focus-visible-ring"
            aria-label="Pay your bill"
          >
            <Receipt class="text-primary-500 mb-2" :size="32" aria-hidden="true" />
            <span class="text-sm font-medium text-gray-900">Pay Bill</span>
          </RouterLink>

          <RouterLink
            to="/usage"
            class="flex flex-col items-center justify-center p-4 border-2 border-gray-200 rounded-lg hover:border-primary-500 hover:bg-primary-50 transition-colors focus-visible-ring"
            aria-label="View detailed usage statistics"
          >
            <BarChart3 class="text-primary-500 mb-2" :size="32" aria-hidden="true" />
            <span class="text-sm font-medium text-gray-900">View Usage</span>
          </RouterLink>

          <RouterLink
            to="/support"
            class="flex flex-col items-center justify-center p-4 border-2 border-gray-200 rounded-lg hover:border-primary-500 hover:bg-primary-50 transition-colors focus-visible-ring"
            aria-label="Get help and support"
          >
            <HelpCircle class="text-primary-500 mb-2" :size="32" aria-hidden="true" />
            <span class="text-sm font-medium text-gray-900">Get Help</span>
          </RouterLink>
        </nav>
      </div>

      <div class="card">
        <h3 class="text-lg font-semibold text-gray-900 mb-4">Recent Activity</h3>
        <div class="space-y-3">
          <div class="flex items-start space-x-3 pb-3 border-b border-gray-200">
            <div class="w-8 h-8 bg-green-100 rounded-full flex items-center justify-center flex-shrink-0">
              <CheckCircle class="text-green-600" :size="16" />
            </div>
            <div class="flex-1">
              <p class="text-sm font-medium text-gray-900">Payment Received</p>
              <p class="text-xs text-gray-600">Invoice #INV-2024-001 paid successfully</p>
              <p class="text-xs text-gray-500 mt-1">2 days ago</p>
            </div>
          </div>

          <div class="flex items-start space-x-3 pb-3 border-b border-gray-200">
            <div class="w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center flex-shrink-0">
              <AlertCircle class="text-blue-600" :size="16" />
            </div>
            <div class="flex-1">
              <p class="text-sm font-medium text-gray-900">Usage Alert</p>
              <p class="text-xs text-gray-600">You've used 80% of your data allowance</p>
              <p class="text-xs text-gray-500 mt-1">5 days ago</p>
            </div>
          </div>

          <div class="flex items-start space-x-3">
            <div class="w-8 h-8 bg-purple-100 rounded-full flex items-center justify-center flex-shrink-0">
              <Zap class="text-purple-600" :size="16" />
            </div>
            <div class="flex-1">
              <p class="text-sm font-medium text-gray-900">Plan Upgraded</p>
              <p class="text-xs text-gray-600">Switched to Premium 10GB plan</p>
              <p class="text-xs text-gray-500 mt-1">1 week ago</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import {
  Smartphone,
  Phone,
  MessageSquare,
  CreditCard,
  Receipt,
  BarChart3,
  HelpCircle,
  CheckCircle,
  AlertCircle,
  Zap
} from 'lucide-vue-next'
import { useAuthStore } from '@/stores/auth'
import { supabase } from '@/lib/supabase'

const authStore = useAuthStore()
const user = ref<any>({ firstName: 'User', lastName: '' })

const loadUserProfile = async () => {
  if (!authStore.user) return
  
  try {
    const { data, error } = await supabase
      .from('profiles')
      .select('first_name, last_name, email')
      .eq('id', authStore.user.id)
      .single()
    
    if (error) throw error
    
    if (data) {
      user.value = {
        firstName: data.first_name || 'User',
        lastName: data.last_name || '',
        email: data.email || authStore.user.email
      }
    }
  } catch (error) {
    console.error('Error loading profile:', error)
  }
}

onMounted(() => {
  loadUserProfile()
})
</script>
