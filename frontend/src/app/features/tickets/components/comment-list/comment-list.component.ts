import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommentDto } from '../../models/comment.models';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-comment-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './comment-list.component.html',
  styleUrl: './comment-list.component.scss'
})
export class CommentListComponent {
  @Input() comments: CommentDto[] = [];
  @Input() isLoading = false;
  @Input() currentUserId: string | null = null;
  @Output() deleteComment = new EventEmitter<number>();
  @Output() editComment = new EventEmitter<CommentDto>();

  onDelete(commentId: number): void {
    if (confirm('Are you sure you want to delete this comment?')) {
      this.deleteComment.emit(commentId);
    }
  }

  onEdit(comment: CommentDto): void {
    this.editComment.emit(comment);
  }

  canModifyComment(comment: CommentDto): boolean {
    return this.currentUserId === comment.createdByUserId;
  }

  formatDate(date: string): string {
    const commentDate = new Date(date);
    const now = new Date();
    const diffMs = now.getTime() - commentDate.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return 'Just now';
    if (diffMins < 60) return `${diffMins} minute${diffMins > 1 ? 's' : ''} ago`;
    if (diffHours < 24) return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`;
    if (diffDays < 7) return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`;

    return commentDate.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }
}
