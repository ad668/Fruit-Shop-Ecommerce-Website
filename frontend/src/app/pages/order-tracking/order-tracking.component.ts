import { Component } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-order-tracking',
  templateUrl: './order-tracking.component.html',
  styleUrls: ['./order-tracking.component.css']
})
export class OrderTrackingComponent {
  orderId: number | null = null;
  trackingData: any = null;
  message = '';

  constructor(private apiService: ApiService) {}

  trackOrder(): void {
    if (!this.orderId || this.orderId <= 0) {
      this.message = 'Please enter a valid order ID.';
      this.trackingData = null;
      return;
    }

    this.apiService.trackOrder(this.orderId).subscribe({
      next: response => {
        if (response?.success) {
          this.trackingData = response.data;
          this.message = response.message;
        } else {
          this.message = response?.message || 'Unable to retrieve tracking data.';
          this.trackingData = null;
        }
      },
      error: err => {
        this.message = err?.error?.message || 'Unable to retrieve tracking data.';
        this.trackingData = null;
      }
    });
  }
}
