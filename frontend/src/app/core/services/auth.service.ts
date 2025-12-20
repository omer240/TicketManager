import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, throwError, BehaviorSubject } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  ApiResponse,
  StoredAuthData
} from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = environment.apiBaseUrl;
  private readonly TOKEN_KEY = 'ticket_manager_auth';

  // Signal-based reactive state
  private currentUserSubject = new BehaviorSubject<StoredAuthData | null>(this.getStoredAuth());
  public currentUser$ = this.currentUserSubject.asObservable();
  
  // Computed signals
  public isAuthenticated = computed(() => this.currentUserSubject.value !== null);

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    // Check token expiry on init
    this.checkTokenExpiry();
  }

  /**
   * Login user
   */
  login(credentials: LoginRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http
      .post<ApiResponse<AuthResponse>>(`${this.API_URL}/api/Auth/Login`, credentials)
      .pipe(
        tap(response => {
          if (response.success && response.data) {
            this.storeAuth(response.data);
            this.currentUserSubject.next(response.data);
          }
        }),
        catchError(this.handleError)
      );
  }

  /**
   * Register new user
   */
  register(userData: RegisterRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http
      .post<ApiResponse<AuthResponse>>(`${this.API_URL}/api/Auth/Register`, userData)
      .pipe(
        tap(response => {
          if (response.success && response.data) {
            this.storeAuth(response.data);
            this.currentUserSubject.next(response.data);
          }
        }),
        catchError(this.handleError)
      );
  }

  /**
   * Logout user
   */
  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.currentUserSubject.next(null);
    this.router.navigate(['/auth/login']);
  }

  /**
   * Get stored access token
   */
  getAccessToken(): string | null {
    const auth = this.getStoredAuth();
    return auth?.accessToken || null;
  }

  /**
   * Get current user info
   */
  getCurrentUser(): StoredAuthData | null {
    return this.currentUserSubject.value;
  }

  /**
   * Check if token is expired
   */
  isTokenExpired(): boolean {
    const auth = this.getStoredAuth();
    if (!auth) return true;

    const expiresAt = new Date(auth.expiresAt);
    const now = new Date();
    return now >= expiresAt;
  }

  /**
   * Store auth data in localStorage
   */
  private storeAuth(authData: AuthResponse): void {
    const stored: StoredAuthData = {
      accessToken: authData.accessToken,
      expiresAt: authData.expiresAt,
      userId: authData.userId,
      email: authData.email,
      fullName: authData.fullName
    };
    localStorage.setItem(this.TOKEN_KEY, JSON.stringify(stored));
  }

  /**
   * Retrieve auth data from localStorage
   */
  private getStoredAuth(): StoredAuthData | null {
    const stored = localStorage.getItem(this.TOKEN_KEY);
    if (!stored) return null;

    try {
      return JSON.parse(stored) as StoredAuthData;
    } catch {
      return null;
    }
  }

  /**
   * Check token expiry and logout if expired
   */
  private checkTokenExpiry(): void {
    if (this.isTokenExpired()) {
      this.logout();
    }
  }

  /**
   * Handle HTTP errors
   */
  private handleError(error: any): Observable<never> {
    console.error('Auth error:', error);
    return throwError(() => error);
  }
}
