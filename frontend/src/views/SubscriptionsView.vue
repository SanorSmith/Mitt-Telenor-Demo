<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <h1 class="text-3xl font-bold text-gray-900">Subscriptions</h1>
      <button v-if="!currentSubscription" class="btn-primary">
        Get Started
      </button>
    </div>

    <div v-if="currentSubscription" class="card bg-gradient-to-r from-primary-500 to-primary-700 text-white">
      <div class="flex items-center justify-between">
        <div>
          <h2 class="text-2xl font-bold mb-2">{{ currentSubscription.plan.name }}</h2>
          <p class="text-primary-100">{{ currentSubscription.plan.description }}</p>
        </div>
        <div class="text-right">
          <p class="text-3xl font-bold">{{ currentSubscription.plan.price }} SEK</p>
          <p class="text-primary-100">per month</p>
        </div>
      </div>

      <div class="grid grid-cols-3 gap-4 mt-6 pt-6 border-t border-primary-400">
        <div>
          <p class="text-primary-100 text-sm">Data</p>
          <p class="text-xl font-bold">{{ currentSubscription.plan.dataAllowanceGB }} GB</p>
        </div>
        <div>
          <p class="text-primary-100 text-sm">Voice</p>
          <p class="text-xl font-bold">{{ currentSubscription.plan.voiceMinutes }} min</p>
        </div>
        <div>
          <p class="text-primary-100 text-sm">SMS</p>
          <p class="text-xl font-bold">{{ currentSubscription.plan.smsCount }}</p>
        </div>
      </div>

      <div class="flex justify-end mt-6">
        <button @click="showChangePlanDialog" class="bg-white text-primary-600 px-4 py-2 rounded-lg font-medium hover:bg-primary-50 transition-colors">
          Change Plan
        </button>
      </div>
    </div>

    <div>
      <h2 class="text-2xl font-bold text-gray-900 mb-4">Available Plans</h2>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div
          v-for="plan in availablePlans"
          :key="plan.id"
          class="card hover:shadow-lg transition-shadow"
          :class="{ 'ring-2 ring-primary-500': currentSubscription?.plan.id === plan.id }"
        >
          <div class="text-center mb-4">
            <h3 class="text-xl font-bold text-gray-900">{{ plan.name }}</h3>
            <p class="text-gray-600 text-sm mt-1">{{ plan.description }}</p>
          </div>

          <div class="text-center mb-6">
            <p class="text-4xl font-bold text-gray-900">{{ plan.price }}</p>
            <p class="text-gray-600">SEK/month</p>
          </div>

          <div class="space-y-3 mb-6">
            <div class="flex items-center text-sm">
              <CheckCircle class="text-green-500 mr-2" :size="16" />
              <span>{{ plan.dataAllowanceGB }} GB Data</span>
            </div>
            <div class="flex items-center text-sm">
              <CheckCircle class="text-green-500 mr-2" :size="16" />
              <span>{{ plan.voiceMinutes }} Voice Minutes</span>
            </div>
            <div class="flex items-center text-sm">
              <CheckCircle class="text-green-500 mr-2" :size="16" />
              <span>{{ plan.smsCount }} SMS</span>
            </div>
          </div>

          <button
            v-if="currentSubscription?.plan.id === plan.id"
            class="btn-secondary w-full"
            disabled
          >
            Current Plan
          </button>
          <button
            v-else
            @click="selectPlan(plan)"
            class="btn-primary w-full"
          >
            Select Plan
          </button>
        </div>
      </div>
    </div>

    <div v-if="availableAddOns.length > 0">
      <h2 class="text-2xl font-bold text-gray-900 mb-4">Add-Ons</h2>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div
          v-for="addOn in availableAddOns"
          :key="addOn.id"
          class="card flex items-center justify-between"
        >
          <div>
            <h3 class="font-semibold text-gray-900">{{ addOn.name }}</h3>
            <p class="text-sm text-gray-600">{{ addOn.description }}</p>
            <p class="text-lg font-bold text-primary-600 mt-2">{{ addOn.price }} SEK/month</p>
          </div>
          <button @click="addAddOn(addOn)" class="btn-primary">
            Add
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { CheckCircle } from 'lucide-vue-next'

const currentSubscription = ref<any>(null)
const availablePlans = ref([
  {
    id: '1',
    name: 'Basic 5GB',
    description: 'Perfect for light users',
    price: 199,
    dataAllowanceGB: 5,
    voiceMinutes: 200,
    smsCount: 100
  },
  {
    id: '2',
    name: 'Standard 10GB',
    description: 'Great for everyday use',
    price: 299,
    dataAllowanceGB: 10,
    voiceMinutes: 500,
    smsCount: 200
  },
  {
    id: '3',
    name: 'Premium 20GB',
    description: 'Unlimited possibilities',
    price: 499,
    dataAllowanceGB: 20,
    voiceMinutes: 1000,
    smsCount: 500
  }
])

const availableAddOns = ref([
  {
    id: '1',
    name: 'Extra 5GB Data',
    description: 'Additional 5GB data allowance',
    price: 99
  },
  {
    id: '2',
    name: 'International Calls',
    description: '100 minutes international calls',
    price: 149
  }
])

const selectPlan = (plan: any) => {
  if (confirm(`Switch to ${plan.name} for ${plan.price} SEK/month?`)) {
    currentSubscription.value = { plan }
    alert('Plan changed successfully!')
  }
}

const showChangePlanDialog = () => {
  alert('Please select a new plan from the Available Plans section below.')
}

const addAddOn = (addOn: any) => {
  if (confirm(`Add ${addOn.name} for ${addOn.price} SEK/month?`)) {
    alert('Add-on added successfully!')
  }
}

onMounted(() => {
  currentSubscription.value = {
    plan: availablePlans.value[1]
  }
})
</script>
