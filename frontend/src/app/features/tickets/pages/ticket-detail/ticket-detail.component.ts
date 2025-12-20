import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { TicketService } from '../../services/ticket.service';
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

@Component({
  selector: 'app-ticket-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './ticket-detail.component.html',
  styleUrl: './ticket-detail.component.scss'
})
export class TicketDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private ticketService = inject(TicketService);
  private fb = inject(FormBuilder);

  ticket: TicketDto | null = null;
  isLoading = false;
  errorMessage = '';
  isEditMode = false;
  isSaving = false;

  editForm!: FormGroup;

  // Enums for template
  TicketStatus = TicketStatus;
  TicketPriority = TicketPriority;
  statusOptions = [
    { value: TicketStatus.Open, label: 'Open' },
    { value: TicketStatus.InProgress, label: 'In Progress' },
    { value: TicketStatus.Done, label: 'Done' }
  ];
  priorityOptions = [
    { value: TicketPriority.Low, label: 'Low' },
    { value: TicketPriority.Medium, label: 'Medium' },
    { value: TicketPriority.High, label: 'High' }
  ];

  ngOnInit(): void {
    const ticketId = this.route.snapshot.paramMap.get('id');
    if (ticketId) {
      this.loadTicket(+ticketId);
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
          this.errorMessage = response.error?.message || 'Failed to load ticket';
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Load ticket error:', error);
        this.errorMessage = 'Failed to load ticket';
        this.isLoading = false;
      }
    });
  }

  initializeForm(): void {
    if (!this.ticket) return;

    this.editForm = this.fb.group({
      title: [this.ticket.title, [Validators.required, Validators.maxLength(200)]],
      description: [this.ticket.description, [Validators.required, Validators.maxLength(2000)]],
      status: [this.ticket.status, Validators.required],
      priority: [this.ticket.priority, Validators.required],
      assignedToUserId: [this.ticket.assignedToUserId || '', Validators.required]
    });
  }

  toggleEditMode(): void {
    this.isEditMode = !this.isEditMode;
    if (!this.isEditMode && this.ticket) {
      // Reset form when canceling edit
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
      status: this.editForm.value.status,
      priority: this.editForm.value.priority,
      assignedToUserId: this.editForm.value.assignedToUserId
    };

    this.ticketService.update(this.ticket.id, request).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.ticket = response.data;
          this.isEditMode = false;
          this.initializeForm();
        } else {
          this.errorMessage = response.error?.message || 'Failed to update ticket';
        }
        this.isSaving = false;
      },
      error: (error) => {
        console.error('Update ticket error:', error);
        this.errorMessage = error.error?.error?.message || 'Failed to update ticket';
        this.isSaving = false;
      }
    });
  }

  updateStatus(newStatus: TicketStatus): void {
    if (!this.ticket) return;

    this.ticketService.updateStatus({
      ticketId: this.ticket.id,
      status: newStatus
    }).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.ticket = response.data;
          this.initializeForm();
        }
      },
      error: (error) => {
        console.error('Update status error:', error);
        this.errorMessage = 'Failed to update status';
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/tickets']);
  }

  // Helper methods
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

  get canEditStatus(): boolean {
    return !this.isEditMode;
  }
}
