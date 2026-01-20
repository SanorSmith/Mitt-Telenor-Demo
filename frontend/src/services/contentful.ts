import { createClient, type Entry } from 'contentful'

const client = createClient({
  space: import.meta.env.VITE_CONTENTFUL_SPACE_ID || '',
  accessToken: import.meta.env.VITE_CONTENTFUL_ACCESS_TOKEN || ''
})

export interface FAQEntry {
  question: string
  answer: string
  category: string
  order: number
}

export interface SupportArticle {
  title: string
  content: string
  category: string
  tags: string[]
  publishedDate: Date
}

export const contentfulService = {
  async getFAQs(): Promise<FAQEntry[]> {
    try {
      const response = await client.getEntries({
        content_type: 'faq',
        order: 'fields.order'
      })

      return response.items.map((item: Entry) => ({
        question: item.fields.question as string,
        answer: item.fields.answer as string,
        category: item.fields.category as string,
        order: item.fields.order as number
      }))
    } catch (error) {
      console.error('Error fetching FAQs from Contentful:', error)
      return []
    }
  },

  async getSupportArticles(category?: string): Promise<SupportArticle[]> {
    try {
      const query: any = {
        content_type: 'supportArticle',
        order: '-fields.publishedDate'
      }

      if (category) {
        query['fields.category'] = category
      }

      const response = await client.getEntries(query)

      return response.items.map((item: Entry) => ({
        title: item.fields.title as string,
        content: item.fields.content as string,
        category: item.fields.category as string,
        tags: item.fields.tags as string[],
        publishedDate: new Date(item.fields.publishedDate as string)
      }))
    } catch (error) {
      console.error('Error fetching support articles from Contentful:', error)
      return []
    }
  },

  async getArticleBySlug(slug: string): Promise<SupportArticle | null> {
    try {
      const response = await client.getEntries({
        content_type: 'supportArticle',
        'fields.slug': slug,
        limit: 1
      })

      if (response.items.length === 0) return null

      const item = response.items[0]
      return {
        title: item.fields.title as string,
        content: item.fields.content as string,
        category: item.fields.category as string,
        tags: item.fields.tags as string[],
        publishedDate: new Date(item.fields.publishedDate as string)
      }
    } catch (error) {
      console.error('Error fetching article from Contentful:', error)
      return null
    }
  }
}
