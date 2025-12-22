import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/auth.models';
import {
  TicketDto,
  TicketQuery,
  TicketCreateRequest,
  TicketUpdateRequest,
  TicketStatus,
  PagedResult
} from '../models/ticket.models';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private readonly API_URL = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  getMyCreated(query?: TicketQuery): Observable<ApiResponse<PagedResult<TicketDto>>> {
    const params = this.buildQueryParams(query);
    return this.http.get<ApiResponse<PagedResult<TicketDto>>>(
      `${this.API_URL}/api/tickets/created`,
      { params }
    );
  }

  getMyAssigned(query?: TicketQuery): Observable<ApiResponse<PagedResult<TicketDto>>> {
    const params = this.buildQueryParams(query);
    return this.http.get<ApiResponse<PagedResult<TicketDto>>>(
      `${this.API_URL}/api/tickets/assigned`,
      { params }
    );
  }

  getDetail(ticketId: number): Observable<ApiResponse<TicketDto>> {
    return this.http.get<ApiResponse<TicketDto>>(
      `${this.API_URL}/api/tickets/${ticketId}`
    );
  }

  create(request: TicketCreateRequest): Observable<ApiResponse<TicketDto>> {
    const url = `${this.API_URL}/api/tickets`;
    console.log('Creating ticket - URL:', url, 'Request:', request);
    return this.http.post<ApiResponse<TicketDto>>(url, request);
  }

  update(ticketId: number, request: TicketUpdateRequest): Observable<ApiResponse<TicketDto>> {
    const url = `${this.API_URL}/api/tickets/${ticketId}`;
    console.log('Updating ticket - URL:', url, 'TicketId:', ticketId, 'Request:', request);
    return this.http.put<ApiResponse<TicketDto>>(url, request);
  }

  updateStatus(ticketId: number, status: TicketStatus): Observable<ApiResponse<TicketDto>> {
    const url = `${this.API_URL}/api/tickets/${ticketId}/status`;
    console.log('Updating status - URL:', url, 'TicketId:', ticketId, 'Status:', status);
    return this.http.patch<ApiResponse<TicketDto>>(url, { status });
  }

  delete(ticketId: number): Observable<ApiResponse<any>> {
    const url = `${this.API_URL}/api/tickets/${ticketId}`;
    console.log('Deleting ticket - URL:', url, 'TicketId:', ticketId);
    return this.http.delete<ApiResponse<any>>(url);
  }

  private buildQueryParams(query?: TicketQuery): HttpParams {
    let params = new HttpParams();

    if (!query) return params;

    if (query.search) {
      params = params.set('search', query.search);
    }
    if (query.status !== undefined && query.status !== null) {
      params = params.set('status', query.status.toString());
    }
    if (query.priority !== undefined && query.priority !== null) {
      params = params.set('priority', query.priority.toString());
    }
    if (query.assignedToUserId) {
      params = params.set('assignedToUserId', query.assignedToUserId);
    }
    if (query.createdByUserId) {
      params = params.set('createdByUserId', query.createdByUserId);
    }
    if (query.page !== undefined) {
      params = params.set('page', query.page.toString());
    }
    if (query.pageSize !== undefined) {
      params = params.set('pageSize', query.pageSize.toString());
    }

    return params;
  }
}
