import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RecentPagesService } from '../../services/recent-pages.service';

@Component({
  selector: 'app-recent-pages',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './recent-pages.component.html',
  styleUrl: './recent-pages.component.scss'
})
export class RecentPagesComponent {
  protected readonly isExpanded = signal(false);

  constructor(
    protected recentPagesService: RecentPagesService,
    private router: Router
  ) {}

  get pages() {
    return this.recentPagesService.pages;
  }

  toggleExpanded(): void {
    this.isExpanded.update(v => !v);
  }

  navigateTo(url: string): void {
    this.router.navigateByUrl(url);
  }

  clearHistory(): void {
    this.recentPagesService.clear();
  }

  getRelativeTime(visitedAt: string): string {
    const now = new Date().getTime();
    const visited = new Date(visitedAt).getTime();
    const diffMs = now - visited;
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return 'Az önce';
    if (diffMins < 60) return `${diffMins} dakika önce`;
    if (diffHours < 24) return `${diffHours} saat önce`;
    return `${diffDays} gün önce`;
  }
}
