// Enums matching backend
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

// DTOs matching backend
export interface TicketDto {
  id: number;
  title: string;
  description: string;
  status: TicketStatus;
  priority: TicketPriority;
  createdByUserId: string;
  assignedToUserId: string | null;
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
  ticketId: number;
  status: TicketStatus;
}

// Paged result
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

// Helper functions for display
export function getStatusLabel(status: TicketStatus): string {
  switch (status) {
    case TicketStatus.Open: return 'Open';
    case TicketStatus.InProgress: return 'In Progress';
    case TicketStatus.Done: return 'Done';
    default: return 'Unknown';
  }
}

export function getPriorityLabel(priority: TicketPriority): string {
  switch (priority) {
    case TicketPriority.Low: return 'Low';
    case TicketPriority.Medium: return 'Medium';
    case TicketPriority.High: return 'High';
    default: return 'Unknown';
  }
}

export function getStatusColor(status: TicketStatus): string {
  switch (status) {
    case TicketStatus.Open: return '#3b82f6'; // blue
    case TicketStatus.InProgress: return '#f59e0b'; // amber
    case TicketStatus.Done: return '#10b981'; // green
    default: return '#6b7280'; // gray
  }
}

export function getPriorityColor(priority: TicketPriority): string {
  switch (priority) {
    case TicketPriority.Low: return '#10b981'; // green
    case TicketPriority.Medium: return '#f59e0b'; // amber
    case TicketPriority.High: return '#ef4444'; // red
    default: return '#6b7280'; // gray
  }
}
