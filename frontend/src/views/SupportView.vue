<template>
  <div class="space-y-6">
    <h1 class="text-3xl font-bold text-gray-900">Help & Support</h1>

    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="card hover:shadow-lg transition-shadow cursor-pointer">
        <div class="w-12 h-12 bg-primary-100 rounded-lg flex items-center justify-center mb-4">
          <HelpCircle class="text-primary-600" :size="24" />
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
      
      <!-- Loading State -->
      <div v-if="loading" class="text-center py-8">
        <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
        <p class="mt-2 text-gray-600">Loading FAQs...</p>
      </div>
      
      <!-- Error State -->
      <div v-else-if="error" class="text-center py-8">
        <p class="text-red-600">{{ error }}</p>
        <p class="text-sm text-gray-600 mt-2">Using fallback FAQ data</p>
      </div>
      
      <!-- FAQ Content -->
      <div v-else class="space-y-4">
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
import { ref, reactive, onMounted } from 'vue'
import { HelpCircle, MessageCircle, Phone, ChevronDown } from 'lucide-vue-next'
import { contentfulService } from '@/services/contentful'

const faqs = ref([])
const loading = ref(true)
const error = ref('')

onMounted(async () => {
  try {
    console.log('Loading FAQs from Contentful...')
    const faqData = await contentfulService.getFAQs()
    console.log('Loaded FAQs:', faqData)
    
    // Add isOpen property to each FAQ
    faqs.value = faqData.map(faq => ({
      ...faq,
      isOpen: false
    }))
    
    // If no FAQs from Contentful, use fallback data
    if (faqs.value.length === 0) {
      console.log('No FAQs from Contentful, using fallback data')
      faqs.value = [
        {
          question: 'How do I change my subscription plan?',
          answer: 'You can change your subscription plan at any time from the Subscriptions page. Simply select your desired plan and confirm the change.',
          category: 'Subscriptions',
          order: 1,
          isOpen: false
        },
        {
          question: 'How do I check my data usage?',
          answer: 'Visit the Usage page to see your current data, voice, and SMS usage. You can also view historical usage trends.',
          category: 'Usage',
          order: 2,
          isOpen: false
        }
      ]
    }
  } catch (err) {
    console.error('Error loading FAQs:', err)
    error.value = 'Failed to load FAQs from Contentful'
    
    // Fallback data
    faqs.value = [
      {
        question: 'How do I change my subscription plan?',
        answer: 'You can change your subscription plan at any time from the Subscriptions page.',
        category: 'Subscriptions',
        order: 1,
        isOpen: false
      }
    ]
  } finally {
    loading.value = false
  }
})

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
