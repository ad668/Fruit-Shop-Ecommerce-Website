import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { ProductListComponent } from './pages/product-list/product-list.component';
import { ProductDetailComponent } from './pages/product-detail/product-detail.component';
import { CartComponent } from './pages/cart/cart.component';
import { CheckoutComponent } from './pages/checkout/checkout.component';
import { OrderTrackingComponent } from './pages/order-tracking/order-tracking.component';
import { AuthComponent } from './pages/auth/auth.component';
import { AdminAddFruitComponent } from './pages/admin-add-fruit/admin-add-fruit.component';
import { AdminManageUsersComponent } from './pages/admin-manage-users/admin-manage-users.component';
import { DeliveryPartnerComponent } from './pages/delivery-partner/delivery-partner.component';
import { ContactComponent } from './pages/contact/contact.component';
import { AuthGuard } from './guards/auth.guard';
import { DeliveryPartnerGuard } from './guards/delivery-partner.guard';
import { AdminGuard } from './guards/admin.guard';

const routes: Routes = [
  { path: 'admin/add-fruit', component: AdminAddFruitComponent, canActivate: [AdminGuard] },
  { path: 'admin/edit-fruit/:id', component: AdminAddFruitComponent, canActivate: [AdminGuard] },
  { path: 'admin/manage-users', component: AdminManageUsersComponent, canActivate: [AdminGuard] },
  { path: 'delivery', component: DeliveryPartnerComponent, canActivate: [DeliveryPartnerGuard] },
  { path: '', component: HomeComponent },
  { path: 'products', component: ProductListComponent },
  { path: 'products/:id', component: ProductDetailComponent },
  { path: 'cart', component: CartComponent, canActivate: [AuthGuard] },
  { path: 'checkout', component: CheckoutComponent },
  { path: 'tracking', component: OrderTrackingComponent, canActivate: [AuthGuard] },
  { path: 'contact', component: ContactComponent, canActivate: [AuthGuard] },
  { path: 'auth', component: AuthComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' })],
  exports: [RouterModule]
})
export class AppRoutingModule {}
