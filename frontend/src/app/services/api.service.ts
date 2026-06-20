import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Fruit } from '../models/fruit';

@Injectable({ providedIn: 'root' })
export class ApiService {
  // Use HTTPS dev API endpoint
  private apiUrl = 'http://localhost:5000/api';
  //private apiUrl=' https://effort-squeak-praying.ngrok-free.dev/api';

  constructor(private http: HttpClient) {}

  getFruits(): Observable<any> {
    return this.http.get(`${this.apiUrl}/fruits`);
  }

  getFruit(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/fruits/${id}`);
  }

  addFruit(data: Omit<Fruit, 'id'>): Observable<any> {
    return this.http.post(`${this.apiUrl}/fruits`, data);
  }

  updateFruit(id: number, data: Omit<Fruit, 'id'>): Observable<any> {
    return this.http.put(`${this.apiUrl}/fruits/${id}`, data);
  }

  register(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/register`, data);
  }

  login(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/login`, data);
  }

  checkout(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/orders/checkout`, data);
  }

  trackOrder(orderId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/orders/track/${orderId}`);
  }

  getOrders(): Observable<any> {
    return this.http.get(`${this.apiUrl}/orders`);
  }

  confirmOrder(orderId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/orders/confirm/${orderId}`, {});
  }

  getDeliveryOrders(): Observable<any> {
    return this.http.get(`${this.apiUrl}/orders/delivery-orders`);
  }

  verifyDeliveryOTP(orderId: number, otp: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/orders/verify-delivery-otp/${orderId}`, { Otp: otp });
  }

  registerDeliveryPartner(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/register-delivery-partner`, data);
  }

  registerAdmin(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/register-admin`, data);
  }

  sendContactMessage(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/contact`, data);
  }
}
