import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent implements OnInit, OnDestroy {
  shipping = {
    name: '',
    email: '',
    address: '',
    city: '',
    postalCode: '',
    country: '',
    method: 'Standard'
  };

  paymentMethod = 'card';
  itemCount = 0;
  subtotal = 0;
  shippingCharge = 0;
  tax = 0;
  total = 0;
  cartItems: any[] = [];
  successMessage = '';
  errorMessage = '';
  orderId: number | null = null;
  trackingNumber = '';
  trackingStatus = '';

  private subscription = new Subscription();

  constructor(
    private cartService: CartService,
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.subscription.add(
      this.cartService.cartItems$.subscribe(items => {
        this.cartItems = items;
        this.itemCount = items.reduce((count, item) => count + item.quantity, 0);
        this.subtotal = this.cartService.subtotal;
        this.tax = this.cartService.tax;
        this.updateShippingCharge();
      })
    );
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  updateShippingCharge(): void {
    switch (this.shipping.method) {
      case 'Express':
        this.shippingCharge = 120;
        break;
      case 'Next Day':
        this.shippingCharge = 200;
        break;
      default:
        this.shippingCharge = 50;
        break;
    }
    this.total = this.subtotal + this.tax + this.shippingCharge;
  }

  submit(): void {
    if (!this.authService.isLoggedIn()) {
      this.errorMessage = 'You must log in to place an order.';
      return;
    }

    if (this.cartItems.length === 0) {
      this.errorMessage = 'Your cart is empty.';
      return;
    }

    if (!this.shipping.email || !this.shipping.address || !this.shipping.city || !this.shipping.postalCode || !this.shipping.country) {
      this.errorMessage = 'Please fill in all shipping details.';
      return;
    }

    const userId = this.authService.getUserId();
    if (!userId) {
      this.errorMessage = 'Unable to determine your user account. Please login again.';
      return;
    }

    const request = {
      userId,
      shippingAddress: `${this.shipping.address}, ${this.shipping.city}, ${this.shipping.postalCode}, ${this.shipping.country}`,
      shippingEmail: this.shipping.email,
      shippingMethod: this.shipping.method,
      paymentMethod: this.paymentMethod,
      tax: this.tax,
      shippingCharge: this.shippingCharge,
      currency: 'INR',
      items: this.cartItems.map(item => ({ fruitId: item.id, quantity: item.quantity }))
    };

    this.apiService.checkout(request).subscribe({
      next: response => {
        if (response?.success) {
          this.successMessage = response.message;
          this.errorMessage = '';
          this.orderId = response.data?.id || response.data?.orderId || null;
          this.trackingNumber = response.data?.trackingNumber || '';
          this.trackingStatus = response.data?.trackingStatus || '';
          this.cartService.clearCart();
        } else {
          this.errorMessage = response?.message || 'Checkout failed.';
          this.successMessage = '';
        }
      },
      error: err => {
        this.errorMessage = err?.error?.message || 'Checkout failed. Please try again.';
        this.successMessage = '';
      }
    });
  }
}
