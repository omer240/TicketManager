import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-tickets',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  template: `
    <div class="tickets-layout">
      <h2>Tickets Feature (Placeholder)</h2>
      <router-outlet></router-outlet>
    </div>
  `,
  styles: [`
    .tickets-layout {
      padding: 2rem;
    }
  `]
})
export class TicketsComponent {}
