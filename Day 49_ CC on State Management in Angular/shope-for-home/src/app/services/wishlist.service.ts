import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { WishlistItem, ApiResponse } from '../models/app.models';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class WishlistService {
  private apiUrl = environment.apiUrl;
  private wishlistItemsSubject = new BehaviorSubject<WishlistItem[]>([]);
  public wishlistItems$ = this.wishlistItemsSubject.asObservable();

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {
    this.loadWishlistItems();
  }

  private loadWishlistItems(): void {
    if (this.isAuthenticated()) {
      this.getWishlistItems().subscribe();
    }
  }

  private isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }

  getWishlistItems(): Observable<ApiResponse<WishlistItem[]>> {
    return this.http.get<ApiResponse<WishlistItem[]>>(`${this.apiUrl}/Wishlist`)
      .pipe(
        tap(response => {
          if (response.success) {
            this.wishlistItemsSubject.next(response.data);
          }
        })
      );
  }

  addToWishlist(productId: number): Observable<ApiResponse<any>> {
    const userId = this.authService.getUserId();
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/Wishlist/add`, {
      userId,
      productId
    }).pipe(
      tap(response => {
        if (response.success) {
          this.getWishlistItems().subscribe();
        }
      })
    );
  }

  removeFromWishlist(productId: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/Wishlist/remove/${productId}`)
      .pipe(
        tap(response => {
          if (response.success) {
            this.getWishlistItems().subscribe();
          }
        })
      );
  }

  moveToCart(productId: number, quantity: number = 1): Observable<ApiResponse<any>> {
    const userId = this.authService.getUserId();
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/Wishlist/move-to-cart`, {
      userId,
      productId,
      quantity
    }).pipe(
      tap(response => {
        if (response.success) {
          this.getWishlistItems().subscribe();
        }
      })
    );
  }
}