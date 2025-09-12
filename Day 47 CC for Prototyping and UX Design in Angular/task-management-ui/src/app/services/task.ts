import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface Task {
  userId: number;
  id: number;
  title: string;
  completed: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = 'https://jsonplaceholder.typicode.com/todos?_limit=5';
  private tasks: Task[] = [];
  private tasksSubject = new BehaviorSubject<Task[]>([]);
  private errorSubject = new BehaviorSubject<string>('');

  constructor(private http: HttpClient) { }

  fetchTasks(): void {
    this.http.get<Task[]>(this.apiUrl)
      .pipe(
        catchError(this.handleError.bind(this))
      )
      .subscribe(tasks => {
        this.tasks = tasks;
        this.tasksSubject.next([...this.tasks]);
      });
  }

  getTasks(): Observable<Task[]> {
    return this.tasksSubject.asObservable();
  }

  getError(): Observable<string> {
    return this.errorSubject.asObservable();
  }

  addTask(title: string): void {
    const newTask: Task = {
      userId: 1,
      id: Date.now(), // Generate a unique ID
      title,
      completed: false
    };
    this.tasks.unshift(newTask);
    this.tasksSubject.next([...this.tasks]);
  }

  updateTask(task: Task): void {
    const index = this.tasks.findIndex(t => t.id === task.id);
    if (index !== -1) {
      this.tasks[index] = task;
      this.tasksSubject.next([...this.tasks]);
    }
  }

  completeAll(): void {
    this.tasks = this.tasks.map(task => ({ ...task, completed: true }));
    this.tasksSubject.next([...this.tasks]);
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    this.errorSubject.next(errorMessage);
    return throwError(errorMessage);
  }
}