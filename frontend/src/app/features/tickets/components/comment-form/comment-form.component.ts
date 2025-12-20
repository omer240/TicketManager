import { Component, Input, Output, EventEmitter, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommentDto } from '../../models/comment.models';

@Component({
  selector: 'app-comment-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './comment-form.component.html',
  styleUrl: './comment-form.component.scss'
})
export class CommentFormComponent implements OnInit {
  @Input() ticketId!: number;
  @Input() editingComment: CommentDto | null = null;
  @Input() isSubmitting = false;
  @Output() submitComment = new EventEmitter<string>();
  @Output() cancelEdit = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  commentForm!: FormGroup;

  ngOnInit(): void {
    this.commentForm = this.fb.group({
      text: ['', [Validators.required, Validators.maxLength(2000)]]
    });

    if (this.editingComment) {
      this.commentForm.patchValue({
        text: this.editingComment.text
      });
    }
  }

  ngOnChanges(): void {
    if (this.commentForm) {
      if (this.editingComment) {
        this.commentForm.patchValue({
          text: this.editingComment.text
        });
      } else {
        this.commentForm.reset();
      }
    }
  }

  onSubmit(): void {
    if (this.commentForm.invalid) {
      this.commentForm.markAllAsTouched();
      return;
    }

    this.submitComment.emit(this.commentForm.value.text);
  }

  onCancel(): void {
    this.commentForm.reset();
    this.cancelEdit.emit();
  }

  get text() {
    return this.commentForm.get('text');
  }

  get isEditMode(): boolean {
    return this.editingComment !== null;
  }

  get characterCount(): number {
    return this.commentForm.value.text?.length || 0;
  }
}
