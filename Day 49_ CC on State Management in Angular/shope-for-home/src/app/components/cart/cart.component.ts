import { Component, OnInit } from '@angular/core';
import { CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import { CouponService } from '../../services/coupon.service';
import { CartItem, CartSummary, Coupon } from '../../models/app.models';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  standalone: false,
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  cartItems: CartItem[] = [];
  cartSummary: CartSummary | null = null;
  coupons: Coupon[] = [];
  couponCode = '';
  appliedCoupon: Coupon | null = null;
  isLoading = false;

  constructor(
    private cartService: CartService,
    private orderService: OrderService,
    private couponService: CouponService
  ) {}

  ngOnInit(): void {
    this.loadCartItems();
    this.loadCartSummary();
    this.loadAvailableCoupons();
  }

  loadCartItems(): void {
    this.cartService.cartItems$.subscribe(items => {
      this.cartItems = items;
    });
  }

  loadCartSummary(): void {
    this.cartService.getCartSummary().subscribe(response => {
      if (response.success) {
        this.cartSummary = response.data;
      }
    });
  }

  loadAvailableCoupons(): void {
    this.couponService.getAvailableCoupons().subscribe(response => {
      if (response.success) {
        this.coupons = response.data;
      }
    });
  }

  updateQuantity(item: CartItem, newQuantity: number): void {
    if (newQuantity < 1) newQuantity = 1;
    if (newQuantity > (item.stockQuantity || 1)) {
      alert(`Cannot add more than ${item.stockQuantity} items`);
      return;
    }
    
    this.cartService.updateCartQuantity(item.productId, newQuantity).subscribe(response => {
      if (response.success) {
        this.loadCartSummary();
      } else {
        alert('Error updating quantity: ' + response.message);
      }
    });
  }

  removeItem(productId: number): void {
    this.cartService.removeFromCart(productId).subscribe(response => {
      if (response.success) {
        this.loadCartSummary();
      } else {
        alert('Error removing item: ' + response.message);
      }
    });
  }

  validateAndApplyCoupon(): void {
    if (!this.couponCode.trim()) {
      alert('Please enter a coupon code');
      return;
    }

    this.isLoading = true;
    
    // First validate the coupon
    this.couponService.validateCoupon(this.couponCode).subscribe({
      next: (validationResponse) => {
        if (validationResponse.success) {
          // If valid, apply the coupon
          this.cartService.applyCoupon(this.couponCode).subscribe({
            next: (applyResponse) => {
              this.isLoading = false;
              if (applyResponse.success) {
                alert('Coupon applied successfully!');
                this.appliedCoupon = validationResponse.data;
                this.loadCartSummary();
              } else {
                alert('Error applying coupon: ' + applyResponse.message);
              }
            },
            error: () => {
              this.isLoading = false;
              alert('Error applying coupon');
            }
          });
        } else {
          this.isLoading = false;
          alert('Invalid coupon: ' + validationResponse.message);
        }
      },
      error: () => {
        this.isLoading = false;
        alert('Error validating coupon');
      }
    });
  }

  placeOrder(): void {
    // Get cart item IDs for ordering
    const cartItemIds = this.cartItems.map(item => item.cartId!).filter(id => id !== undefined);
    
    this.orderService.createOrder(cartItemIds).subscribe(response => {
      if (response.success) {
        alert('Order placed successfully!');
        this.cartService.clearCart().subscribe();
        this.cartItems = [];
        this.cartSummary = null;
        this.appliedCoupon = null;
      } else {
        alert('Error placing order: ' + response.message);
      }
    });
  }

  getImageUrl(imagePath: string | undefined): string {
    if (!imagePath) return '/assets/placeholder-product.png';
    if (imagePath.startsWith('http')) return imagePath;
    return `https://localhost:7086${imagePath}`;
  }
}