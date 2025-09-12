import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { Task } from '../../models/task.model';

export const TaskActions = createActionGroup({
  source: 'Task',
  events: {
    'Load Tasks': emptyProps(),
    'Load Tasks Success': props<{ tasks: Task[] }>(),
    'Load Tasks Failure': props<{ error: string }>(),

    'Add Task': props<{ task: Omit<Task, 'id' | 'createdAt'> }>(),
    'Add Task Success': props<{ task: Task }>(),
    'Add Task Failure': props<{ error: string }>(),

    'Update Task Status': props<{ taskId: string; completed: boolean }>(),
    'Update Task Status Success': props<{ task: Task }>(),
    'Update Task Status Failure': props<{ error: string }>(),

    'Delete Task': props<{ taskId: string }>(),
    'Delete Task Success': props<{ taskId: string }>(),
    'Delete Task Failure': props<{ error: string }>(),

    // Local Storage Actions
    'Save Tasks To Local Storage': props<{ tasks: Task[] }>(),
    'Clear Local Storage': emptyProps(),
  }
});