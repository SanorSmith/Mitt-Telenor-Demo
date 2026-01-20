export interface User {
  id: string
  email: string
  firstName: string
  lastName: string
  phone?: string
  role: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  email: string
  password: string
  firstName: string
  lastName: string
  phone?: string
}

export interface TokenResponse {
  accessToken: string
  refreshToken: string
  expiresAt: string
  user: User
}

export interface AuthState {
  user: User | null
  accessToken: string | null
  refreshToken: string | null
  isAuthenticated: boolean
}
