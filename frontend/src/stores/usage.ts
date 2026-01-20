import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { UsageData, UsageMetric } from '@/types/usage.types'

export const useUsageStore = defineStore('usage', () => {
  const currentUsage = ref<UsageData | null>(null)
  const dailyUsage = ref<UsageMetric[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  const fetchCurrentUsage = async () => {
    loading.value = true
    error.value = null
    try {
      currentUsage.value = {
        period: new Date().toISOString().slice(0, 7),
        data: {
          used: 5324,
          total: 10240,
          unit: 'MB'
        },
        voice: {
          used: 120,
          total: 500,
          unit: 'minutes'
        },
        sms: {
          used: 45,
          total: 200,
          unit: 'messages'
        },
        lastUpdated: new Date()
      }
    } catch (err) {
      error.value = 'Failed to fetch usage data'
      console.error(err)
    } finally {
      loading.value = false
    }
  }

  const fetchDailyUsage = async (days: number = 30) => {
    loading.value = true
    error.value = null
    try {
      const metrics: UsageMetric[] = []
      const now = new Date()
      
      for (let i = days - 1; i >= 0; i--) {
        const date = new Date(now)
        date.setDate(date.getDate() - i)
        
        metrics.push({
          date: date.toISOString().split('T')[0],
          data: Math.floor(Math.random() * 500) + 100,
          voice: Math.floor(Math.random() * 30) + 5,
          sms: Math.floor(Math.random() * 10) + 1
        })
      }
      
      dailyUsage.value = metrics
    } catch (err) {
      error.value = 'Failed to fetch daily usage'
      console.error(err)
    } finally {
      loading.value = false
    }
  }

  const getUsagePercentage = (type: 'data' | 'voice' | 'sms'): number => {
    if (!currentUsage.value) return 0
    
    const usage = currentUsage.value[type]
    return Math.round((usage.used / usage.total) * 100)
  }

  const getRemainingAmount = (type: 'data' | 'voice' | 'sms'): number => {
    if (!currentUsage.value) return 0
    
    const usage = currentUsage.value[type]
    return usage.total - usage.used
  }

  return {
    currentUsage,
    dailyUsage,
    loading,
    error,
    fetchCurrentUsage,
    fetchDailyUsage,
    getUsagePercentage,
    getRemainingAmount
  }
})
