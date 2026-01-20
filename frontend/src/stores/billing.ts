import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Invoice, Payment, PaymentMethod } from '@/types/billing.types'

export const useBillingStore = defineStore('billing', () => {
  const invoices = ref<Invoice[]>([])
  const payments = ref<Payment[]>([])
  const paymentMethods = ref<PaymentMethod[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  const fetchInvoices = async () => {
    loading.value = true
    error.value = null
    try {
      invoices.value = [
        {
          id: '1',
          invoiceNumber: 'INV-2024-001',
          date: new Date('2024-01-15'),
          dueDate: new Date('2024-01-29'),
          amount: 299,
          status: 'Paid',
          items: [
            {
              description: 'Standard 10GB Plan',
              amount: 299,
              quantity: 1
            }
          ]
        },
        {
          id: '2',
          invoiceNumber: 'INV-2023-012',
          date: new Date('2023-12-15'),
          dueDate: new Date('2023-12-29'),
          amount: 299,
          status: 'Paid',
          items: [
            {
              description: 'Standard 10GB Plan',
              amount: 299,
              quantity: 1
            }
          ]
        },
        {
          id: '3',
          invoiceNumber: 'INV-2023-011',
          date: new Date('2023-11-15'),
          dueDate: new Date('2023-11-29'),
          amount: 199,
          status: 'Paid',
          items: [
            {
              description: 'Basic 5GB Plan',
              amount: 199,
              quantity: 1
            }
          ]
        }
      ]
    } catch (err) {
      error.value = 'Failed to fetch invoices'
      console.error(err)
    } finally {
      loading.value = false
    }
  }

  const fetchPayments = async () => {
    loading.value = true
    error.value = null
    try {
      payments.value = [
        {
          id: '1',
          invoiceId: '1',
          amount: 299,
          date: new Date('2024-01-15'),
          method: 'Credit Card',
          status: 'Completed',
          transactionId: 'TXN-123456'
        },
        {
          id: '2',
          invoiceId: '2',
          amount: 299,
          date: new Date('2023-12-15'),
          method: 'Credit Card',
          status: 'Completed',
          transactionId: 'TXN-123455'
        }
      ]
    } catch (err) {
      error.value = 'Failed to fetch payments'
      console.error(err)
    } finally {
      loading.value = false
    }
  }

  const fetchPaymentMethods = async () => {
    loading.value = true
    error.value = null
    try {
      paymentMethods.value = [
        {
          id: '1',
          type: 'Credit Card',
          last4: '4242',
          expiryMonth: 12,
          expiryYear: 2025,
          isDefault: true
        },
        {
          id: '2',
          type: 'Credit Card',
          last4: '8888',
          expiryMonth: 6,
          expiryYear: 2026,
          isDefault: false
        }
      ]
    } catch (err) {
      error.value = 'Failed to fetch payment methods'
      console.error(err)
    } finally {
      loading.value = false
    }
  }

  const makePayment = async (invoiceId: string, paymentMethodId: string, amount: number) => {
    loading.value = true
    error.value = null
    try {
      const newPayment: Payment = {
        id: Date.now().toString(),
        invoiceId,
        amount,
        date: new Date(),
        method: 'Credit Card',
        status: 'Completed',
        transactionId: `TXN-${Math.random().toString(36).substr(2, 9).toUpperCase()}`
      }
      
      payments.value.unshift(newPayment)
      
      const invoice = invoices.value.find(inv => inv.id === invoiceId)
      if (invoice) {
        invoice.status = 'Paid'
      }
      
      return { success: true, payment: newPayment }
    } catch (err) {
      error.value = 'Failed to process payment'
      console.error(err)
      return { success: false, error: error.value }
    } finally {
      loading.value = false
    }
  }

  const addPaymentMethod = async (method: Omit<PaymentMethod, 'id'>) => {
    loading.value = true
    error.value = null
    try {
      const newMethod: PaymentMethod = {
        ...method,
        id: Date.now().toString()
      }
      
      if (method.isDefault) {
        paymentMethods.value.forEach(pm => {
          pm.isDefault = false
        })
      }
      
      paymentMethods.value.push(newMethod)
      return { success: true, method: newMethod }
    } catch (err) {
      error.value = 'Failed to add payment method'
      console.error(err)
      return { success: false, error: error.value }
    } finally {
      loading.value = false
    }
  }

  const removePaymentMethod = async (methodId: string) => {
    loading.value = true
    error.value = null
    try {
      paymentMethods.value = paymentMethods.value.filter(pm => pm.id !== methodId)
      return { success: true }
    } catch (err) {
      error.value = 'Failed to remove payment method'
      console.error(err)
      return { success: false, error: error.value }
    } finally {
      loading.value = false
    }
  }

  const getOutstandingBalance = (): number => {
    return invoices.value
      .filter(inv => inv.status !== 'Paid')
      .reduce((sum, inv) => sum + inv.amount, 0)
  }

  return {
    invoices,
    payments,
    paymentMethods,
    loading,
    error,
    fetchInvoices,
    fetchPayments,
    fetchPaymentMethods,
    makePayment,
    addPaymentMethod,
    removePaymentMethod,
    getOutstandingBalance
  }
})
