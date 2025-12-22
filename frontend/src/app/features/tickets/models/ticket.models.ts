export enum TicketStatus {
  Open = 1,
  InProgress = 2,
  Done = 3
}

export enum TicketPriority {
  Low = 1,
  Medium = 2,
  High = 3
}

export interface TicketDto {
  id: number;
  title: string;
  description: string;
  status: TicketStatus;
  priority: TicketPriority;
  createdByUserId: string;
  assignedToUserId: string | null;
  createdByUserFullName: string | null;
  assignedToUserFullName: string | null;
  createdAt: string;
  updatedAt: string;
  commentCount: number;
}

export interface TicketQuery {
  search?: string;
  status?: TicketStatus;
  priority?: TicketPriority;
  assignedToUserId?: string;
  createdByUserId?: string;
  page?: number;
  pageSize?: number;
}

export interface TicketCreateRequest {
  title: string;
  description: string;
  priority: TicketPriority;
  assignedToUserId: string;
}

export interface TicketUpdateRequest {
  title: string;
  description: string;
  priority: TicketPriority;
}

export interface TicketStatusUpdateRequest {
  status: TicketStatus;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export function getStatusLabel(status: TicketStatus): string {
  switch (status) {
    case TicketStatus.Open: return 'Açık';
    case TicketStatus.InProgress: return 'Devam Ediyor';
    case TicketStatus.Done: return 'Tamamlandı';
    default: return 'Bilinmiyor';
  }
}

export function getPriorityLabel(priority: TicketPriority): string {
  switch (priority) {
    case TicketPriority.Low: return 'Düşük';
    case TicketPriority.Medium: return 'Orta';
    case TicketPriority.High: return 'Yüksek';
    default: return 'Bilinmiyor';
  }
}

export function getStatusColor(status: TicketStatus): string {
  switch (status) {
    case TicketStatus.Open: return '#3b82f6';
    case TicketStatus.InProgress: return '#f59e0b';
    case TicketStatus.Done: return '#10b981';
    default: return '#6b7280';
  }
}

export function getPriorityColor(priority: TicketPriority): string {
  switch (priority) {
    case TicketPriority.Low: return '#10b981';
    case TicketPriority.Medium: return '#f59e0b';
    case TicketPriority.High: return '#ef4444';
    default: return '#6b7280';
  }
}
