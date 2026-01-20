<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-3xl font-bold text-gray-900">Payment Methods</h1>
        <p class="text-gray-600 mt-1">Manage your payment methods and billing information</p>
      </div>
      <button @click="showAddModal = true" class="btn-primary">
        <Plus :size="20" class="mr-2" />
        Add Payment Method
      </button>
    </div>

    <div v-if="loading" class="flex justify-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
    </div>

    <div v-else-if="paymentMethods.length === 0" class="card text-center py-12">
      <CreditCard :size="64" class="mx-auto text-gray-400 mb-4" />
      <h3 class="text-xl font-semibold text-gray-900 mb-2">No payment methods</h3>
      <p class="text-gray-600 mb-4">Add a payment method to start making payments</p>
      <button @click="showAddModal = true" class="btn-primary">
        Add Your First Payment Method
      </button>
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div
        v-for="method in paymentMethods"
        :key="method.id"
        class="card hover:shadow-lg transition-shadow relative"
        :class="{ 'ring-2 ring-primary-500': method.is_default }"
      >
        <div class="flex items-start justify-between mb-4">
          <div class="flex items-center space-x-3">
            <div class="w-12 h-12 bg-primary-100 rounded-lg flex items-center justify-center">
              <CreditCard :size="24" class="text-primary-600" />
            </div>
            <div>
              <p class="font-semibold text-gray-900">{{ getCardType(method.type) }}</p>
              <p class="text-sm text-gray-600">•••• •••• •••• {{ method.last_four }}</p>
            </div>
          </div>
          <span
            v-if="method.is_default"
            class="text-xs bg-primary-100 text-primary-700 px-2 py-1 rounded-full font-medium"
          >
            Default
          </span>
        </div>

        <div class="space-y-2 mb-4">
          <div class="flex justify-between text-sm">
            <span class="text-gray-600">Expires</span>
            <span class="font-medium">{{ formatExpiry(method.expiry_month, method.expiry_year) }}</span>
          </div>
          <div class="flex justify-between text-sm">
            <span class="text-gray-600">Added</span>
            <span class="font-medium">{{ formatDate(method.created_at) }}</span>
          </div>
        </div>

        <div class="flex space-x-2">
          <button
            v-if="!method.is_default"
            @click="setAsDefault(method.id)"
            class="flex-1 btn-secondary text-sm"
          >
            Set as Default
          </button>
          <button
            @click="editMethod(method)"
            class="flex-1 btn-secondary text-sm"
          >
            Edit
          </button>
          <button
            @click="deleteMethod(method.id)"
            :disabled="method.is_default"
            class="px-3 py-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            :title="method.is_default ? 'Cannot delete default payment method' : 'Delete'"
          >
            <Trash2 :size="18" />
          </button>
        </div>
      </div>
    </div>

    <!-- Add/Edit Payment Method Modal -->
    <div
      v-if="showAddModal || showEditModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="closeModals"
    >
      <div class="bg-white rounded-lg max-w-md w-full p-6">
        <h2 class="text-2xl font-bold text-gray-900 mb-4">
          {{ showEditModal ? 'Edit Payment Method' : 'Add Payment Method' }}
        </h2>

        <form @submit.prevent="handleSubmit" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              Card Type
            </label>
            <select v-model="formData.type" required class="input">
              <option value="card">Credit/Debit Card</option>
              <option value="bank_account">Bank Account</option>
              <option value="paypal">PayPal</option>
            </select>
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              Card Number
            </label>
            <input
              v-model="formData.cardNumber"
              type="text"
              placeholder="1234 5678 9012 3456"
              maxlength="19"
              required
              class="input"
              @input="formatCardNumber"
            />
          </div>

          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">
                Expiry Month
              </label>
              <select v-model="formData.expiryMonth" required class="input">
                <option value="">MM</option>
                <option v-for="month in 12" :key="month" :value="month">
                  {{ String(month).padStart(2, '0') }}
                </option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">
                Expiry Year
              </label>
              <select v-model="formData.expiryYear" required class="input">
                <option value="">YYYY</option>
                <option v-for="year in 10" :key="year" :value="new Date().getFullYear() + year - 1">
                  {{ new Date().getFullYear() + year - 1 }}
                </option>
              </select>
            </div>
          </div>

          <div>
            <label class="flex items-center space-x-2">
              <input
                v-model="formData.isDefault"
                type="checkbox"
                class="w-4 h-4 text-primary-600 border-gray-300 rounded focus:ring-primary-500"
              />
              <span class="text-sm text-gray-700">Set as default payment method</span>
            </label>
          </div>

          <div class="flex space-x-3 pt-4">
            <button type="button" @click="closeModals" class="flex-1 btn-secondary">
              Cancel
            </button>
            <button type="submit" :disabled="saving" class="flex-1 btn-primary">
              {{ saving ? 'Saving...' : (showEditModal ? 'Update' : 'Add') }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { CreditCard, Plus, Trash2 } from 'lucide-vue-next'
import { useAuthStore } from '@/stores/auth'
import { supabase } from '@/lib/supabase'

const authStore = useAuthStore()
const loading = ref(false)
const saving = ref(false)
const showAddModal = ref(false)
const showEditModal = ref(false)
const paymentMethods = ref<any[]>([])
const editingId = ref<string | null>(null)

const formData = ref({
  type: 'card',
  cardNumber: '',
  expiryMonth: '',
  expiryYear: '',
  isDefault: false
})

const loadPaymentMethods = async () => {
  if (!authStore.user) return

  loading.value = true
  try {
    const { data, error } = await supabase
      .from('payment_methods')
      .select('*')
      .eq('user_id', authStore.user.id)
      .order('is_default', { ascending: false })
      .order('created_at', { ascending: false })

    if (error) throw error
    paymentMethods.value = data || []
  } catch (error) {
    console.error('Error loading payment methods:', error)
    alert('Failed to load payment methods')
  } finally {
    loading.value = false
  }
}

const formatCardNumber = (event: Event) => {
  const input = event.target as HTMLInputElement
  let value = input.value.replace(/\s/g, '')
  let formattedValue = value.match(/.{1,4}/g)?.join(' ') || value
  formData.value.cardNumber = formattedValue
}

const handleSubmit = async () => {
  if (!authStore.user) return

  saving.value = true
  try {
    const lastFour = formData.value.cardNumber.replace(/\s/g, '').slice(-4)

    // If setting as default, unset all other defaults first
    if (formData.value.isDefault) {
      await supabase
        .from('payment_methods')
        .update({ is_default: false })
        .eq('user_id', authStore.user.id)
    }

    const paymentData = {
      user_id: authStore.user.id,
      type: formData.value.type,
      last_four: lastFour,
      expiry_month: parseInt(formData.value.expiryMonth),
      expiry_year: parseInt(formData.value.expiryYear),
      is_default: formData.value.isDefault
    }

    if (showEditModal.value && editingId.value) {
      // Update existing
      const { error } = await supabase
        .from('payment_methods')
        .update(paymentData)
        .eq('id', editingId.value)

      if (error) throw error
      alert('Payment method updated successfully!')
    } else {
      // Insert new
      const { error } = await supabase
        .from('payment_methods')
        .insert(paymentData)

      if (error) throw error
      alert('Payment method added successfully!')
    }

    closeModals()
    await loadPaymentMethods()
  } catch (error: any) {
    console.error('Error saving payment method:', error)
    alert(`Failed to save payment method: ${error.message}`)
  } finally {
    saving.value = false
  }
}

const setAsDefault = async (id: string) => {
  if (!authStore.user) return

  try {
    // Unset all defaults
    await supabase
      .from('payment_methods')
      .update({ is_default: false })
      .eq('user_id', authStore.user.id)

    // Set new default
    const { error } = await supabase
      .from('payment_methods')
      .update({ is_default: true })
      .eq('id', id)

    if (error) throw error

    alert('Default payment method updated!')
    await loadPaymentMethods()
  } catch (error: any) {
    console.error('Error setting default:', error)
    alert(`Failed to set default: ${error.message}`)
  }
}

const editMethod = (method: any) => {
  editingId.value = method.id
  formData.value = {
    type: method.type,
    cardNumber: `•••• •••• •••• ${method.last_four}`,
    expiryMonth: String(method.expiry_month),
    expiryYear: String(method.expiry_year),
    isDefault: method.is_default
  }
  showEditModal.value = true
}

const deleteMethod = async (id: string) => {
  if (!confirm('Are you sure you want to delete this payment method?')) return

  try {
    const { error } = await supabase
      .from('payment_methods')
      .delete()
      .eq('id', id)

    if (error) throw error

    alert('Payment method deleted successfully!')
    await loadPaymentMethods()
  } catch (error: any) {
    console.error('Error deleting payment method:', error)
    alert(`Failed to delete payment method: ${error.message}`)
  }
}

const closeModals = () => {
  showAddModal.value = false
  showEditModal.value = false
  editingId.value = null
  formData.value = {
    type: 'card',
    cardNumber: '',
    expiryMonth: '',
    expiryYear: '',
    isDefault: false
  }
}

const getCardType = (type: string) => {
  const types: Record<string, string> = {
    card: 'Credit/Debit Card',
    bank_account: 'Bank Account',
    paypal: 'PayPal'
  }
  return types[type] || type
}

const formatExpiry = (month: number, year: number) => {
  if (!month || !year) return 'N/A'
  return `${String(month).padStart(2, '0')}/${year}`
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

onMounted(() => {
  loadPaymentMethods()
})
</script>
