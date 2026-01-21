// Test Contentful Connection
import { createClient } from 'contentful'

const client = createClient({
  space: '455sis87wj48',
  accessToken: 'phzfMIq8VW9RK1vQCyEjpxXa5H_FL-GVBEcAfNXmLjY'
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
