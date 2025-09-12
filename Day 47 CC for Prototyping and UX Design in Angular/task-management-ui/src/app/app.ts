import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskListComponent } from './components/task-list/task-list';
import { TaskItemComponent } from './components/task-item/task-item';
import { AddTaskComponent } from './components/add-task/add-task';
import { ErrorMessageComponent } from './components/error-message/error-message';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    TaskListComponent,
    TaskItemComponent,
    AddTaskComponent,
    ErrorMessageComponent
  ],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent {
  title = 'Task Management System';
}