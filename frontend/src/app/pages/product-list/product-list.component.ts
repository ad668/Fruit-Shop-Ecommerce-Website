import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { CartService, CartItem } from '../../services/cart.service';
import { Fruit } from '../../models/fruit';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  fruits: Fruit[] = [];
  allFruits: Fruit[] = [];
  categories: string[] = [];
  selectedCategory = 'All';
  searchQuery = '';
  cartItems: CartItem[] = [];
  message = '';
  messageType: 'success' | 'error' = 'success';

  constructor(
    private api: ApiService,
    public auth: AuthService,
    private cartService: CartService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.api.getFruits().subscribe((response: any) => {
      this.allFruits = response.data || [];
      const uniqueCategories = Array.from(new Set(this.allFruits.map(f => f.category || 'Others'))).sort((a, b) => a.localeCompare(b));
      this.categories = ['All', ...uniqueCategories];
      this.applyCategoryFilter('All');
    });

    this.cartService.cartItems$.subscribe(items => {
      this.cartItems = items;
    });
  }

  applyCategoryFilter(category: string): void {
    this.selectedCategory = category;
    this.applyFilters();
  }

  applyFilters(): void {
    let filtered = this.allFruits;

    // Apply category filter
    if (this.selectedCategory !== 'All') {
      filtered = filtered.filter(fruit => fruit.category === this.selectedCategory);
    }

    // Apply search filter
    if (this.searchQuery.trim()) {
      const query = this.searchQuery.toLowerCase();
      filtered = filtered.filter(fruit =>
        fruit.name.toLowerCase().includes(query) ||
        fruit.description.toLowerCase().includes(query)
      );
    }

    this.fruits = filtered;
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  addToCart(fruit: Fruit): void {
    if (!this.auth.isLoggedIn()) {
      this.messageType = 'error';
      this.message = 'Please login to add items to the cart.';
      setTimeout(() => {
        this.message = '';
      }, 2200);
      this.router.navigate(['/auth']);
      return;
    }

    const result = this.cartService.addItem(fruit);
    if (result === 'outOfStock') {
      this.messageType = 'error';
      this.message = 'This item is currently out of stock. Please choose another product.';
    } else if (result === 'limitReached') {
      this.messageType = 'error';
      this.message = 'You already have the maximum available quantity of this item in your cart.';
    } else {
      this.messageType = 'success';
      this.message = `${fruit.name} added to cart.`;
    }

    setTimeout(() => {
      this.message = '';
    }, 2200);
  }

  getCartQuantity(fruitId: number): number {
    return this.cartService.getItemQuantity(fruitId);
  }
}
