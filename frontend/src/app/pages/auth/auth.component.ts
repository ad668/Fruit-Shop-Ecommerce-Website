import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css']
})
export class AuthComponent {
  mode: 'login' | 'register' = 'login';
  credentials = { name: '', email: '', password: '' };
  message = '';
  messageType: 'success' | 'error' | '' = '';

  constructor(private api: ApiService, private auth: AuthService, private router: Router) {}

  toggleMode(): void {
    this.mode = this.mode === 'login' ? 'register' : 'login';
    this.message = '';
    this.messageType = '';
  }

  submit(): void {
    const payload = this.mode === 'register'
      ? { name: this.credentials.name, email: this.credentials.email, password: this.credentials.password }
      : { email: this.credentials.email, password: this.credentials.password };

    const request$ = this.mode === 'register'
      ? this.api.register(payload)
      : this.api.login(payload);

    request$.subscribe({
      next: (result: any) => {
        this.messageType = 'success';
        this.message = result?.message || (this.mode === 'register' ? 'User created successfully.' : 'Login successful.');

        if (this.mode === 'register') {
          this.mode = 'login';
          this.credentials.password = '';
          return;
        }

        if (result?.data?.token) {
          this.auth.setToken(result.data.token);
        }

        this.router.navigate(['/']);
      },
      error: (error) => {
        this.messageType = 'error';
        this.message = error?.error?.message || 'Unable to complete request. Please try again.';
      }
    });
  }
}
