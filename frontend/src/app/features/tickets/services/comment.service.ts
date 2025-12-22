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

  getByTicket(ticketId: number): Observable<ApiResponse<CommentDto[]>> {
    return this.http.get<ApiResponse<CommentDto[]>>(
      `${this.API_URL}/api/tickets/${ticketId}/comments`
    );
  }

  add(ticketId: number, text: string): Observable<ApiResponse<CommentDto>> {
    const url = `${this.API_URL}/api/tickets/${ticketId}/comments`;
    console.log('Adding comment - URL:', url, 'TicketId:', ticketId, 'Text:', text);
    return this.http.post<ApiResponse<CommentDto>>(url, { text });
  }

  update(commentId: number, request: CommentUpdateRequest): Observable<ApiResponse<CommentDto>> {
    const url = `${this.API_URL}/api/comments/${commentId}`;
    console.log('Updating comment - URL:', url, 'CommentId:', commentId, 'Request:', request);
    return this.http.put<ApiResponse<CommentDto>>(url, request);
  }

  delete(commentId: number): Observable<ApiResponse<{ deleted: boolean }>> {
    const url = `${this.API_URL}/api/comments/${commentId}`;
    console.log('Deleting comment - URL:', url, 'CommentId:', commentId);
    return this.http.delete<ApiResponse<{ deleted: boolean }>>(url);
  }
}
