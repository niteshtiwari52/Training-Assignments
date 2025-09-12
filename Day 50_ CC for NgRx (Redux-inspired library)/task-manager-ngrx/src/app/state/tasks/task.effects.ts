import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType, OnInitEffects } from '@ngrx/effects';
import { Action } from '@ngrx/store';
import { of } from 'rxjs';
import { map, catchError, tap, withLatestFrom } from 'rxjs/operators';
import { Store } from '@ngrx/store';
import { v4 as uuidv4 } from 'uuid';

import { TaskActions } from './task.actions';
import { LocalStorageService } from '../../services/local-storage.service';
import { selectTasks } from './task.selectors';
import { Task } from '../../models/task.model';

@Injectable()
export class TaskEffects implements OnInitEffects {

  // Load tasks from localStorage on app init
  loadTasks$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TaskActions.loadTasks),
      map(() => {
        try {
          const tasks = this.localStorageService.getTasks();
          return TaskActions.loadTasksSuccess({ tasks });
        } catch (error) {
          return TaskActions.loadTasksFailure({ 
            error: 'Failed to load tasks from localStorage' 
          });
        }
      })
    )
  );

  // Add new task
  addTask$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TaskActions.addTask),
      map(({ task }) => {
        try {
          const newTask: Task = {
            ...task,
            id: uuidv4(),
            createdAt: new Date(),
            updatedAt: new Date()
          };
          return TaskActions.addTaskSuccess({ task: newTask });
        } catch (error) {
          return TaskActions.addTaskFailure({ 
            error: 'Failed to add task' 
          });
        }
      })
    )
  );

  // Update task status
  updateTaskStatus$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TaskActions.updateTaskStatus),
      withLatestFrom(this.store.select(selectTasks)),
      map(([{ taskId, completed }, tasks]) => {
        try {
          const taskToUpdate = tasks.find(t => t.id === taskId);
          if (!taskToUpdate) {
            return TaskActions.updateTaskStatusFailure({ 
              error: 'Task not found' 
            });
          }

          const updatedTask: Task = {
            ...taskToUpdate,
            completed,
            updatedAt: new Date()
          };

          return TaskActions.updateTaskStatusSuccess({ task: updatedTask });
        } catch (error) {
          return TaskActions.updateTaskStatusFailure({ 
            error: 'Failed to update task status' 
          });
        }
      })
    )
  );

  // Delete task
  deleteTask$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TaskActions.deleteTask),
      map(({ taskId }) => {
        try {
          return TaskActions.deleteTaskSuccess({ taskId });
        } catch (error) {
          return TaskActions.deleteTaskFailure({ 
            error: 'Failed to delete task' 
          });
        }
      })
    )
  );

  // Save tasks to localStorage whenever tasks change
  saveTasksToLocalStorage$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        TaskActions.addTaskSuccess,
        TaskActions.updateTaskStatusSuccess,
        TaskActions.deleteTaskSuccess,
        TaskActions.loadTasksSuccess
      ),
      withLatestFrom(this.store.select(selectTasks)),
      tap(([, tasks]) => {
        this.localStorageService.saveTasks(tasks);
      })
    ),
    { dispatch: false }
  );

  // Clear localStorage
  clearLocalStorage$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TaskActions.clearLocalStorage),
      tap(() => {
        this.localStorageService.clearTasks();
      })
    ),
    { dispatch: false }
  );

  constructor(
    private actions$: Actions,
    private store: Store,
    private localStorageService: LocalStorageService
  ) {}

  // Initialize loading tasks when the app starts
  ngrxOnInitEffects(): Action {
    return TaskActions.loadTasks();
  }
}