import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private tokenKey = 'authToken';
  private roleKey = 'userRole';

  setToken(token: string | null): void {
    if (token && !this.isTokenExpired(token)) {
      localStorage.setItem(this.tokenKey, token);
      const role = this.decodeRole(token);
      if (role) {
        localStorage.setItem(this.roleKey, role);
      }
    } else {
      localStorage.removeItem(this.tokenKey);
      localStorage.removeItem(this.roleKey);
    }
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getRole(): string | null {
    return localStorage.getItem(this.roleKey);
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) {
      return false;
    }

    if (this.isTokenExpired(token)) {
      this.logout();
      return false;
    }

    return true;
  }

  getUserId(): number | null {
    const payload = this.decodePayload(this.getToken() ?? '');
    if (!payload?.sub) {
      return null;
    }

    return Number(payload.sub) || null;
  }

  getUserEmail(): string | null {
    const payload = this.decodePayload(this.getToken() ?? '');
    return payload?.email || null;
  }

  isAdmin(): boolean {
    return this.getRole() === 'Admin';
  }

  isDeliveryPartner(): boolean {
    return this.getRole() === 'DeliveryPartner';
  }

  isAdminOrDeliveryPartner(): boolean {
    const role = this.getRole();
    return role === 'Admin' || role === 'DeliveryPartner';
  }

  logout(): void {
    this.setToken(null);
  }

  private decodeRole(token: string): string | null {
    const payload = this.decodePayload(token);
    return payload?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || payload?.role || null;
  }

  private decodePayload(token: string): any | null {
    try {
      const payload = token.split('.')[1];
      const normalized = payload.replace(/-/g, '+').replace(/_/g, '/');
      const padded = normalized.padEnd(Math.ceil(normalized.length / 4) * 4, '=');
      return JSON.parse(atob(padded));
    } catch {
      return null;
    }
  }

  private isTokenExpired(token: string): boolean {
    const payload = this.decodePayload(token);
    if (!payload?.exp) {
      return true;
    }

    const expiration = payload.exp * 1000;
    return Date.now() >= expiration;
  }
}
