import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { HomeComponent } from './pages/home/home.component';
import { ProductListComponent } from './pages/product-list/product-list.component';
import { ProductDetailComponent } from './pages/product-detail/product-detail.component';
import { CartComponent } from './pages/cart/cart.component';
import { CheckoutComponent } from './pages/checkout/checkout.component';
import { OrderTrackingComponent } from './pages/order-tracking/order-tracking.component';
import { AuthComponent } from './pages/auth/auth.component';
import { AdminAddFruitComponent } from './pages/admin-add-fruit/admin-add-fruit.component';
import { DeliveryPartnerComponent } from './pages/delivery-partner/delivery-partner.component';
import { AdminManageUsersComponent } from './pages/admin-manage-users/admin-manage-users.component';
import { ContactComponent } from './pages/contact/contact.component';
import { AuthInterceptor } from './interceptors/auth.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ProductListComponent,
    ProductDetailComponent,
    CartComponent,
    CheckoutComponent,
    OrderTrackingComponent,
    AuthComponent,
    AdminAddFruitComponent,
    DeliveryPartnerComponent,
    AdminManageUsersComponent,
    ContactComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    RouterModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
