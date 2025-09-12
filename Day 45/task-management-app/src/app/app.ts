// import { Component, signal } from '@angular/core';

// @Component({
//   selector: 'app-root',
//   templateUrl: './app.html',
//   standalone: false,
//   styleUrl: './app.css'
// })
// export class App {
//   protected readonly title = signal('task-management-app');
// }


// src/app/app.component.ts
import { Component, ViewChild } from '@angular/core';
import { TaskListComponent } from './components/task-list/task-list';

@Component({
  selector: 'app-root',
  standalone: false,
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent {
  title = 'Task Management App';
  
  @ViewChild(TaskListComponent) taskListComponent!: TaskListComponent;

  // This method will be called when a new task is added
  onTaskAdded(): void {
    // Refresh the task list to show the new task
    if (this.taskListComponent) {
      this.taskListComponent.loadTasks();
    }
  }

  // This method will be called when a task is deleted
  onTaskDeleted(): void {
    // Task list automatically updates when a task is deleted
    // This method is here for potential future use (e.g., showing notifications)
    console.log('Task deleted successfully');
  }
}