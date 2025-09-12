import { Component, OnInit } from '@angular/core';
import { TaskService } from '../../services/task';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-error-message',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule],
  templateUrl: './error-message.html',
  styleUrls: ['./error-message.css']
})
export class ErrorMessageComponent implements OnInit {
  errorMessage = '';

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.taskService.getError().subscribe(error => {
      this.errorMessage = error;
    });
  }

  onRetry(): void {
    this.errorMessage = '';
    this.taskService.fetchTasks();
  }
}