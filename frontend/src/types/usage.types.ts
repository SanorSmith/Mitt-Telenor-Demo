export interface UsageData {
  data: UsageMetric
  voice: UsageMetric
  sms: UsageMetric
}

export interface UsageMetric {
  used: number
  total: number
  unit: string
  percentage?: number
}

export interface UsageRecord {
  userId: string
  timestamp: number
  date: string
  dataUsedMB: number
  voiceMinutes: number
  smsCount: number
  type: 'data' | 'voice' | 'sms'
}

export interface UsageAlert {
  id: string
  userId: string
  type: 'data' | 'voice' | 'sms'
  threshold: number
  enabled: boolean
}
