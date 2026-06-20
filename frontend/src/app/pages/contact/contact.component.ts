import { Component } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent {
  contactForm = {
    name: '',
    email: '',
    phone: '',
    subject: '',
    message: ''
  };

  isSubmitting = false;
  message = '';
  messageType: 'success' | 'error' | '' = '';

  constructor(
    private apiService: ApiService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    // Pre-fill email if user is logged in
    if (this.authService.isLoggedIn()) {
      this.contactForm.email = this.authService.getUserEmail() || '';
    }
  }

  submitContact(): void {
    this.message = '';
    this.messageType = '';

    if (!this.contactForm.name || !this.contactForm.email || !this.contactForm.message) {
      this.messageType = 'error';
      this.message = 'Please fill in all required fields.';
      return;
    }

    if (!this.validateEmail(this.contactForm.email)) {
      this.messageType = 'error';
      this.message = 'Please enter a valid email address.';
      return;
    }

    this.isSubmitting = true;

    this.apiService.sendContactMessage(this.contactForm).subscribe({
      next: (response: any) => {
        this.messageType = 'success';
        this.message = response?.message || 'Thank you for contacting us! We will get back to you soon.';
        this.resetForm();
        this.isSubmitting = false;
      },
      error: (error) => {
        this.messageType = 'error';
        this.message = error?.error?.message || 'Failed to send message. Please try again later.';
        this.isSubmitting = false;
      }
    });
  }

  private resetForm(): void {
    this.contactForm = {
      name: '',
      email: this.authService.isLoggedIn() ? this.authService.getUserEmail() || '' : '',
      phone: '',
      subject: '',
      message: ''
    };
  }

  private validateEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }
}
