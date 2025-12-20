import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-ticket-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="ticket-list">
      <h3>Ticket List (Coming Soon)</h3>
      <p>This page will display all tickets.</p>
    </div>
  `,
  styles: [`
    .ticket-list {
      padding: 1rem;
    }
  `]
})
export class TicketListComponent {}
