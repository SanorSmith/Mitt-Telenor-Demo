<template>
  <div class="space-y-6">
    <h1 class="text-3xl font-bold text-gray-900">Usage Overview</h1>

    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="card">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900">Data Usage</h3>
          <Smartphone class="text-primary-500" :size="24" />
        </div>
        <div class="space-y-2">
          <div class="flex justify-between items-end">
            <span class="text-3xl font-bold text-gray-900">5.2 GB</span>
            <span class="text-sm text-gray-600">of 10 GB</span>
          </div>
          <div class="w-full bg-gray-200 rounded-full h-3">
            <div class="bg-primary-500 h-3 rounded-full transition-all" :style="{ width: '52%' }"></div>
          </div>
          <p class="text-sm text-gray-600">52% used • 4.8 GB remaining</p>
        </div>
      </div>

      <div class="card">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900">Voice Minutes</h3>
          <Phone class="text-green-500" :size="24" />
        </div>
        <div class="space-y-2">
          <div class="flex justify-between items-end">
            <span class="text-3xl font-bold text-gray-900">120</span>
            <span class="text-sm text-gray-600">of 300 min</span>
          </div>
          <div class="w-full bg-gray-200 rounded-full h-3">
            <div class="bg-green-500 h-3 rounded-full transition-all" :style="{ width: '40%' }"></div>
          </div>
          <p class="text-sm text-gray-600">40% used • 180 min remaining</p>
        </div>
      </div>

      <div class="card">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900">SMS</h3>
          <MessageSquare class="text-blue-500" :size="24" />
        </div>
        <div class="space-y-2">
          <div class="flex justify-between items-end">
            <span class="text-3xl font-bold text-gray-900">45</span>
            <span class="text-sm text-gray-600">of 100</span>
          </div>
          <div class="w-full bg-gray-200 rounded-full h-3">
            <div class="bg-blue-500 h-3 rounded-full transition-all" :style="{ width: '45%' }"></div>
          </div>
          <p class="text-sm text-gray-600">45% used • 55 remaining</p>
        </div>
      </div>
    </div>

    <div class="card">
      <div class="flex items-center justify-between mb-6">
        <h2 class="text-xl font-semibold text-gray-900">Usage Trends</h2>
        <select v-model="selectedPeriod" class="input w-auto">
          <option value="7">Last 7 days</option>
          <option value="30">Last 30 days</option>
          <option value="90">Last 90 days</option>
        </select>
      </div>
      <div class="h-80 bg-gray-50 rounded-lg p-6">
        <div class="h-full flex flex-col">
          <!-- Y-axis labels -->
          <div class="flex items-center mb-2">
            <div class="w-12 text-right text-xs text-gray-500 pr-2">High</div>
            <div class="flex-1 border-t border-gray-300"></div>
          </div>
          
          <!-- Chart area -->
          <div class="flex-1 flex items-end justify-between space-x-3">
            <div v-for="(day, index) in usageTrends" :key="index" class="flex-1 flex flex-col items-center justify-end space-y-2">
              <!-- Bars container -->
              <div class="w-full flex items-end justify-center space-x-1" style="height: 200px">
                <!-- Data bar -->
                <div class="flex-1 bg-primary-500 rounded-t transition-all hover:bg-primary-600 cursor-pointer relative group" 
                     :style="{ height: (day.data / maxUsage * 100) + '%' }">
                  <div class="absolute bottom-full mb-1 left-1/2 transform -translate-x-1/2 bg-gray-900 text-white text-xs rounded py-1 px-2 opacity-0 group-hover:opacity-100 transition-opacity whitespace-nowrap">
                    Data: {{ day.data }} GB
                  </div>
                </div>
                <!-- Voice bar -->
                <div class="flex-1 bg-green-500 rounded-t transition-all hover:bg-green-600 cursor-pointer relative group" 
                     :style="{ height: (day.voice / maxUsage * 80) + '%' }">
                  <div class="absolute bottom-full mb-1 left-1/2 transform -translate-x-1/2 bg-gray-900 text-white text-xs rounded py-1 px-2 opacity-0 group-hover:opacity-100 transition-opacity whitespace-nowrap">
                    Voice: {{ day.voice }} min
                  </div>
                </div>
                <!-- SMS bar -->
                <div class="flex-1 bg-blue-500 rounded-t transition-all hover:bg-blue-600 cursor-pointer relative group" 
                     :style="{ height: (day.sms / 50 * 100) + '%' }">
                  <div class="absolute bottom-full mb-1 left-1/2 transform -translate-x-1/2 bg-gray-900 text-white text-xs rounded py-1 px-2 opacity-0 group-hover:opacity-100 transition-opacity whitespace-nowrap">
                    SMS: {{ day.sms }}
                  </div>
                </div>
              </div>
              <!-- Day label -->
              <span class="text-xs font-medium text-gray-600">{{ day.label }}</span>
            </div>
          </div>
          
          <!-- X-axis line -->
          <div class="border-t border-gray-300 mt-2"></div>
        </div>
      </div>
      
      <!-- Legend -->
      <div class="mt-4 flex items-center justify-center space-x-6">
        <div class="flex items-center space-x-2">
          <div class="w-4 h-4 bg-primary-500 rounded"></div>
          <span class="text-sm text-gray-600">Data (GB)</span>
        </div>
        <div class="flex items-center space-x-2">
          <div class="w-4 h-4 bg-green-500 rounded"></div>
          <span class="text-sm text-gray-600">Voice (min)</span>
        </div>
        <div class="flex items-center space-x-2">
          <div class="w-4 h-4 bg-blue-500 rounded"></div>
          <span class="text-sm text-gray-600">SMS</span>
        </div>
      </div>
    </div>

    <div class="card">
      <h2 class="text-xl font-semibold text-gray-900 mb-4">Recent Activity</h2>
      <div class="space-y-3">
        <div
          v-for="(activity, index) in recentActivity"
          :key="index"
          class="flex items-center justify-between py-3 border-b border-gray-200 last:border-0"
        >
          <div class="flex items-center space-x-3">
            <div
              class="w-10 h-10 rounded-full flex items-center justify-center"
              :class="getActivityColor(activity.type)"
            >
              <component :is="getActivityIcon(activity.type)" :size="20" />
            </div>
            <div>
              <p class="font-medium text-gray-900">{{ activity.description }}</p>
              <p class="text-sm text-gray-600">{{ activity.amount }} • {{ activity.time }}</p>
            </div>
          </div>
          <span class="text-sm text-gray-500">{{ activity.date }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { Smartphone, Phone, MessageSquare, Wifi, PhoneCall, Mail } from 'lucide-vue-next'

const selectedPeriod = ref('7')

const usageTrends = ref([
  { label: 'Mon', data: 0.8, voice: 25, sms: 12 },
  { label: 'Tue', data: 1.2, voice: 35, sms: 18 },
  { label: 'Wed', data: 0.6, voice: 15, sms: 8 },
  { label: 'Thu', data: 1.5, voice: 42, sms: 22 },
  { label: 'Fri', data: 0.9, voice: 28, sms: 15 },
  { label: 'Sat', data: 1.8, voice: 48, sms: 30 },
  { label: 'Sun', data: 1.3, voice: 38, sms: 20 }
])

const maxUsage = computed(() => {
  return Math.max(...usageTrends.value.map(d => d.data))
})

const recentActivity = ref([
  {
    type: 'data',
    description: 'Data Usage',
    amount: '250 MB',
    time: '2:30 PM',
    date: 'Today'
  },
  {
    type: 'voice',
    description: 'Voice Call',
    amount: '15 minutes',
    time: '11:45 AM',
    date: 'Today'
  },
  {
    type: 'sms',
    description: 'SMS Sent',
    amount: '3 messages',
    time: '9:20 AM',
    date: 'Today'
  },
  {
    type: 'data',
    description: 'Data Usage',
    amount: '180 MB',
    time: '8:15 PM',
    date: 'Yesterday'
  }
])

const getActivityIcon = (type: string) => {
  switch (type) {
    case 'data':
      return Wifi
    case 'voice':
      return PhoneCall
    case 'sms':
      return Mail
    default:
      return Smartphone
  }
}

const getActivityColor = (type: string) => {
  switch (type) {
    case 'data':
      return 'bg-primary-100 text-primary-600'
    case 'voice':
      return 'bg-green-100 text-green-600'
    case 'sms':
      return 'bg-blue-100 text-blue-600'
    default:
      return 'bg-gray-100 text-gray-600'
  }
}
</script>
