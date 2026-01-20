<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <h1 class="text-3xl font-bold text-gray-900">Billing & Payments</h1>
      <RouterLink to="/payment-methods" class="btn-primary inline-flex items-center">
        <CreditCard :size="20" class="mr-2" />
        Manage Payment Methods
      </RouterLink>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <div class="lg:col-span-2 space-y-6">
        <div class="card">
          <h2 class="text-xl font-semibold text-gray-900 mb-4">Recent Invoices</h2>
          <div class="space-y-3">
            <div
              v-for="invoice in invoices"
              :key="invoice.id"
              class="flex items-center justify-between p-4 border border-gray-200 rounded-lg hover:border-primary-300 transition-colors"
            >
              <div class="flex items-center space-x-4">
                <div
                  class="w-12 h-12 rounded-lg flex items-center justify-center"
                  :class="invoice.status === 'Paid' ? 'bg-green-100' : 'bg-yellow-100'"
                >
                  <Receipt
                    :size="24"
                    :class="invoice.status === 'Paid' ? 'text-green-600' : 'text-yellow-600'"
                  />
                </div>
                <div>
                  <p class="font-semibold text-gray-900">{{ invoice.number }}</p>
                  <p class="text-sm text-gray-600">{{ invoice.date }}</p>
                </div>
              </div>
              <div class="text-right">
                <p class="font-bold text-gray-900">{{ invoice.amount }} SEK</p>
                <span
                  class="inline-block px-2 py-1 text-xs font-medium rounded-full"
                  :class="invoice.status === 'Paid' ? 'bg-green-100 text-green-700' : 'bg-yellow-100 text-yellow-700'"
                >
                  {{ invoice.status }}
                </span>
              </div>
              <button class="text-primary-500 hover:text-primary-600">
                <Download :size="20" />
              </button>
            </div>
          </div>
        </div>

        <div class="card">
          <h2 class="text-xl font-semibold text-gray-900 mb-4">Payment History</h2>
          <div class="space-y-3">
            <div
              v-for="payment in payments"
              :key="payment.id"
              class="flex items-center justify-between py-3 border-b border-gray-200 last:border-0"
            >
              <div class="flex items-center space-x-3">
                <div class="w-10 h-10 bg-green-100 rounded-full flex items-center justify-center">
                  <CheckCircle class="text-green-600" :size="20" />
                </div>
                <div>
                  <p class="font-medium text-gray-900">{{ payment.description }}</p>
                  <p class="text-sm text-gray-600">{{ payment.method }} •••• {{ payment.last4 }}</p>
                </div>
              </div>
              <div class="text-right">
                <p class="font-semibold text-gray-900">{{ payment.amount }} SEK</p>
                <p class="text-sm text-gray-600">{{ payment.date }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="space-y-6">
        <div class="card">
          <h2 class="text-xl font-semibold text-gray-900 mb-4">Current Balance</h2>
          <div class="text-center py-4">
            <p class="text-4xl font-bold text-gray-900">0 SEK</p>
            <p class="text-gray-600 mt-2">No outstanding balance</p>
          </div>
          <div class="mt-4 p-3 bg-green-50 rounded-lg">
            <p class="text-sm text-green-700 text-center">
              All invoices are paid up to date
            </p>
          </div>
        </div>

        <div class="card">
          <h2 class="text-xl font-semibold text-gray-900 mb-4">Payment Methods</h2>
          <div class="space-y-3">
            <div
              v-for="method in paymentMethods"
              :key="method.id"
              class="p-3 border border-gray-200 rounded-lg"
            >
              <div class="flex items-center justify-between mb-2">
                <div class="flex items-center space-x-2">
                  <CreditCard :size="20" class="text-gray-600" />
                  <span class="font-medium">{{ method.type }}</span>
                </div>
                <span
                  v-if="method.isDefault"
                  class="text-xs bg-primary-100 text-primary-700 px-2 py-1 rounded-full"
                >
                  Default
                </span>
              </div>
              <p class="text-sm text-gray-600">•••• •••• •••• {{ method.last4 }}</p>
              <p class="text-xs text-gray-500 mt-1">Expires {{ method.expiry }}</p>
            </div>
          </div>
        </div>

        <div class="card">
          <h2 class="text-xl font-semibold text-gray-900 mb-4">Billing Settings</h2>
          <div class="space-y-3">
            <div class="flex items-center justify-between">
              <span class="text-gray-700">Auto-pay</span>
              <label class="relative inline-flex items-center cursor-pointer">
                <input type="checkbox" checked class="sr-only peer">
                <div class="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-primary-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-primary-600"></div>
              </label>
            </div>
            <div class="flex items-center justify-between">
              <span class="text-gray-700">Email notifications</span>
              <label class="relative inline-flex items-center cursor-pointer">
                <input type="checkbox" checked class="sr-only peer">
                <div class="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-primary-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-primary-600"></div>
              </label>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink } from 'vue-router'
import { CreditCard, Receipt, Download, CheckCircle } from 'lucide-vue-next'

const invoices = ref([
  {
    id: '1',
    number: 'INV-2024-001',
    date: 'Jan 15, 2024',
    amount: 299,
    status: 'Paid'
  },
  {
    id: '2',
    number: 'INV-2023-012',
    date: 'Dec 15, 2023',
    amount: 299,
    status: 'Paid'
  },
  {
    id: '3',
    number: 'INV-2023-011',
    date: 'Nov 15, 2023',
    amount: 199,
    status: 'Paid'
  }
])

const payments = ref([
  {
    id: '1',
    description: 'Monthly subscription',
    method: 'Visa',
    last4: '4242',
    amount: 299,
    date: 'Jan 15, 2024'
  },
  {
    id: '2',
    description: 'Monthly subscription',
    method: 'Visa',
    last4: '4242',
    amount: 299,
    date: 'Dec 15, 2023'
  }
])

const paymentMethods = ref([
  {
    id: '1',
    type: 'Visa',
    last4: '4242',
    expiry: '12/2025',
    isDefault: true
  },
  {
    id: '2',
    type: 'Mastercard',
    last4: '8888',
    expiry: '06/2026',
    isDefault: false
  }
])
</script>
