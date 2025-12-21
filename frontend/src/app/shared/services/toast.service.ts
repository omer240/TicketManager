import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface ToastMessage {
  message: string;
  type: 'success' | 'error' | 'info' | 'warning';
  duration?: number;
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private toastSubject = new Subject<ToastMessage>();
  toast$ = this.toastSubject.asObservable();

  success(message: string, duration: number = 3000): void {
    this.toastSubject.next({ message, type: 'success', duration });
  }

  error(message: string, duration: number = 4000): void {
    this.toastSubject.next({ message, type: 'error', duration });
  }

  info(message: string, duration: number = 3000): void {
    this.toastSubject.next({ message, type: 'info', duration });
  }

  warning(message: string, duration: number = 3000): void {
    this.toastSubject.next({ message, type: 'warning', duration });
  }
}
