import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [NgClass],
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.css']
})
export class NotificationComponent implements OnInit, OnDestroy {
  @Input() message: string = '';
  @Input() type: 'success' | 'error' | 'warning' | 'info' = 'info';
  @Input() autoDismiss: boolean = true;
  @Input() dismissTime: number = 3000;
  
  @Output() closed = new EventEmitter<void>();

  private timeoutId?: number;

  ngOnInit(): void {
    if (this.autoDismiss) {
      this.timeoutId = window.setTimeout(() => {
        this.closeNotification();
      }, this.dismissTime);
    }
  }

  ngOnDestroy(): void {
    if (this.timeoutId) {
      clearTimeout(this.timeoutId);
    }
  }

  closeNotification(): void {
    this.closed.emit();
  }

  getIcon(): string {
    const icons = {
      success: '✓',
      error: '✗',
      warning: '⚠',
      info: 'ℹ'
    };
    
    return icons[this.type] || icons.info;
  }
}