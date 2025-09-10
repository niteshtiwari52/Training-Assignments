import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Coupon, CouponValidationRequest, ApiResponse } from '../models/app.models';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class CouponService {
  private apiUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) { }

  getAvailableCoupons(): Observable<ApiResponse<Coupon[]>> {
    return this.http.get<ApiResponse<Coupon[]>>(`${this.apiUrl}/Coupons/available`);
  }

  validateCoupon(couponCode: string): Observable<ApiResponse<Coupon>> {
    const userId = this.authService.getUserId();
    return this.http.post<ApiResponse<Coupon>>(`${this.apiUrl}/Coupons/validate`, {
      couponCode,
      userId
    } as CouponValidationRequest);
  }
}