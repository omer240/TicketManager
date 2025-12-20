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
  TicketStatusUpdateRequest,
  PagedResult
} from '../models/ticket.models';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private readonly API_URL = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  /**
   * Get tickets created by current user
   */
  getMyCreated(query?: TicketQuery): Observable<ApiResponse<PagedResult<TicketDto>>> {
    const params = this.buildQueryParams(query);
    return this.http.get<ApiResponse<PagedResult<TicketDto>>>(
      `${this.API_URL}/api/Tickets/MyCreated`,
      { params }
    );
  }

  /**
   * Get tickets assigned to current user
   */
  getMyAssigned(query?: TicketQuery): Observable<ApiResponse<PagedResult<TicketDto>>> {
    const params = this.buildQueryParams(query);
    return this.http.get<ApiResponse<PagedResult<TicketDto>>>(
      `${this.API_URL}/api/Tickets/MyAssigned`,
      { params }
    );
  }

  /**
   * Get ticket detail by ID
   */
  getDetail(ticketId: number): Observable<ApiResponse<TicketDto>> {
    return this.http.get<ApiResponse<TicketDto>>(
      `${this.API_URL}/api/Tickets/Detail`,
      { params: { ticketId: ticketId.toString() } }
    );
  }

  /**
   * Create new ticket
   */
  create(request: TicketCreateRequest): Observable<ApiResponse<TicketDto>> {
    return this.http.post<ApiResponse<TicketDto>>(
      `${this.API_URL}/api/Tickets/Create`,
      request
    );
  }

  /**
   * Update ticket
   */
  update(ticketId: number, request: TicketUpdateRequest): Observable<ApiResponse<TicketDto>> {
    return this.http.put<ApiResponse<TicketDto>>(
      `${this.API_URL}/api/Tickets/Update`,
      request,
      { params: { ticketId: ticketId.toString() } }
    );
  }

  /**
   * Update ticket status only
   */
  updateStatus(request: TicketStatusUpdateRequest): Observable<ApiResponse<TicketDto>> {
    return this.http.patch<ApiResponse<TicketDto>>(
      `${this.API_URL}/api/Tickets/UpdateStatus`,
      request
    );
  }

  /**
   * Build HTTP params from query object
   */
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
