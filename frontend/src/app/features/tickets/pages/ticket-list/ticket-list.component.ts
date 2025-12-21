import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { TicketService } from '../../services/ticket.service';
import {
  TicketDto,
  TicketQuery,
  TicketStatus,
  TicketPriority,
  getStatusLabel,
  getPriorityLabel,
  getStatusColor,
  getPriorityColor
} from '../../models/ticket.models';
import { AuthService } from '../../../../core/services/auth.service';

type ViewMode = 'myCreated' | 'myAssigned';

@Component({
  selector: 'app-ticket-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './ticket-list.component.html',
  styleUrl: './ticket-list.component.scss'
})
export class TicketListComponent implements OnInit {
  // Expose Math to template
  protected readonly Math = Math;
  private ticketService = inject(TicketService);
  private authService = inject(AuthService);
  private router = inject(Router);

  tickets: TicketDto[] = [];
  isLoading = false;
  errorMessage = '';
  
  // Pagination
  currentPage = 1;
  pageSize = 20;
  totalCount = 0;
  totalPages = 0;

  // View mode
  viewMode: ViewMode = 'myCreated';

  // Filters
  filterForm = new FormGroup({
    search: new FormControl(''),
    status: new FormControl<TicketStatus | ''>(''),
    priority: new FormControl<TicketPriority | ''>('')
  });

  // Enums for template
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
    this.loadTickets();
    this.setupFilterWatchers();
  }

  loadTickets(): void {
    this.isLoading = true;
    this.errorMessage = '';

    const query: TicketQuery = {
      page: this.currentPage,
      pageSize: this.pageSize,
      search: this.filterForm.value.search || undefined,
      status: this.filterForm.value.status || undefined,
      priority: this.filterForm.value.priority || undefined
    };

    const request$ = this.viewMode === 'myCreated'
      ? this.ticketService.getMyCreated(query)
      : this.ticketService.getMyAssigned(query);

    request$.subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.tickets = response.data.items;
          this.totalCount = response.data.totalCount;
          this.currentPage = response.data.page;
          this.pageSize = response.data.pageSize;
          this.totalPages = Math.ceil(this.totalCount / this.pageSize);
        } else {
          this.errorMessage = response.error?.message || 'Talepler yüklenemedi';
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Load tickets error:', error);
        this.errorMessage = 'Talepler yüklenemedi';
        this.isLoading = false;
      }
    });
  }

  setupFilterWatchers(): void {
    // Watch for filter changes and reload
    this.filterForm.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(() => {
        this.currentPage = 1;
        this.loadTickets();
      });
  }

  switchView(mode: ViewMode): void {
    this.viewMode = mode;
    this.currentPage = 1;
    this.loadTickets();
  }

  clearFilters(): void {
    this.filterForm.reset({
      search: '',
      status: '',
      priority: ''
    });
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadTickets();
    }
  }

  navigateToDetail(ticketId: number): void {
    this.router.navigate(['/tickets', ticketId]);
  }

  navigateToCreate(): void {
    this.router.navigate(['/tickets/create']);
  }

  // Helper methods for template
  getStatusLabel = getStatusLabel;
  getPriorityLabel = getPriorityLabel;
  getStatusColor = getStatusColor;
  getPriorityColor = getPriorityColor;

  get currentUser() {
    return this.authService.getCurrentUser();
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
