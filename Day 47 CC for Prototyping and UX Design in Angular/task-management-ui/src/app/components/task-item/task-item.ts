import { Component, Input } from '@angular/core';
// import { Task } from 'src/app/services/task.service';
import { Task, TaskService } from '../../services/task';
import { CommonModule } from '@angular/common';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'app-task-item',
  standalone: true,
  imports: [CommonModule, MatCheckboxModule, MatListModule],
  templateUrl: './task-item.html',
  styleUrls: ['./task-item.css']
})
export class TaskItemComponent {
  @Input() task!: Task;

  constructor(private taskService: TaskService) {}

  onToggle(): void {
    this.task.completed = !this.task.completed;
    this.taskService.updateTask(this.task);
  }
}