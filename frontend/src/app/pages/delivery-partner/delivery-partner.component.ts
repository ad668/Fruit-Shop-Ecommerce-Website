import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';

interface DeliveryOrder {
  id: number;
  userId: number;
  customerName: string;
  customerEmail: string;
  shippingAddress: string;
  shippingMethod: string;
  paymentMethod: string;
  trackingNumber: string;
  trackingStatus: string;
  status: string;
  subtotal: number;
  tax: number;
  shippingCharge: number;
  total: number;
  createdAt: string;
  items: OrderItem[];
}

interface OrderItem {
  fruitId: number;
  quantity: number;
  unitPrice: number;
  fruitName: string;
}

@Component({
  selector: 'app-delivery-partner',
  templateUrl: './delivery-partner.component.html',
  styleUrls: ['./delivery-partner.component.css']
})
export class DeliveryPartnerComponent implements OnInit {
  orders: DeliveryOrder[] = [];
  selectedOrder: DeliveryOrder | null = null;
  otpInput = '';
  loading = false;
  message = '';
  messageType: 'success' | 'error' = 'success';

  constructor(
    private api: ApiService,
    public auth: AuthService
  ) {}

  ngOnInit(): void {
    this.loadDeliveryOrders();
  }

  loadDeliveryOrders(): void {
    this.loading = true;
    this.api.getDeliveryOrders().subscribe(
      (response: any) => {
        this.orders = response.data || [];
        this.loading = false;
      },
      (error) => {
        this.loading = false;
        this.messageType = 'error';
        this.message = 'Failed to load delivery orders';
        setTimeout(() => {
          this.message = '';
        }, 2200);
      }
    );
  }

  selectOrder(order: DeliveryOrder): void {
    this.selectedOrder = order;
    this.otpInput = '';
    this.message = '';
  }

  deselectOrder(): void {
    this.selectedOrder = null;
    this.otpInput = '';
    this.message = '';
  }

  confirmDelivery(): void {
    if (!this.selectedOrder) {
      return;
    }

    if (!this.otpInput || this.otpInput.length !== 4) {
      this.messageType = 'error';
      this.message = 'Please enter a valid 4-digit OTP';
      return;
    }

    this.loading = true;
    this.api.verifyDeliveryOTP(this.selectedOrder.id, this.otpInput).subscribe(
      (response: any) => {
        this.loading = false;
        this.messageType = 'success';
        this.message = response.message || 'Order delivered successfully!';
        setTimeout(() => {
          this.loadDeliveryOrders();
          this.deselectOrder();
          this.message = '';
        }, 2200);
      },
      (error: any) => {
        this.loading = false;
        this.messageType = 'error';
        this.message = error.error?.message || 'Failed to verify OTP';
        setTimeout(() => {
          this.message = '';
        }, 2200);
      }
    );
  }

  getOrderItemsTotal(order: DeliveryOrder): number {
    return order.items.reduce((total, item) => total + (item.quantity * item.unitPrice), 0);
  }
}
