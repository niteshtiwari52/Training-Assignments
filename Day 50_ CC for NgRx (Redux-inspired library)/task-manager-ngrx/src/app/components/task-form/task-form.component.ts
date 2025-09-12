import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Store } from '@ngrx/store';
import { CommonModule } from '@angular/common';

import { TaskActions } from '../../state/tasks/task.actions';
import { AppState } from '../../models/app-state.model';
import { selectTasksLoading, selectTasksError } from '../../state/tasks/task.selectors';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './task-form.component.html',
  styleUrls: ['./task-form.component.scss']
})
export class TaskFormComponent {
  taskForm: FormGroup;
  loading$: Observable<boolean>;
  error$: Observable<string | null>;

  constructor(
    private fb: FormBuilder,
    private store: Store<AppState>
  ) {
    this.taskForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(500)]]
    });

    this.loading$ = this.store.select(selectTasksLoading);
    this.error$ = this.store.select(selectTasksError);
  }

  onSubmit(): void {
    if (this.taskForm.valid) {
      const { title, description } = this.taskForm.value;

      this.store.dispatch(TaskActions.addTask({
        task: {
          title: title.trim(),
          description: description.trim(),
          completed: false
        }
      }));

      // Reset form after submission
      this.taskForm.reset();

      // Mark form as untouched to reset validation states
      this.taskForm.markAsUntouched();
    } else {
      // Mark all fields as touched to show validation errors
      this.taskForm.markAllAsTouched();
    }
  }

  // Helper methods for template
  get titleControl() {
    return this.taskForm.get('title');
  }

  get descriptionControl() {
    return this.taskForm.get('description');
  }

  get isFormInvalid(): boolean {
    return this.taskForm.invalid;
  }
}