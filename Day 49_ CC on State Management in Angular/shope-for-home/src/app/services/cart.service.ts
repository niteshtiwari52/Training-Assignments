import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { CartItem, CartSummary, ApiResponse } from '../models/app.models';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private apiUrl = environment.apiUrl;
  private cartItemsSubject = new BehaviorSubject<CartItem[]>([]);
  public cartItems$ = this.cartItemsSubject.asObservable();

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {
    this.loadCartItems();
  }

  private loadCartItems(): void {
    if (this.isAuthenticated()) {
      this.getCartItems().subscribe();
    }
  }

  private isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }

  getCartItems(): Observable<ApiResponse<CartItem[]>> {
    return this.http.get<ApiResponse<CartItem[]>>(`${this.apiUrl}/cart`)
      .pipe(
        tap(response => {
          if (response.success) {
            this.cartItemsSubject.next(response.data);
          }
        })
      );
  }

  addToCart(productId: number, quantity: number = 1): Observable<ApiResponse<any>> {
    const userId = this.authService.getUserId();
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/cart/add`, {
      userId,
      productId,
      quantity
    }).pipe(
      tap(() => {
        this.getCartItems().subscribe();
      })
    );
  }

  updateCartQuantity(productId: number, quantity: number): Observable<ApiResponse<any>> {
    const userId = this.authService.getUserId();
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/cart/update`, {
      userId,
      productId,
      quantity
    }).pipe(
      tap(() => {
        this.getCartItems().subscribe();
      })
    );
  }

  removeFromCart(productId: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/cart/remove/${productId}`)
      .pipe(
        tap(() => {
          this.getCartItems().subscribe();
        })
      );
  }

  clearCart(): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/cart/clear`)
      .pipe(
        tap(() => {
          this.cartItemsSubject.next([]);
        })
      );
  }

  getCartSummary(): Observable<ApiResponse<CartSummary>> {
    return this.http.get<ApiResponse<CartSummary>>(`${this.apiUrl}/cart/summary`);
  }

  applyCoupon(couponCode: string): Observable<ApiResponse<any>> {
    const userId = this.authService.getUserId();
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/cart/apply-coupon`, {
      userId,
      couponCode
    });
  }
}