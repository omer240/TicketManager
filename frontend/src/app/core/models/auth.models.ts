// Request models
export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  fullName: string;
  password: string;
}

// Response models
export interface AuthResponse {
  accessToken: string;
  expiresAt: string; // ISO 8601 date string
  userId: string;
  email: string;
  fullName: string;
}

// API wrapper
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: ApiError;
}

export interface ApiError {
  code: string;
  message: string;
}

// Token storage
export interface StoredAuthData {
  accessToken: string;
  expiresAt: string;
  userId: string;
  email: string;
  fullName: string;
}
