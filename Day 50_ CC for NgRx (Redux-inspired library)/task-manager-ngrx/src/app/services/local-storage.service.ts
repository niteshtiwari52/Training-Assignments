import { Injectable } from '@angular/core';
import { Task } from '../models/task.model';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {
  private readonly TASKS_KEY = 'task-manager-tasks';

  constructor() { }

  /**
   * Save tasks to localStorage
   */
  saveTasks(tasks: Task[]): void {
    try {
      const serializedTasks = JSON.stringify(tasks);
      localStorage.setItem(this.TASKS_KEY, serializedTasks);
    } catch (error) {
      console.error('Error saving tasks to localStorage:', error);
    }
  }

  /**
   * Get tasks from localStorage
   */
  getTasks(): Task[] {
    try {
      const serializedTasks = localStorage.getItem(this.TASKS_KEY);
      if (serializedTasks) {
        const tasks = JSON.parse(serializedTasks);
        // Convert date strings back to Date objects
        return tasks.map((task: any) => ({
          ...task,
          createdAt: new Date(task.createdAt),
          updatedAt: task.updatedAt ? new Date(task.updatedAt) : undefined
        }));
      }
      return [];
    } catch (error) {
      console.error('Error loading tasks from localStorage:', error);
      return [];
    }
  }

  /**
   * Clear all tasks from localStorage
   */
  clearTasks(): void {
    try {
      localStorage.removeItem(this.TASKS_KEY);
    } catch (error) {
      console.error('Error clearing tasks from localStorage:', error);
    }
  }

  /**
   * Check if localStorage is available
   */
  isLocalStorageAvailable(): boolean {
    try {
      const test = 'test';
      localStorage.setItem(test, test);
      localStorage.removeItem(test);
      return true;
    } catch {
      return false;
    }
  }

  /**
   * Get storage size
   */
  getStorageSize(): string {
    try {
      const tasks = localStorage.getItem(this.TASKS_KEY);
      return tasks ? `${(tasks.length / 1024).toFixed(2)} KB` : '0 KB';
    } catch {
      return 'Unknown';
    }
  }
}