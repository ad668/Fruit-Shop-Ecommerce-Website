import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { Fruit } from '../../models/fruit';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.css']
})
export class ProductDetailComponent implements OnInit {
  fruit: Fruit | null = null;
  quantity = 1;
  message = '';
  messageType: 'success' | 'error' = 'success';
  private readonly quantityStep = 0.5;

  constructor(
    private route: ActivatedRoute,
    private api: ApiService,
    private cartService: CartService,
    private auth: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.api.getFruit(id).subscribe((response: any) => {
      this.fruit = response.data || null;
    });
  }

  increase(): void {
    if (this.fruit) {
      this.quantity = Math.min(this.quantity + this.quantityStep, this.fruit.stock);
    }
  }

  decrease(): void {
    this.quantity = Math.max(this.quantityStep, this.quantity - this.quantityStep);
  }

  addToCart(): void {
    if (!this.auth.isLoggedIn()) {
      this.messageType = 'error';
      this.message = 'Please login to add items to the cart.';
      setTimeout(() => {
        this.message = '';
      }, 2200);
      this.router.navigate(['/auth']);
      return;
    }

    if (this.fruit) {
      if (this.fruit.stock <= 0) {
        this.messageType = 'error';
        this.message = 'This item is currently out of stock. Please choose another product.';
      } else {
        const result = this.cartService.addItem(this.fruit, this.quantity);
        if (result === 'outOfStock') {
          this.messageType = 'error';
          this.message = 'This item is currently out of stock. Please choose another product.';
        } else if (result === 'limitReached') {
          this.messageType = 'error';
          this.message = 'You already have the maximum available quantity of this item in your cart.';
        } else {
          this.messageType = 'success';
          this.message = `${this.fruit.name} added to cart.`;
        }
      }

      setTimeout(() => {
        this.message = '';
      }, 2200);
    }
  }
}
