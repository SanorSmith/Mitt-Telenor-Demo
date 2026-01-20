<template>
  <div class="space-y-6">
    <h1 class="text-3xl font-bold text-gray-900">Help & Support</h1>

    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="card hover:shadow-lg transition-shadow cursor-pointer">
        <div class="w-12 h-12 bg-primary-100 rounded-lg flex items-center justify-center mb-4">
          <HelpCircle class="text-primary-600" :size="24} />
        </div>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">FAQ</h3>
        <p class="text-gray-600 text-sm">Find answers to commonly asked questions</p>
      </div>

      <div class="card hover:shadow-lg transition-shadow cursor-pointer">
        <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center mb-4">
          <MessageCircle class="text-green-600" :size="24" />
        </div>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">Live Chat</h3>
        <p class="text-gray-600 text-sm">Chat with our support team</p>
      </div>

      <div class="card hover:shadow-lg transition-shadow cursor-pointer">
        <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center mb-4">
          <Phone class="text-blue-600" :size="24" />
        </div>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">Call Us</h3>
        <p class="text-gray-600 text-sm">Speak with a support agent</p>
      </div>
    </div>

    <div class="card">
      <h2 class="text-2xl font-bold text-gray-900 mb-6">Frequently Asked Questions</h2>
      <div class="space-y-4">
        <div
          v-for="(faq, index) in faqs"
          :key="index"
          class="border-b border-gray-200 last:border-0 pb-4 last:pb-0"
        >
          <button
            @click="toggleFaq(index)"
            class="w-full flex items-center justify-between text-left"
          >
            <h3 class="font-semibold text-gray-900">{{ faq.question }}</h3>
            <ChevronDown
              :size="20"
              class="text-gray-600 transition-transform"
              :class="{ 'rotate-180': faq.isOpen }"
            />
          </button>
          <div v-if="faq.isOpen" class="mt-3 text-gray-600">
            {{ faq.answer }}
          </div>
        </div>
      </div>
    </div>

    <div class="card">
      <h2 class="text-2xl font-bold text-gray-900 mb-4">Contact Us</h2>
      <form @submit.prevent="handleSubmit" class="space-y-4">
        <div>
          <label for="subject" class="block text-sm font-medium text-gray-700 mb-1">
            Subject
          </label>
          <input
            id="subject"
            v-model="contactForm.subject"
            type="text"
            required
            class="input"
            placeholder="How can we help?"
          />
        </div>

        <div>
          <label for="message" class="block text-sm font-medium text-gray-700 mb-1">
            Message
          </label>
          <textarea
            id="message"
            v-model="contactForm.message"
            rows="5"
            required
            class="input"
            placeholder="Describe your issue or question..."
          ></textarea>
        </div>

        <div class="flex justify-end">
          <button type="submit" class="btn-primary">
            Send Message
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { HelpCircle, MessageCircle, Phone, ChevronDown } from 'lucide-vue-next'

const faqs = ref([
  {
    question: 'How do I change my subscription plan?',
    answer: 'You can change your subscription plan at any time from the Subscriptions page. Simply select your desired plan and confirm the change. The new plan will take effect immediately.',
    isOpen: false
  },
  {
    question: 'When will I be charged for my subscription?',
    answer: 'You will be charged on the same day each month based on when you first subscribed. You can view your next billing date in the Billing section.',
    isOpen: false
  },
  {
    question: 'How do I check my data usage?',
    answer: 'Visit the Usage page to see your current data, voice, and SMS usage. You can also view historical usage trends and set up usage alerts.',
    isOpen: false
  },
  {
    question: 'Can I add extra data to my plan?',
    answer: 'Yes! You can purchase add-ons from the Subscriptions page. Add-ons are billed monthly and can be removed at any time.',
    isOpen: false
  },
  {
    question: 'How do I update my payment method?',
    answer: 'Go to the Billing page and click "Add Payment Method". You can add multiple payment methods and set one as default.',
    isOpen: false
  }
])

const contactForm = reactive({
  subject: '',
  message: ''
})

const toggleFaq = (index: number) => {
  faqs.value[index].isOpen = !faqs.value[index].isOpen
}

const handleSubmit = () => {
  console.log('Contact form submitted:', contactForm)
  contactForm.subject = ''
  contactForm.message = ''
}
</script>
