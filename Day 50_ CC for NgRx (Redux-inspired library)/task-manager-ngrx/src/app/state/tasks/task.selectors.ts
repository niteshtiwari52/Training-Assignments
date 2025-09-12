import { createFeatureSelector, createSelector } from '@ngrx/store';
import { TaskState } from '../../models/task.model';

// Feature selector
export const selectTaskState = createFeatureSelector<TaskState>('tasks');

// Basic selectors
export const selectTasks = createSelector(
  selectTaskState,
  (state: TaskState) => state.tasks
);

export const selectTasksLoading = createSelector(
  selectTaskState,
  (state: TaskState) => state.loading
);

export const selectTasksError = createSelector(
  selectTaskState,
  (state: TaskState) => state.error
);

// Computed selectors
export const selectCompletedTasks = createSelector(
  selectTasks,
  (tasks) => tasks.filter(task => task.completed)
);

export const selectPendingTasks = createSelector(
  selectTasks,
  (tasks) => tasks.filter(task => !task.completed)
);

export const selectTasksCount = createSelector(
  selectTasks,
  (tasks) => tasks.length
);

export const selectCompletedTasksCount = createSelector(
  selectCompletedTasks,
  (tasks) => tasks.length
);

export const selectPendingTasksCount = createSelector(
  selectPendingTasks,
  (tasks) => tasks.length
);

export const selectTaskById = (taskId: string) => createSelector(
  selectTasks,
  (tasks) => tasks.find(task => task.id === taskId)
);