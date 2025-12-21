import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/auth.models';
import { CommentDto, CommentCreateRequest, CommentUpdateRequest } from '../models/comment.models';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private http = inject(HttpClient);
  private readonly API_URL = environment.apiBaseUrl;

  /**
   * Get all comments for a ticket
   */
  getByTicket(ticketId: number): Observable<ApiResponse<CommentDto[]>> {
    return this.http.get<ApiResponse<CommentDto[]>>(
      `${this.API_URL}/api/Comments/ByTicket?ticketId=${ticketId}`
    );
  }

  /**
   * Add new comment to ticket
   */
  add(request: CommentCreateRequest): Observable<ApiResponse<CommentDto>> {
    return this.http.post<ApiResponse<CommentDto>>(
      `${this.API_URL}/api/Comments/Create`,
      request
    );
  }

  /**
   * Update existing comment
   */
  update(commentId: number, request: CommentUpdateRequest): Observable<ApiResponse<CommentDto>> {
    return this.http.put<ApiResponse<CommentDto>>(
      `${this.API_URL}/api/Comments/Update?commentId=${commentId}`,
      request
    );
  }

  /**
   * Delete comment
   */
  delete(commentId: number): Observable<ApiResponse<{ deleted: boolean }>> {
    return this.http.delete<ApiResponse<{ deleted: boolean }>>(
      `${this.API_URL}/api/Comments/Delete?commentId=${commentId}`
    );
  }
}
