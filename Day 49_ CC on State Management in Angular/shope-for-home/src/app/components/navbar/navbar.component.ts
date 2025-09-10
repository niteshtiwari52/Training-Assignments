import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { WishlistService } from '../../services/wishlist.service';
import { User } from '../../models/app.models';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  standalone: false,
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  isAuthenticated = false;
  cartItemCount = 0;
  wishlistItemCount = 0;
  currentUser: User | null = null;
  isAdmin = false;

  constructor(
    private authService: AuthService,
    private cartService: CartService,
    private wishlistService: WishlistService
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.isAuthenticated = !!user;
      this.currentUser = user;
      this.isAdmin = this.authService.isAdmin();

    });

    this.cartService.cartItems$.subscribe(items => {
      this.cartItemCount = items.length;
    });

    this.wishlistService.wishlistItems$.subscribe(items => {
      this.wishlistItemCount = items.length;
    });
  }

  openAdminPanel(): void {
    window.open('https://localhost:7086', '_blank');
  }

  logout(): void {
    this.authService.logout();
  }
}