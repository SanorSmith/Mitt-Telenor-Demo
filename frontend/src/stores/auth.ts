import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { supabase } from '@/lib/supabase'
import type { User as SupabaseUser } from '@supabase/supabase-js'

interface RegisterData {
  email: string
  password: string
  firstName: string
  lastName: string
}

interface LoginData {
  email: string
  password: string
}

export const useAuthStore = defineStore('auth', () => {
  const user = ref<SupabaseUser | null>(null)
  const loading = ref(false)

  const isAuthenticated = computed(() => !!user.value)

  async function register(data: RegisterData) {
    loading.value = true
    try {
      const { data: authData, error: authError } = await supabase.auth.signUp({
        email: data.email,
        password: data.password,
        options: {
          data: {
            first_name: data.firstName,
            last_name: data.lastName
          }
        }
      })

      if (authError) throw authError

      if (authData.user) {
        // Profile will be created automatically by database trigger
        user.value = authData.user
        return { success: true }
      }

      return { success: false, error: 'Registration failed' }
    } catch (error: any) {
      return { 
        success: false, 
        error: error.message || 'Registration failed' 
      }
    } finally {
      loading.value = false
    }
  }

  async function login(data: LoginData) {
    loading.value = true
    try {
      const { data: authData, error } = await supabase.auth.signInWithPassword({
        email: data.email,
        password: data.password
      })

      if (error) throw error

      user.value = authData.user

      // Check if user is unsubscribed
      if (authData.user) {
        const { data: profileData } = await supabase
          .from('profiles')
          .select('is_unsubscribed')
          .eq('id', authData.user.id)
          .single()

        if (profileData?.is_unsubscribed) {
          const reactivate = confirm(
            'Your account was previously unsubscribed. Would you like to reactivate it?'
          )

          if (reactivate) {
            // Reactivate account
            await supabase
              .from('profiles')
              .update({
                is_unsubscribed: false,
                unsubscribed_at: null,
                updated_at: new Date().toISOString()
              })
              .eq('id', authData.user.id)

            return { success: true, reactivated: true }
          } else {
            // User declined reactivation, log them out
            await supabase.auth.signOut()
            user.value = null
            return {
              success: false,
              error: 'Account is unsubscribed. Please reactivate to continue.'
            }
          }
        }
      }

      return { success: true }
    } catch (error: any) {
      return { 
        success: false, 
        error: error.message || 'Login failed' 
      }
    } finally {
      loading.value = false
    }
  }

  async function logout() {
    loading.value = true
    try {
      const { error } = await supabase.auth.signOut()
      if (error) throw error
      user.value = null
    } catch (error) {
      console.error('Logout error:', error)
    } finally {
      loading.value = false
    }
  }

  async function initializeAuth() {
    loading.value = true
    try {
      const { data: { session } } = await supabase.auth.getSession()
      user.value = session?.user ?? null

      supabase.auth.onAuthStateChange((_event, session) => {
        user.value = session?.user ?? null
      })
    } catch (error) {
      console.error('Auth initialization error:', error)
      user.value = null
    } finally {
      loading.value = false
    }
  }

  return {
    user,
    loading,
    isAuthenticated,
    register,
    login,
    logout,
    initializeAuth
  }
})
