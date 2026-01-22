// Test Contentful Connection
import { createClient } from 'contentful'

// Load credentials from environment variables
const spaceId = process.env.VITE_CONTENTFUL_SPACE_ID
const accessToken = process.env.VITE_CONTENTFUL_ACCESS_TOKEN

if (!spaceId || !accessToken) {
  console.error('❌ Contentful credentials not found in environment variables')
  console.log('Please set VITE_CONTENTFUL_SPACE_ID and VITE_CONTENTFUL_ACCESS_TOKEN')
  process.exit(1)
}

const client = createClient({
  space: spaceId,
  accessToken: accessToken
})

async function testContentful() {
  try {
    console.log('Testing Contentful connection...')
    
    // Test 1: Get spaces (basic connection test)
    const space = await client.getSpace()
    console.log('✅ Connected to space:', space.name)
    
    // Test 2: Get content types
    const contentTypes = await client.getContentTypes()
    console.log('📋 Available content types:', contentTypes.items.map(ct => ct.name))
    
    // Test 3: Try to get FAQs
    try {
      const faqs = await client.getEntries({ content_type: 'faq' })
      console.log(`❓ Found ${faqs.items.length} FAQ entries`)
      faqs.items.forEach(faq => {
        console.log(`  - ${faq.fields.question}`)
      })
    } catch (error) {
      console.log('❓ FAQ content type not found or no entries')
    }
    
    // Test 4: Try to get support articles
    try {
      const articles = await client.getEntries({ content_type: 'supportArticle' })
      console.log(`📄 Found ${articles.items.length} support articles`)
      articles.items.forEach(article => {
        console.log(`  - ${article.fields.title}`)
      })
    } catch (error) {
      console.log('📄 Support Article content type not found or no entries')
    }
    
  } catch (error) {
    console.error('❌ Contentful connection failed:', error.message)
  }
}

testContentful()
