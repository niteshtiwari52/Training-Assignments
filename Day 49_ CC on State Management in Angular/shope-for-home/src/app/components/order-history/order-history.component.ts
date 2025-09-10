import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../services/order.service';
import { Order } from '../../models/app.models';

@Component({
  selector: 'app-order-history',
  templateUrl: './order-history.component.html',
  standalone: false,
  styleUrls: ['./order-history.component.css']
})
export class OrderHistoryComponent implements OnInit {
  orders: Order[] = [];
  selectedOrder: Order | null = null;
  isLoading = false;

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.isLoading = true;
    this.orderService.getUserOrders().subscribe(response => {
      this.isLoading = false;
      if (response.success) {
        this.orders = response.data;
      }
    });
  }

  viewOrderDetails(order: Order): void {
    this.selectedOrder = order;
    console.log("selected ordr:", this.selectedOrder)
  }

  cancelOrder(orderId: number): void {
    if (confirm('Are you sure you want to cancel this order?')) {
      this.orderService.cancelOrder(orderId).subscribe(response => {
        if (response.success) {
          alert('Order cancelled successfully!');
          this.loadOrders();
          this.selectedOrder = null;
        } else {
          alert('Error cancelling order: ' + response.message);
        }
      });
    }
  }

  getImageUrl(imagePath: string | undefined): string {
    if (!imagePath) return '/assets/placeholder-product.png';
    if (imagePath.startsWith('http')) return imagePath;
    return `https://localhost:7086${imagePath}`;
  }
}