import { Component, signal, ViewChild, AfterViewInit, OnInit } from '@angular/core';
import { Router, RouterOutlet, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { ToastComponent } from './shared/components/toast/toast.component';
import { ToastService } from './shared/services/toast.service';
import { RecentPagesComponent } from './shared/components/recent-pages/recent-pages.component';
import { RecentPagesService } from './shared/services/recent-pages.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ToastComponent, RecentPagesComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements AfterViewInit, OnInit {
  protected readonly title = signal('ticket-manager-frontend');
  
  @ViewChild(ToastComponent) toastComponent!: ToastComponent;

  constructor(
    private toastService: ToastService,
    private router: Router,
    private recentPagesService: RecentPagesService
  ) {}

  ngOnInit(): void {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        const title = this.getTitleFromUrl(event.urlAfterRedirects);
        this.recentPagesService.addPage(event.urlAfterRedirects, title);
      });
  }

  ngAfterViewInit(): void {
    this.toastService.toast$.subscribe(toast => {
      this.toastComponent.showToast(toast.message, toast.type, toast.duration);
    });
  }

  private getTitleFromUrl(url: string): string {
    const cleanUrl = url.split('?')[0];
    
    if (cleanUrl.includes('/tickets/create')) return 'Yeni Talep';
    if (cleanUrl.match(/\/tickets\/\d+/)) {
      const id = cleanUrl.split('/').pop();
      return `Talep #${id}`;
    }
    if (cleanUrl.includes('/tickets')) return 'Taleplerim';
    
    return cleanUrl.substring(1) || 'Ana Sayfa';
  }
}
