import apiClient from './axios-instance'
import type { LoginRequest, RegisterRequest, TokenResponse } from '@/types/auth.types'

export const authApi = {
  async register(data: RegisterRequest): Promise<TokenResponse> {
    const response = await apiClient.post<TokenResponse>('/auth/register', data)
    return response.data
  },

  async login(data: LoginRequest): Promise<TokenResponse> {
    const response = await apiClient.post<TokenResponse>('/auth/login', data)
    return response.data
  },

  async refreshToken(refreshToken: string): Promise<TokenResponse> {
    const response = await apiClient.post<TokenResponse>('/auth/refresh', { refreshToken })
    return response.data
  },

  async logout(refreshToken: string): Promise<void> {
    await apiClient.post('/auth/logout', { refreshToken })
  },

  async validateToken(token: string): Promise<boolean> {
    try {
      const response = await apiClient.post<{ valid: boolean }>('/auth/validate', { token })
      return response.data.valid
    } catch {
      return false
    }
  }
}
