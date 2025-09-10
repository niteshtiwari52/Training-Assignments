import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Product, Category, ProductResponse, ApiResponse } from '../models/app.models';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getCategories(): Observable<ApiResponse<Category[]>> {
    return this.http.get<ApiResponse<Category[]>>(`${this.apiUrl}/products/categories`);
  }

  getProducts(
    page: number = 1, 
    pageSize: number = 10, 
    categoryId?: number, 
    minPrice?: number, 
    maxPrice?: number
  ): Observable<ApiResponse<ProductResponse>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (categoryId) params = params.set('categoryId', categoryId.toString());
    if (minPrice) params = params.set('minPrice', minPrice.toString());
    if (maxPrice) params = params.set('maxPrice', maxPrice.toString());

    return this.http.get<ApiResponse<ProductResponse>>(`${this.apiUrl}/Products`, { params });
  }

  getProductById(id: number): Observable<ApiResponse<Product>> {
    return this.http.get<ApiResponse<Product>>(`${this.apiUrl}/Products/${id}`);
  }
}