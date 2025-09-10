// User Models
export interface User {
  userId?: number;
  fullName: string;
  email: string;
  isActive?: boolean;
  roles?: string[];
  createdAt?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  success: boolean;
  message: string;
  data: {
    token: string;
    email: string;
    fullName: string;
    roles: string[];
  };
}

// Product Models
export interface Category {
  categoryId: number;
  name: string;
  description: string;
  createdAt?: string;
  updatedAt?: string;
  products?: any[];
}

export interface Product {
  productId: number;
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  gst: number;
  categoryId: number;
  categoryName: string;
  imagePath: string;
}

export interface ProductResponse {
  items: Product[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

// Cart Models
export interface CartItem {
  cartId?: number;
  productId: number;
  productName?: string;
  productImage?: string;
  categoryName?: string;
  unitPrice?: number;
  quantity: number;
  price?: number;
  gst?: number;
  discountAmount?: number;
  finalPrice?: number;
  couponCode?: string | null;
  stockQuantity?: number;
  gstAmount?: number;
  totalPrice?: number;
  product?: Product;
}

export interface CartSummary {
  totalItems: number;
  subTotal: number;
  totalDiscount: number;
  totalGST: number;
  grandTotal: number;
  hasItems: boolean;
  items?: any[];
}

// Wishlist Models
export interface WishlistItem {
  wishlistId?: number;
  productId: number;
  productName?: string;
  productImage?: string;
  categoryName?: string;
  price?: number;
  stockQuantity?: number;
  addedDate?: string;
  isInStock?: boolean;
  product?: Product;
}

// Coupon Models
export interface Coupon {
  couponId: number;
  code: string;
  discountPercent?: number;
  discountValue?: number;
  isPublic?: boolean;
  validFrom: string;
  validTo: string;
  maxUses?: number;
  totalUsed?: number;
  description: string;
  isActive: boolean;
  remainingUses?: number;
  validityText?: string;
  couponType?: string;
  minOrderAmount?: number;
  maxDiscountAmount?: number;
}

export interface CouponValidationRequest {
  couponCode: string;
  userId: number;
}

// Order Models
export interface Order {
  orderId: number;
  userId?: number;
  totalAmount: number;
  discountAmount: number;
  gst: number;
  payableAmount: number;
  balanceAmount: number;
  grandTotal: number;
  status: string;
  createdAt: string;
  orderItems: OrderItem[];
  totalItems?: number;
  canCancel?: boolean;
}

export interface OrderItem {
  orderItemId: number;
  productId: number;
  productName: string;
  productImage?: string;
  quantity: number;
  priceAtPurchase: number;
  totalPrice: number;
}

export interface CreateOrderRequest {
  cartItemIds?: number[];
}

// API Response Models
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}