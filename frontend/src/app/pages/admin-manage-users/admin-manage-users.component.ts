import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';

interface User {
  name: string;
  email: string;
  password: string;
  role: 'Admin' | 'DeliveryPartner';
}

@Component({
  selector: 'app-admin-manage-users',
  templateUrl: './admin-manage-users.component.html',
  styleUrls: ['./admin-manage-users.component.css']
})
export class AdminManageUsersComponent implements OnInit {
  formData: User = {
    name: '',
    email: '',
    password: '',
    role: 'DeliveryPartner'
  };

  loading = false;
  message = '';
  messageType: 'success' | 'error' = 'success';
  formSubmitted = false;
  selectedRole: 'DeliveryPartner' | 'Admin' = 'DeliveryPartner';

  constructor(
    private api: ApiService,
    public auth: AuthService
  ) {}

  ngOnInit(): void {
    // Component initialization if needed
  }

  onRoleChange(role: 'DeliveryPartner' | 'Admin'): void {
    this.selectedRole = role;
    this.formData.role = role;
  }

  submitForm(): void {
    this.formSubmitted = true;

    // Validation
    if (!this.formData.name.trim()) {
      this.messageType = 'error';
      this.message = 'Please enter a name';
      return;
    }

    if (!this.formData.email.trim() || !this.isValidEmail(this.formData.email)) {
      this.messageType = 'error';
      this.message = 'Please enter a valid email';
      return;
    }

    if (!this.formData.password || this.formData.password.length < 6) {
      this.messageType = 'error';
      this.message = 'Password must be at least 6 characters';
      return;
    }

    this.loading = true;

    if (this.selectedRole === 'DeliveryPartner') {
      this.api.registerDeliveryPartner(this.formData).subscribe(
        (response: any) => {
          this.loading = false;
          this.messageType = 'success';
          this.message = response.message || 'Delivery Partner registered successfully!';
          this.resetForm();
          setTimeout(() => {
            this.message = '';
          }, 2200);
        },
        (error: any) => {
          this.loading = false;
          this.messageType = 'error';
          this.message = error.error?.message || 'Failed to register Delivery Partner';
          setTimeout(() => {
            this.message = '';
          }, 2200);
        }
      );
    } else {
      this.api.registerAdmin(this.formData).subscribe(
        (response: any) => {
          this.loading = false;
          this.messageType = 'success';
          this.message = response.message || 'Admin user registered successfully!';
          this.resetForm();
          setTimeout(() => {
            this.message = '';
          }, 2200);
        },
        (error: any) => {
          this.loading = false;
          this.messageType = 'error';
          this.message = error.error?.message || 'Failed to register Admin user';
          setTimeout(() => {
            this.message = '';
          }, 2200);
        }
      );
    }
  }

  resetForm(): void {
    this.formData = {
      name: '',
      email: '',
      password: '',
      role: 'DeliveryPartner'
    };
    this.formSubmitted = false;
    this.selectedRole = 'DeliveryPartner';
  }

  isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }
}
