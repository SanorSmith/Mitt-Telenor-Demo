import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Subscription, Plan, AddOn } from '@/types/subscription.types'

export const useSubscriptionStore = defineStore('subscription', () => {
  const currentSubscription = ref<Subscription | null>(null)
  const availablePlans = ref<Plan[]>([])
  const availableAddOns = ref<AddOn[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  const fetchCurrentSubscription = async () => {
    loading.value = true
    error.value = null
    try {
      currentSubscription.value = {
        id: '1',
        userId: '1',
        plan: {
          id: '2',
          name: 'Standard 10GB',
          description: 'Great for everyday use',
          price: 299,
          dataAllowanceGB: 10,
          voiceMinutes: 500,
          smsCount: 200
        },
        status: 'Active',
        startDate: new Date('2024-01-01'),
        billingCycle: 'Monthly',
        addOns: []
      }
    } catch (err) {
      error.value = 'Failed to fetch subscription'
      console.error(err)
    } finally {
      loading.value = false
    }
  }

  const fetchAvailablePlans = async () => {
    loading.value = true
    error.value = null
    try {
      availablePlans.value = [
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
      ]
    } catch (err) {
      error.value = 'Failed to fetch plans'
      console.error(err)
    } finally {
      loading.value = false
    }
  }

  const fetchAvailableAddOns = async () => {
    loading.value = true
    error.value = null
    try {
      availableAddOns.value = [
        {
          id: '1',
          name: 'Extra 5GB Data',
          description: 'Additional 5GB data allowance',
          price: 99,
          type: 'Data',
          value: 5
        },
        {
          id: '2',
          name: 'International Calls',
          description: '100 minutes international calls',
          price: 149,
          type: 'Voice',
          value: 100
        }
      ]
    } catch (err) {
      error.value = 'Failed to fetch add-ons'
      console.error(err)
    } finally {
      loading.value = false
    }
  }

  const changePlan = async (planId: string) => {
    loading.value = true
    error.value = null
    try {
      const newPlan = availablePlans.value.find(p => p.id === planId)
      if (newPlan && currentSubscription.value) {
        currentSubscription.value.plan = newPlan
      }
      return { success: true }
    } catch (err) {
      error.value = 'Failed to change plan'
      console.error(err)
      return { success: false, error: error.value }
    } finally {
      loading.value = false
    }
  }

  const addAddOn = async (addOnId: string) => {
    loading.value = true
    error.value = null
    try {
      const addOn = availableAddOns.value.find(a => a.id === addOnId)
      if (addOn && currentSubscription.value) {
        currentSubscription.value.addOns.push(addOn)
      }
      return { success: true }
    } catch (err) {
      error.value = 'Failed to add add-on'
      console.error(err)
      return { success: false, error: error.value }
    } finally {
      loading.value = false
    }
  }

  const removeAddOn = async (addOnId: string) => {
    loading.value = true
    error.value = null
    try {
      if (currentSubscription.value) {
        currentSubscription.value.addOns = currentSubscription.value.addOns.filter(
          a => a.id !== addOnId
        )
      }
      return { success: true }
    } catch (err) {
      error.value = 'Failed to remove add-on'
      console.error(err)
      return { success: false, error: error.value }
    } finally {
      loading.value = false
    }
  }

  return {
    currentSubscription,
    availablePlans,
    availableAddOns,
    loading,
    error,
    fetchCurrentSubscription,
    fetchAvailablePlans,
    fetchAvailableAddOns,
    changePlan,
    addAddOn,
    removeAddOn
  }
})
