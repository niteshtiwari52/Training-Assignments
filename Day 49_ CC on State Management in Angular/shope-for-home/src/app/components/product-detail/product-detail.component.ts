import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { WishlistService } from '../../services/wishlist.service';
import { AuthService } from '../../services/auth.service';
import { Product } from '../../models/app.models';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  standalone: false,
  styleUrls: ['./product-detail.component.css']
})
export class ProductDetailComponent implements OnInit {
  product: Product | null = null;
  quantity = 1;
  isAuthenticated = false;
  isLoading = false;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private cartService: CartService,
    private wishlistService: WishlistService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    const productId = this.route.snapshot.paramMap.get('id');
    if (productId) {
      this.loadProduct(parseInt(productId, 10));
    }
    
    this.authService.currentUser$.subscribe(user => {
      this.isAuthenticated = !!user;
    });
  }

  loadProduct(id: number): void {
    this.isLoading = true;
    this.productService.getProductById(id).subscribe(response => {
      this.isLoading = false;
      if (response.success) {
        this.product = response.data;
      }
    });
  }

  addToCart(): void {
    if (this.product) {
      this.cartService.addToCart(this.product.productId, this.quantity).subscribe(response => {
        if (response.success) {
          alert('Product added to cart!');
        } else {
          alert('Error: ' + response.message);
        }
      });
    }
  }

  addToWishlist(): void {
    if (this.product) {

      this.wishlistService.addToWishlist(this.product.productId).subscribe({
      next: (response) => {
        if (response.success) {
          alert('✅ Product added to wishlist!');
        } else {
          alert(response.message);
        }
      },
      error: (err) => {
        console.error('Wishlist error:', err);
        alert((err.error?.message || err.message));
      }
    });
    }
  }

  increaseQuantity(): void {
    if (this.product && this.quantity < this.product.stockQuantity) {
      this.quantity++;
    }
  }

  decreaseQuantity(): void {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  getImageUrl(imagePath: string): string {
    if (!imagePath) return '/assets/placeholder-product.png';
    if (imagePath.startsWith('http')) return imagePath;
    return `https://localhost:7086${imagePath}`;
  }
}