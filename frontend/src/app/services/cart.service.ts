import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Fruit } from '../models/fruit';

export interface CartItem {
  id: number;
  name: string;
  price: number;
  discountPrice: number;
  quantity: number;
  imageUrl: string;
  imageData?: string;
  priceUnit: string;
  stock: number;
}

@Injectable({ providedIn: 'root' })
export class CartService {
  private cartItemsSubject = new BehaviorSubject<CartItem[]>([]);
  public cartItems$ = this.cartItemsSubject.asObservable();

  private get currentItems(): CartItem[] {
    return this.cartItemsSubject.value;
  }

  addItem(fruit: Fruit, quantity = 1): 'added' | 'outOfStock' | 'limitReached' {
    if (fruit.stock <= 0) {
      return 'outOfStock';
    }

    const existing = this.currentItems.find(item => item.id === fruit.id);
    const maxQuantity = fruit.stock;

    if (existing) {
      if (existing.quantity >= maxQuantity) {
        return 'limitReached';
      }

      const updatedQuantity = Math.min(existing.quantity + quantity, maxQuantity);
      this.updateItemQuantity(fruit.id, updatedQuantity);
      return 'added';
    }

    const item: CartItem = {
      id: fruit.id,
      name: fruit.name,
      price: fruit.price,
      discountPrice: fruit.discountPrice,
      quantity: Math.min(quantity, maxQuantity),
      imageUrl: fruit.imageUrl,
      imageData: fruit.imageData,
      priceUnit: fruit.priceUnit,
      stock: fruit.stock
    };

    this.cartItemsSubject.next([...this.currentItems, item]);
    return 'added';
  }

  getItemQuantity(id: number): number {
    const item = this.currentItems.find(cartItem => cartItem.id === id);
    return item?.quantity ?? 0;
  }

  removeItem(id: number): void {
    this.cartItemsSubject.next(this.currentItems.filter(item => item.id !== id));
  }

  updateItemQuantity(id: number, quantity: number): void {
    const items = this.currentItems.map(item => {
      if (item.id !== id) {
        return item;
      }

      const clampedQuantity = Math.max(0.5, Math.min(quantity, item.stock > 0 ? item.stock : quantity));
      return { ...item, quantity: clampedQuantity };
    });

    this.cartItemsSubject.next(items);
  }

  clearCart(): void {
    this.cartItemsSubject.next([]);
  }

  get subtotal(): number {
    return this.currentItems.reduce((sum, item) => sum + (item.discountPrice || item.price) * item.quantity, 0);
  }

  get tax(): number {
    // return Math.round(this.subtotal * 0.05);
    return 0;
  }

  get total(): number {
    return this.subtotal + this.tax;
  }
}
