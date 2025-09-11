import { Component } from '@angular/core';
import { NgIf } from '@angular/common';
import { NotificationComponent } from './notification.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [NgIf, NotificationComponent],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent {
  showSuccessNotification = false;
  showErrorNotification = false;
  showWarningNotification = false;
  showInfoNotification = false;

  showSuccess() {
    this.showSuccessNotification = true;
  }

  showError() {
    this.showErrorNotification = true;
  }

  showWarning() {
    this.showWarningNotification = true;
  }

  showInfo() {
    this.showInfoNotification = true;
  }

  onNotificationClosed(type: string) {
    console.log(`${type} notification closed`);
    
    switch(type) {
      case 'success':
        this.showSuccessNotification = false;
        break;
      case 'error':
        this.showErrorNotification = false;
        break;
      case 'warning':
        this.showWarningNotification = false;
        break;
      case 'info':
        this.showInfoNotification = false;
        break;
    }
  }
}