export type Json =
  | string
  | number
  | boolean
  | null
  | { [key: string]: Json | undefined }
  | Json[]

export interface Database {
  public: {
    Tables: {
      profiles: {
        Row: {
          id: string
          email: string
          first_name: string
          last_name: string
          phone: string | null
          address: string | null
          date_of_birth: string | null
          profile_picture_url: string | null
          is_unsubscribed: boolean
          unsubscribed_at: string | null
          created_at: string
          updated_at: string
        }
        Insert: {
          id: string
          email: string
          first_name: string
          last_name: string
          phone?: string | null
          address?: string | null
          date_of_birth?: string | null
          profile_picture_url?: string | null
          is_unsubscribed?: boolean
          unsubscribed_at?: string | null
          created_at?: string
          updated_at?: string
        }
        Update: {
          id?: string
          email?: string
          first_name?: string
          last_name?: string
          phone?: string | null
          address?: string | null
          date_of_birth?: string | null
          profile_picture_url?: string | null
          is_unsubscribed?: boolean
          unsubscribed_at?: string | null
          created_at?: string
          updated_at?: string
        }
      }
      subscriptions: {
        Row: {
          id: string
          user_id: string
          plan_id: string
          status: 'active' | 'suspended' | 'cancelled'
          start_date: string
          end_date: string | null
          auto_renew: boolean
          created_at: string
          updated_at: string
        }
        Insert: {
          id?: string
          user_id: string
          plan_id: string
          status?: 'active' | 'suspended' | 'cancelled'
          start_date?: string
          end_date?: string | null
          auto_renew?: boolean
          created_at?: string
          updated_at?: string
        }
        Update: {
          id?: string
          user_id?: string
          plan_id?: string
          status?: 'active' | 'suspended' | 'cancelled'
          start_date?: string
          end_date?: string | null
          auto_renew?: boolean
          created_at?: string
          updated_at?: string
        }
      }
      plans: {
        Row: {
          id: string
          name: string
          type: 'mobile' | 'broadband' | 'tv'
          price: number
          data_gb: number | null
          voice_minutes: number | null
          sms_count: number | null
          features: string[]
          is_popular: boolean
          created_at: string
          updated_at: string
        }
        Insert: {
          id?: string
          name: string
          type: 'mobile' | 'broadband' | 'tv'
          price: number
          data_gb?: number | null
          voice_minutes?: number | null
          sms_count?: number | null
          features?: string[]
          is_popular?: boolean
          created_at?: string
          updated_at?: string
        }
        Update: {
          id?: string
          name?: string
          type?: 'mobile' | 'broadband' | 'tv'
          price?: number
          data_gb?: number | null
          voice_minutes?: number | null
          sms_count?: number | null
          features?: string[]
          is_popular?: boolean
          created_at?: string
          updated_at?: string
        }
      }
      addons: {
        Row: {
          id: string
          subscription_id: string
          name: string
          price: number
          data_gb: number | null
          voice_minutes: number | null
          sms_count: number | null
          created_at: string
        }
        Insert: {
          id?: string
          subscription_id: string
          name: string
          price: number
          data_gb?: number | null
          voice_minutes?: number | null
          sms_count?: number | null
          created_at?: string
        }
        Update: {
          id?: string
          subscription_id?: string
          name?: string
          price?: number
          data_gb?: number | null
          voice_minutes?: number | null
          sms_count?: number | null
          created_at?: string
        }
      }
      usage_records: {
        Row: {
          id: string
          user_id: string
          subscription_id: string
          period: string
          data_used_mb: number
          data_allowance_mb: number
          voice_used_minutes: number
          voice_allowance_minutes: number
          sms_used_count: number
          sms_allowance_count: number
          created_at: string
          updated_at: string
        }
        Insert: {
          id?: string
          user_id: string
          subscription_id: string
          period: string
          data_used_mb?: number
          data_allowance_mb: number
          voice_used_minutes?: number
          voice_allowance_minutes: number
          sms_used_count?: number
          sms_allowance_count: number
          created_at?: string
          updated_at?: string
        }
        Update: {
          id?: string
          user_id?: string
          subscription_id?: string
          period?: string
          data_used_mb?: number
          data_allowance_mb?: number
          voice_used_minutes?: number
          voice_allowance_minutes?: number
          sms_used_count?: number
          sms_allowance_count?: number
          created_at?: string
          updated_at?: string
        }
      }
      invoices: {
        Row: {
          id: string
          user_id: string
          invoice_number: string
          amount: number
          tax: number
          total: number
          status: 'pending' | 'paid' | 'overdue' | 'cancelled'
          due_date: string
          paid_date: string | null
          created_at: string
        }
        Insert: {
          id?: string
          user_id: string
          invoice_number: string
          amount: number
          tax: number
          total: number
          status?: 'pending' | 'paid' | 'overdue' | 'cancelled'
          due_date: string
          paid_date?: string | null
          created_at?: string
        }
        Update: {
          id?: string
          user_id?: string
          invoice_number?: string
          amount?: number
          tax?: number
          total?: number
          status?: 'pending' | 'paid' | 'overdue' | 'cancelled'
          due_date?: string
          paid_date?: string | null
          created_at?: string
        }
      }
      invoice_items: {
        Row: {
          id: string
          invoice_id: string
          description: string
          quantity: number
          unit_price: number
          total: number
          created_at: string
        }
        Insert: {
          id?: string
          invoice_id: string
          description: string
          quantity: number
          unit_price: number
          total: number
          created_at?: string
        }
        Update: {
          id?: string
          invoice_id?: string
          description?: string
          quantity?: number
          unit_price?: number
          total?: number
          created_at?: string
        }
      }
      payments: {
        Row: {
          id: string
          user_id: string
          invoice_id: string
          amount: number
          payment_method: string
          status: 'pending' | 'completed' | 'failed'
          transaction_id: string | null
          created_at: string
        }
        Insert: {
          id?: string
          user_id: string
          invoice_id: string
          amount: number
          payment_method: string
          status?: 'pending' | 'completed' | 'failed'
          transaction_id?: string | null
          created_at?: string
        }
        Update: {
          id?: string
          user_id?: string
          invoice_id?: string
          amount?: number
          payment_method?: string
          status?: 'pending' | 'completed' | 'failed'
          transaction_id?: string | null
          created_at?: string
        }
      }
      payment_methods: {
        Row: {
          id: string
          user_id: string
          type: 'card' | 'bank_account' | 'paypal'
          last_four: string
          expiry_month: number | null
          expiry_year: number | null
          is_default: boolean
          created_at: string
        }
        Insert: {
          id?: string
          user_id: string
          type: 'card' | 'bank_account' | 'paypal'
          last_four: string
          expiry_month?: number | null
          expiry_year?: number | null
          is_default?: boolean
          created_at?: string
        }
        Update: {
          id?: string
          user_id?: string
          type?: 'card' | 'bank_account' | 'paypal'
          last_four?: string
          expiry_month?: number | null
          expiry_year?: number | null
          is_default?: boolean
          created_at?: string
        }
      }
      notifications: {
        Row: {
          id: string
          user_id: string
          title: string
          message: string
          type: 'info' | 'warning' | 'error' | 'success'
          read: boolean
          created_at: string
        }
        Insert: {
          id?: string
          user_id: string
          title: string
          message: string
          type?: 'info' | 'warning' | 'error' | 'success'
          read?: boolean
          created_at?: string
        }
        Update: {
          id?: string
          user_id?: string
          title?: string
          message?: string
          type?: 'info' | 'warning' | 'error' | 'success'
          read?: boolean
          created_at?: string
        }
      }
    }
    Views: {
      [_ in never]: never
    }
    Functions: {
      [_ in never]: never
    }
    Enums: {
      [_ in never]: never
    }
  }
}
