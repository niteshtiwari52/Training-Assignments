import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { WishlistService } from '../../services/wishlist.service';
import { AuthService } from '../../services/auth.service';
import { Category, Product } from '../../models/app.models';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  standalone: false,
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  categories: Category[] = [];
  filteredProducts: Product[] = [];
  selectedCategory: number | null = null;
  minPrice: number | null = null;
  maxPrice: number | null = null;
  page = 1;
  pageSize = 10;
  totalPages = 1;
  isAuthenticated = false;
  isLoading = false;

  constructor(
    private productService: ProductService,
    private cartService: CartService,
    private wishlistService: WishlistService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();
    this.authService.currentUser$.subscribe(user => {
      this.isAuthenticated = !!user;
    });
  }

  loadCategories(): void {
    this.productService.getCategories().subscribe(response => {
      if (response.success) {
        this.categories = response.data;
      }
    });
  }

  loadProducts(): void {
    this.isLoading = true;
    this.productService.getProducts(
      this.page, 
      this.pageSize, 
      this.selectedCategory || undefined, 
      this.minPrice || undefined, 
      this.maxPrice || undefined
    ).subscribe(response => {
      this.isLoading = false;
      if (response.success) {
        this.products = response.data.items;
        this.filteredProducts = [...this.products];
        this.totalPages = response.data.totalPages;
      }
    });
  }

  applyFilters(): void {
    this.page = 1;
    this.loadProducts();
  }

  clearFilters(): void {
    this.selectedCategory = null;
    this.minPrice = null;
    this.maxPrice = null;
    this.applyFilters();
  }

  addToCart(productId: number): void {
    this.cartService.addToCart(productId).subscribe(response => {
      if (response.success) {
        alert('Product added to cart!');
      } else {
        alert('Error: ' + response.message);
      }
    });
  }

  addToWishlist(productId: number): void {
    this.wishlistService.addToWishlist(productId).subscribe(response => {
      if (response.success) {
        alert('Product added to wishlist!');
      } else {
        alert('Error: ' + response.message);
      }
    });
  }

  changePage(page: number): void {
    this.page = page;
    this.loadProducts();
    window.scrollTo(0, 0);
  }

  getImageUrl(imagePath: string): string {
    if (!imagePath) return '/assets/placeholder-product.png';
    if (imagePath.startsWith('http')) return imagePath;
    return `https://localhost:7086${imagePath}`;
  }
}