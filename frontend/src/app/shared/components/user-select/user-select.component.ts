import { Component, OnInit, Input, Output, EventEmitter, forwardRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormsModule } from '@angular/forms';
import { UserService } from '../../../core/services/user.service';
import { UserDto } from '../../../core/models/user.models';
import { ApiResponse } from '../../../core/models/auth.models';
import { debounceTime, distinctUntilChanged, Subject, switchMap } from 'rxjs';

@Component({
  selector: 'app-user-select',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-select.component.html',
  styleUrl: './user-select.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => UserSelectComponent),
      multi: true
    }
  ]
})
export class UserSelectComponent implements OnInit, ControlValueAccessor {
  private userService = inject(UserService);
  
  @Input() label: string = 'Atanan Kişi';
  @Input() placeholder: string = 'Kullanıcı ara ve seç...';
  @Input() required: boolean = false;
  @Input() disabled: boolean = false;

  users: UserDto[] = [];
  filteredUsers: UserDto[] = [];
  isLoading = false;
  isOpen = false;
  searchText = '';
  selectedUser: UserDto | null = null;
  
  private searchSubject = new Subject<string>();
  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  ngOnInit(): void {
    this.loadUsers();
    
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap((search: string) => {
        this.isLoading = true;
        return this.userService.getAssignees(search);
      })
    ).subscribe({
      next: (response: ApiResponse<UserDto[]>) => {
        if (response.success && response.data) {
          this.users = response.data;
          this.filteredUsers = response.data;
        }
        this.isLoading = false;
      },
      error: (error: any) => {
        console.error('Load users error:', error);
        this.isLoading = false;
      }
    });
  }

  loadUsers(search?: string): void {
    this.isLoading = true;
    this.userService.getAssignees(search).subscribe({
      next: (response: ApiResponse<UserDto[]>) => {
        if (response.success && response.data) {
          this.users = response.data;
          this.filteredUsers = response.data;
        }
        this.isLoading = false;
      },
      error: (error: any) => {
        console.error('Load users error:', error);
        this.isLoading = false;
      }
    });
  }

  onSearchChange(search: string): void {
    this.searchText = search;
    this.searchSubject.next(search);
  }

  toggleDropdown(): void {
    if (this.disabled) return;
    this.isOpen = !this.isOpen;
    if (this.isOpen) {
      this.searchText = '';
      this.loadUsers();
    }
  }

  selectUser(user: UserDto): void {
    this.selectedUser = user;
    this.isOpen = false;
    this.searchText = '';
    this.onChange(user.id);
    this.onTouched();
  }

  clearSelection(): void {
    this.selectedUser = null;
    this.onChange('');
    this.onTouched();
  }

  closeDropdown(): void {
    this.isOpen = false;
    this.searchText = '';
  }

  writeValue(userId: string): void {
    if (userId) {
      const user = this.users.find(u => u.id === userId);
      if (user) {
        this.selectedUser = user;
      } else {
        this.userService.getAssignees().subscribe({
          next: (response: ApiResponse<UserDto[]>) => {
            if (response.success && response.data) {
              const foundUser = response.data.find((u: UserDto) => u.id === userId);
              if (foundUser) {
                this.selectedUser = foundUser;
              }
            }
          }
        });
      }
    } else {
      this.selectedUser = null;
    }
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
