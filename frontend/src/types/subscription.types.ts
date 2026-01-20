export interface Plan {
  id: string
  name: string
  type: 'mobile' | 'broadband' | 'tv'
  price: number
  dataGb?: number
  voiceMinutes?: number
  smsCount?: number
  features?: string[]
  isPopular?: boolean
}

export interface Subscription {
  id: string
  userId: string
  planId: string
  plan?: Plan
  status: 'active' | 'suspended' | 'cancelled'
  startDate: string
  endDate?: string
  autoRenew: boolean
  addOns?: AddOn[]
}

export interface AddOn {
  id: string
  subscriptionId: string
  name: string
  price: number
}
