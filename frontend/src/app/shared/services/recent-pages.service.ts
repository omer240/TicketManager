import { Injectable, signal } from '@angular/core';

export interface RecentPage {
  url: string;
  title: string;
  visitedAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class RecentPagesService {
  private readonly STORAGE_KEY = 'recent_pages';
  private readonly MAX_ITEMS = 3;
  
  private pagesSignal = signal<RecentPage[]>([]);
  
  readonly pages = this.pagesSignal.asReadonly();

  constructor() {
    this.loadFromStorage();
  }

  addPage(url: string, title: string): void {
    if (url.startsWith('/auth')) {
      return;
    }

    const pages = this.pagesSignal();
    
    const filtered = pages.filter(p => p.url !== url);
    
    const newPage: RecentPage = {
      url,
      title,
      visitedAt: new Date().toISOString()
    };
    
    const updated = [newPage, ...filtered].slice(0, this.MAX_ITEMS);
    
    this.pagesSignal.set(updated);
    this.saveToStorage(updated);
  }

  clear(): void {
    this.pagesSignal.set([]);
    localStorage.removeItem(this.STORAGE_KEY);
  }

  private loadFromStorage(): void {
    try {
      const stored = localStorage.getItem(this.STORAGE_KEY);
      if (stored) {
        const pages = JSON.parse(stored) as RecentPage[];
        this.pagesSignal.set(pages);
      }
    } catch (error) {
      console.error('Error loading recent pages:', error);
      this.pagesSignal.set([]);
    }
  }

  private saveToStorage(pages: RecentPage[]): void {
    try {
      localStorage.setItem(this.STORAGE_KEY, JSON.stringify(pages));
    } catch (error) {
      console.error('Error saving recent pages:', error);
    }
  }
}
