import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';

import { TaskActions } from '../../state/tasks/task.actions';
import { AppState } from '../../models/app-state.model';
import { Task } from '../../models/task.model';
import {
  selectTasks,
  selectTasksLoading,
  selectTasksError,
  selectCompletedTasks,
  selectPendingTasks,
  selectTasksCount,
  selectCompletedTasksCount,
  selectPendingTasksCount
} from '../../state/tasks/task.selectors';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.scss']
})
export class TaskListComponent implements OnInit {
  tasks$: Observable<Task[]>;
  loading$: Observable<boolean>;
  error$: Observable<string | null>;
  completedTasks$: Observable<Task[]>;
  pendingTasks$: Observable<Task[]>;
  tasksCount$: Observable<number>;
  completedTasksCount$: Observable<number>;
  pendingTasksCount$: Observable<number>;

  // View state
  showCompleted = true;
  viewMode: 'all' | 'pending' | 'completed' = 'all';

  constructor(private store: Store<AppState>) {
    this.tasks$ = this.store.select(selectTasks);
    this.loading$ = this.store.select(selectTasksLoading);
    this.error$ = this.store.select(selectTasksError);
    this.completedTasks$ = this.store.select(selectCompletedTasks);
    this.pendingTasks$ = this.store.select(selectPendingTasks);
    this.tasksCount$ = this.store.select(selectTasksCount);
    this.completedTasksCount$ = this.store.select(selectCompletedTasksCount);
    this.pendingTasksCount$ = this.store.select(selectPendingTasksCount);
  }

  ngOnInit(): void {
    // Tasks are automatically loaded via TaskEffects OnInitEffects
  }

  onToggleTaskStatus(taskId: string, currentStatus: boolean): void {
    this.store.dispatch(TaskActions.updateTaskStatus({
      taskId,
      completed: !currentStatus
    }));
  }

  onDeleteTask(taskId: string): void {
    if (confirm('Are you sure you want to delete this task?')) {
      this.store.dispatch(TaskActions.deleteTask({ taskId }));
    }
  }

  onClearAllTasks(): void {
    if (confirm('Are you sure you want to clear all tasks? This action cannot be undone.')) {
      this.store.dispatch(TaskActions.clearLocalStorage());
      // Reload tasks to reflect the cleared state
      this.store.dispatch(TaskActions.loadTasks());
    }
  }

  setViewMode(mode: 'all' | 'pending' | 'completed'): void {
    this.viewMode = mode;
  }

  getTasksToDisplay(): Observable<Task[]> {
    switch (this.viewMode) {
      case 'completed':
        return this.completedTasks$;
      case 'pending':
        return this.pendingTasks$;
      default:
        return this.tasks$;
    }
  }

  formatDate(date: Date): string {
    return new Intl.DateTimeFormat('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(new Date(date));
  }

  trackByTaskId(index: number, task: Task): string {
    return task.id;
  }
}