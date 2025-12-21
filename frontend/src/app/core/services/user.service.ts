import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/auth.models';
import { UserDto } from '../models/user.models';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly API_URL = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  /**
   * Get assignable users with optional search
   */
  getAssignees(search?: string): Observable<ApiResponse<UserDto[]>> {
    let params = new HttpParams();
    
    if (search && search.trim()) {
      params = params.set('search', search.trim());
    }

    return this.http.get<ApiResponse<UserDto[]>>(
      `${this.API_URL}/api/Users/Assignees`,
      { params }
    );
  }
}
