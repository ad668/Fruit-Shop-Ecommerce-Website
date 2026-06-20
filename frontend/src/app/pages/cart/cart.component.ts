import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { CartService, CartItem } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit, OnDestroy {
  cartItems: CartItem[] = [];
  subtotal = 0;
  tax = 0;
  total = 0;

  private subscriptions = new Subscription();

  constructor(
    private cartService: CartService,
    private auth: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (!this.auth.isLoggedIn()) {
      this.router.navigate(['/auth']);
      return;
    }

    this.subscriptions.add(
      this.cartService.cartItems$.subscribe(items => {
        this.cartItems = items;
        this.recalculate();
      })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  removeItem(id: number): void {
    this.cartService.removeItem(id);
  }

  updateQuantity(item: CartItem, quantity: number): void {
    this.cartService.updateItemQuantity(item.id, quantity);
  }

  private recalculate(): void {
    this.subtotal = this.cartItems.reduce((sum, item) => sum + (item.discountPrice || item.price) * item.quantity, 0);
    this.tax = Math.round(this.subtotal * 0.05);
    // set tax=0 
    this.tax=0;
    this.total = this.subtotal + this.tax;
  }
}
