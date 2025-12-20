import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { TicketService } from '../../services/ticket.service';
import { TicketCreateRequest, TicketPriority } from '../../models/ticket.models';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-ticket-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './ticket-create.component.html',
  styleUrl: './ticket-create.component.scss'
})
export class TicketCreateComponent implements OnInit {
  private fb = inject(FormBuilder);
  private ticketService = inject(TicketService);
  private authService = inject(AuthService);
  private router = inject(Router);

  createForm!: FormGroup;
  isSubmitting = false;
  errorMessage = '';

  priorityOptions = [
    { value: TicketPriority.Low, label: 'Low' },
    { value: TicketPriority.Medium, label: 'Medium' },
    { value: TicketPriority.High, label: 'High' }
  ];

  ngOnInit(): void {
    this.createForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.required, Validators.maxLength(2000)]],
      priority: [TicketPriority.Medium, Validators.required],
      assignedToUserId: ['', Validators.required]
    });

    // Pre-fill assignedToUserId with current user
    const currentUser = this.authService.getCurrentUser();
    if (currentUser) {
      this.createForm.patchValue({
        assignedToUserId: currentUser.userId
      });
    }
  }

  onSubmit(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    const request: TicketCreateRequest = {
      title: this.createForm.value.title,
      description: this.createForm.value.description,
      priority: this.createForm.value.priority,
      assignedToUserId: this.createForm.value.assignedToUserId
    };

    this.ticketService.create(request).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.router.navigate(['/tickets', response.data.id]);
        } else {
          this.errorMessage = response.error?.message || 'Failed to create ticket';
          this.isSubmitting = false;
        }
      },
      error: (error) => {
        console.error('Create ticket error:', error);
        this.errorMessage = error.error?.error?.message || 'Failed to create ticket';
        this.isSubmitting = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/tickets']);
  }

  get title() {
    return this.createForm.get('title');
  }

  get description() {
    return this.createForm.get('description');
  }

  get priority() {
    return this.createForm.get('priority');
  }

  get assignedToUserId() {
    return this.createForm.get('assignedToUserId');
  }
}
