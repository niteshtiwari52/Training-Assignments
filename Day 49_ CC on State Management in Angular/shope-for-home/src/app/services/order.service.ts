import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Order, CreateOrderRequest, ApiResponse } from '../models/app.models';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  createOrder(cartItemIds: number[] = []): Observable<ApiResponse<number>> {
    const request: CreateOrderRequest = cartItemIds.length > 0 ? { cartItemIds } : {};
    return this.http.post<ApiResponse<number>>(`${this.apiUrl}/orders/create`, request);
  }

  getUserOrders(): Observable<ApiResponse<Order[]>> {
    return this.http.get<ApiResponse<Order[]>>(`${this.apiUrl}/orders`);
  }

  getOrderDetails(orderId: number): Observable<ApiResponse<Order>> {
    return this.http.get<ApiResponse<Order>>(`${this.apiUrl}/orders/${orderId}`);
  }

  cancelOrder(orderId: number): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/orders/${orderId}/cancel`, {});
  }
}