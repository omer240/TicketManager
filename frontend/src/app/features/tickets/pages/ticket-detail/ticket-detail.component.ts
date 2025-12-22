import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { TicketService } from '../../services/ticket.service';
import { CommentService } from '../../services/comment.service';
import { AuthService } from '../../../../core/services/auth.service';
import {
  TicketDto,
  TicketStatus,
  TicketPriority,
  TicketUpdateRequest,
  getStatusLabel,
  getPriorityLabel,
  getStatusColor,
  getPriorityColor
} from '../../models/ticket.models';
import { CommentDto, CommentUpdateRequest } from '../../models/comment.models';
import { CommentListComponent } from '../../components/comment-list/comment-list.component';
import { CommentFormComponent } from '../../components/comment-form/comment-form.component';
import { ConfirmModalComponent } from '../../../../shared/components/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-ticket-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CommentListComponent, CommentFormComponent, ConfirmModalComponent],
  templateUrl: './ticket-detail.component.html',
  styleUrl: './ticket-detail.component.scss'
})
export class TicketDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private ticketService = inject(TicketService);
  private commentService = inject(CommentService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);

  ticket: TicketDto | null = null;
  isLoading = false;
  errorMessage = '';
  isEditMode = false;
  isSaving = false;
  showDeleteModal = false;

  comments: CommentDto[] = [];
  isLoadingComments = false;
  isSubmittingComment = false;
  editingComment: CommentDto | null = null;
  commentError = '';

  editForm!: FormGroup;

  TicketStatus = TicketStatus;
  TicketPriority = TicketPriority;
  statusOptions = [
    { value: TicketStatus.Open, label: 'Açık' },
    { value: TicketStatus.InProgress, label: 'Devam Ediyor' },
    { value: TicketStatus.Done, label: 'Tamamlandı' }
  ];
  priorityOptions = [
    { value: TicketPriority.Low, label: 'Düşük' },
    { value: TicketPriority.Medium, label: 'Orta' },
    { value: TicketPriority.High, label: 'Yüksek' }
  ];

  ngOnInit(): void {
    const ticketId = this.route.snapshot.paramMap.get('id');
    if (ticketId) {
      this.loadTicket(+ticketId);
      this.loadComments(+ticketId);
    }
  }

  loadTicket(ticketId: number): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.ticketService.getDetail(ticketId).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.ticket = response.data;
          this.initializeForm();
        } else {
          this.errorMessage = response.error?.message || 'Talep yüklenemedi';
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Load ticket error:', error);
        this.errorMessage = 'Talep yüklenemedi';
        this.isLoading = false;
      }
    });
  }

  loadComments(ticketId: number): void {
    this.isLoadingComments = true;
    this.commentError = '';

    this.commentService.getByTicket(ticketId).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.comments = response.data;
        } else {
          this.commentError = 'Yorumlar yüklenemedi';
        }
        this.isLoadingComments = false;
      },
      error: (error) => {
        console.error('Load comments error:', error);
        this.commentError = 'Yorumlar yüklenemedi';
        this.isLoadingComments = false;
      }
    });
  }

  initializeForm(): void {
    if (!this.ticket) return;

    this.editForm = this.fb.group({
      title: [this.ticket.title, [Validators.required, Validators.maxLength(200)]],
      description: [this.ticket.description, [Validators.required, Validators.maxLength(2000)]],
      priority: [this.ticket.priority, Validators.required]
    });
  }

  toggleEditMode(): void {
    this.isEditMode = !this.isEditMode;
    if (!this.isEditMode && this.ticket) {
      this.initializeForm();
    }
  }

  saveChanges(): void {
    if (!this.ticket || !this.editForm.valid) {
      this.editForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    const request: TicketUpdateRequest = {
      title: this.editForm.value.title,
      description: this.editForm.value.description,
      priority: this.editForm.value.priority
    };

    this.ticketService.update(this.ticket.id, request).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.ticket = response.data;
          this.isEditMode = false;
          this.initializeForm();
        } else {
          this.errorMessage = response.error?.message || 'Talep güncellenemedi';
        }
        this.isSaving = false;
      },
      error: (error) => {
        console.error('Update ticket error:', error);
        this.errorMessage = error.error?.error?.message || 'Talep güncellenemedi';
        this.isSaving = false;
      }
    });
  }

  updateStatus(newStatus: TicketStatus): void {
    if (!this.ticket) return;

    this.ticketService.updateStatus(this.ticket.id, newStatus).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.ticket = response.data;
          this.initializeForm();
        }
      },
      error: (error) => {
        console.error('Update status error:', error);
        this.errorMessage = 'Durum güncellenemedi';
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/tickets']);
  }

  showDeleteConfirmation(): void {
    this.showDeleteModal = true;
  }

  cancelDelete(): void {
    this.showDeleteModal = false;
  }

  confirmDelete(): void {
    if (!this.ticket) return;

    this.showDeleteModal = false;
    this.isSaving = true;
    this.errorMessage = '';

    this.ticketService.delete(this.ticket.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.router.navigate(['/tickets']);
        } else {
          this.errorMessage = response.error?.message || 'Talep silinemedi';
          this.isSaving = false;
        }
      },
      error: (error) => {
        console.error('Delete ticket error:', error);
        this.errorMessage = error.error?.error?.message || 'Talep silinemedi';
        this.isSaving = false;
      }
    });
  }

  // Comment methods
  onSubmitComment(text: string): void {
    if (!this.ticket) return;

    this.isSubmittingComment = true;
    this.commentError = '';

    if (this.editingComment) {
      const request: CommentUpdateRequest = { text };
      this.commentService.update(this.editingComment.id, request).subscribe({
        next: (response) => {
          if (response.success && response.data) {
            const index = this.comments.findIndex(c => c.id === this.editingComment!.id);
            if (index !== -1) {
              this.comments[index] = response.data;
            }
            this.editingComment = null;
          } else {
            this.commentError = 'Yorum güncellenemedi';
          }
          this.isSubmittingComment = false;
        },
        error: (error) => {
          console.error('Update comment error:', error);
          this.commentError = 'Yorum güncellenemedi';
          this.isSubmittingComment = false;
        }
      });
    } else {
      this.commentService.add(this.ticket.id, text).subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.comments.push(response.data);
            if (this.ticket) {
              this.ticket.commentCount = this.comments.length;
            }
          } else {
            this.commentError = 'Yorum eklenemedi';
          }
          this.isSubmittingComment = false;
        },
        error: (error) => {
          console.error('Add comment error:', error);
          this.commentError = 'Yorum eklenemedi';
          this.isSubmittingComment = false;
        }
      });
    }
  }

  onEditComment(comment: CommentDto): void {
    this.editingComment = comment;
  }

  onCancelEdit(): void {
    this.editingComment = null;
  }

  onDeleteComment(commentId: number): void {
    this.commentService.delete(commentId).subscribe({
      next: (response) => {
        if (response.success) {
          this.comments = this.comments.filter(c => c.id !== commentId);
          // Update ticket comment count
          if (this.ticket) {
          }
        } else {
          this.commentError = 'Yorum silinemedi';
        }
      },
      error: (error) => {
        console.error('Delete comment error:', error);
        this.commentError = 'Yorum silinemedi';
      }
    });
  }

  get currentUserId(): string | null {
    return this.authService.getCurrentUser()?.userId || null;
  }

  getStatusLabel = getStatusLabel;
  getPriorityLabel = getPriorityLabel;
  getStatusColor = getStatusColor;
  getPriorityColor = getPriorityColor;

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  get canEdit(): boolean {
    const userId = this.currentUserId;
    return !!this.ticket && !!userId && this.ticket.createdByUserId === userId;
  }

  get canDelete(): boolean {
    const userId = this.currentUserId;
    return !!this.ticket && !!userId && this.ticket.createdByUserId === userId;
  }

  get canEditStatus(): boolean {
    const userId = this.currentUserId;
    return !!this.ticket && !!userId && this.ticket.assignedToUserId === userId && !this.isEditMode;
  }
}
