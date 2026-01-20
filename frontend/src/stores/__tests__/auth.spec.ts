import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useAuthStore } from '../auth'

describe('Auth Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    localStorage.clear()
  })

  it('should initialize with no user', () => {
    const store = useAuthStore()
    expect(store.user).toBeNull()
    expect(store.isAuthenticated).toBe(false)
  })

  it('should set user after successful login', async () => {
    const store = useAuthStore()
    
    const result = await store.login({
      email: 'test@example.com',
      password: 'Password123!'
    })

    expect(result.success).toBe(true)
    expect(store.user).toBeTruthy()
    expect(store.isAuthenticated).toBe(true)
  })

  it('should clear user after logout', async () => {
    const store = useAuthStore()
    
    await store.login({
      email: 'test@example.com',
      password: 'Password123!'
    })

    await store.logout()

    expect(store.user).toBeNull()
    expect(store.accessToken).toBeNull()
    expect(store.isAuthenticated).toBe(false)
  })

  it('should persist tokens in localStorage', async () => {
    const store = useAuthStore()
    
    await store.login({
      email: 'test@example.com',
      password: 'Password123!'
    })

    expect(localStorage.getItem('accessToken')).toBeTruthy()
    expect(localStorage.getItem('refreshToken')).toBeTruthy()
  })

  it('should initialize auth from localStorage', () => {
    localStorage.setItem('accessToken', 'test-token')
    localStorage.setItem('refreshToken', 'test-refresh-token')
    localStorage.setItem('user', JSON.stringify({
      id: '1',
      email: 'test@example.com',
      firstName: 'Test',
      lastName: 'User',
      role: 'user'
    }))

    const store = useAuthStore()
    store.initializeAuth()

    expect(store.isAuthenticated).toBe(true)
    expect(store.user).toBeTruthy()
  })
})
