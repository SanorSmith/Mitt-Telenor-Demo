export interface Invoice {
  id: string
  userId: string
  invoiceNumber: string
  amount: number
  tax: number
  total: number
  dueDate: string
  status: 'pending' | 'paid' | 'overdue'
  pdfUrl?: string
  createdAt: string
}

export interface Payment {
  id: string
  invoiceId: string
  amount: number
  paymentMethod: string
  transactionId?: string
  status: string
  paidAt?: string
  createdAt: string
}

export interface PaymentMethod {
  id: string
  type: string
  last4: string
  expiryMonth: number
  expiryYear: number
  isDefault: boolean
}
