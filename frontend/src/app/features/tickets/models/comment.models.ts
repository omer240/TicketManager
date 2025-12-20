export interface CommentDto {
  id: number;
  ticketId: number;
  text: string;
  createdByUserId: string;
  createdAt: string;
}

export interface CommentCreateRequest {
  ticketId: number;
  text: string;
}

export interface CommentUpdateRequest {
  text: string;
}
