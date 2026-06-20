import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { Fruit } from '../../models/fruit';

@Component({
  selector: 'app-admin-add-fruit',
  templateUrl: './admin-add-fruit.component.html',
  styleUrls: ['./admin-add-fruit.component.css']
})
export class AdminAddFruitComponent implements OnInit {
  fruit: Omit<Fruit, 'id'> = {
    name: '',
    description: '',
    category: 'Fruits',
    price: 0,
    discountPrice: 0,
    stock: 0,
    imageUrl: '',
    imageData: undefined,
    isFeatured: false,
    status: 'Available',
    priceUnit: 'KG'
  };

  selectedFile: File | null = null;
  imagePreview: string | ArrayBuffer | null = null;
  isEditMode = false;
  fruitId: number | null = null;
  title = 'Add New Fruit';
  submitLabel = 'Add Fruit';
  message = '';
  messageType: 'success' | 'error' | '' = '';

  selectedTab: 'fruits' | 'orders' = 'orders';
  orders: any[] = [];
  filteredOrders: any[] = [];
  searchTracking = '';
  adminMessage = '';
  adminMessageType: 'success' | 'error' | '' = '';

  constructor(
    public auth: AuthService,
    private api: ApiService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    if (!this.auth.isAdmin()) {
      this.router.navigate(['/']);
      return;
    }

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEditMode = true;
      this.fruitId = Number(idParam);
      this.title = 'Update Fruit';
      this.submitLabel = 'Save Changes';
      this.loadFruit(this.fruitId);
    }

    this.loadOrders();
  }

  private loadFruit(id: number): void {
    this.api.getFruit(id).subscribe({
      next: (response: any) => {
        const data: Fruit = response.data;
        if (data) {
          this.fruit = {
            name: data.name,
            description: data.description,
            category: data.category,
            price: data.price,
            discountPrice: data.discountPrice,
            stock: data.stock,
            imageUrl: data.imageUrl,
            imageData: data.imageData,
            isFeatured: data.isFeatured,
            status: data.status || 'Available',
            priceUnit: data.priceUnit || 'KG'
          };
          this.imagePreview = data.imageData || data.imageUrl;
        }
      },
      error: () => {
        this.messageType = 'error';
        this.message = 'Could not load fruit for editing.';
      }
    });
  }

  submit(): void {
    this.message = '';
    this.messageType = '';

    if (this.isEditMode && this.fruitId !== null) {
      this.api.updateFruit(this.fruitId, this.fruit).subscribe({
        next: () => {
          this.messageType = 'success';
          this.message = 'Fruit updated successfully.';
        },
        error: (error) => {
          this.messageType = 'error';
          this.message = error?.error?.message || 'Could not update fruit. Please check your input and try again.';
        }
      });
      return;
    }

    this.api.addFruit(this.fruit).subscribe({
      next: () => {
        this.messageType = 'success';
        this.message = 'Fruit added successfully.';
        this.fruit = {
          name: '',
          description: '',
          category: 'Fruits',
          price: 0,
          discountPrice: 0,
          stock: 0,
          imageUrl: '',
          imageData: undefined,
          isFeatured: false,
          status: 'Available',
          priceUnit: 'KG'
        };
        this.imagePreview = null;
        this.selectedFile = null;
      },
      error: (error) => {
        this.messageType = 'error';
        this.message = error?.error?.message || 'Could not add fruit. Please check your input and try again.';
      }
    });
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];

    if (file) {
      this.selectedFile = file;

      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result;
        if (typeof this.imagePreview === 'string') {
          this.fruit.imageData = this.imagePreview;
        }
      };
      reader.readAsDataURL(file);
    }
  }

  switchTab(tab: 'fruits' | 'orders'): void {
    this.selectedTab = tab;
    this.adminMessage = '';
    this.adminMessageType = '';
    if (tab === 'orders') {
      this.loadOrders();
    }
  }

  loadOrders(): void {
    this.api.getOrders().subscribe({
      next: (response: any) => {
        this.orders = response.data || [];
        this.filteredOrders = [...this.orders];
      },
      error: () => {
        this.adminMessageType = 'error';
        this.adminMessage = 'Unable to load orders right now. Please try again later.';
      }
    });
  }

  applySearch(): void {
    const searchValue = this.searchTracking.trim().toLowerCase();
    this.filteredOrders = this.orders.filter(order =>
      order.trackingNumber?.toLowerCase().includes(searchValue)
    );
  }

  confirmOrder(order: any): void {
    this.api.confirmOrder(order.id).subscribe({
      next: (response: any) => {
        this.adminMessageType = 'success';
        this.adminMessage = response.message || 'Order confirmed as shipped.';
        order.status = 'Shipped';
        order.trackingStatus = 'Shipped';
      },
      error: (error) => {
        this.adminMessageType = 'error';
        this.adminMessage = error?.error?.message || 'Could not confirm order. Please try again.';
      }
    });
  }

  formatDate(value: string): string {
    return new Date(value).toLocaleString();
  }
}
