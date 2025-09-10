import { Component, OnInit } from '@angular/core';
import { WishlistService } from '../../services/wishlist.service';
import { CartService } from '../../services/cart.service';
import { WishlistItem } from '../../models/app.models';

@Component({
  selector: 'app-wishlist',
  templateUrl: './wishlist.component.html',
  standalone: false,
  styleUrls: ['./wishlist.component.css']
})
export class WishlistComponent implements OnInit {
  wishlistItems: WishlistItem[] = [];

  constructor(
    private wishlistService: WishlistService,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    this.loadWishlistItems();
  }

  loadWishlistItems(): void {
    this.wishlistService.wishlistItems$.subscribe(items => {
      this.wishlistItems = items;
    });
  }

  removeFromWishlist(productId: number): void {
    this.wishlistService.removeFromWishlist(productId).subscribe(response => {
      if (response.success) {
        alert('Product removed from wishlist!');
      } else {
        alert('Error: ' + response.message);
      }
    });
  }

  moveToCart(productId: number): void {
    this.wishlistService.moveToCart(productId, 1).subscribe(response => {
      if (response.success) {
        alert('Product moved to cart!');
        this.loadWishlistItems();
      } else {
        alert('Error: ' + response.message);
      }
    });
  }

  getImageUrl(imagePath: string | undefined): string {
    if (!imagePath) return '/assets/placeholder-product.png';
    if (imagePath.startsWith('http')) return imagePath;
    return `https://localhost:7086${imagePath}`;
  }
}