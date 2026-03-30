import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { OwnerGuard } from './core/guards/owner.guard';

import { PropertyListComponent } from './features/properties/property-list/property-list.component';
import { PropertyDetailComponent } from './features/properties/property-detail/property-detail.component';
import { CreatePropertyComponent } from './features/properties/create-property/create-property.component';
import { EditPropertyComponent } from './features/properties/edit-property/edit-property.component';
import { MyPropertiesComponent } from './features/properties/my-properties/my-properties.component';
import { MyBookingsComponent } from './features/bookings/my-bookings/my-bookings.component';
import { BookingDetailComponent } from './features/bookings/booking-detail/booking-detail.component';
import { MakeReviewComponent } from './features/bookings/make-review/make-review.component';
import { ProfileComponent } from './features/profile/profile.component';
import { ChangePasswordComponent } from './features/profile/change-password/change-password.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { ForgotPasswordComponent } from './features/auth/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './features/auth/reset-password/reset-password.component';
import { BecomeOwnerComponent } from './features/owner/become-owner/become-owner.component';
import { OwnerProfileComponent } from './features/owner/owner-profile/owner-profile.component';

const routes: Routes = [
  { path: '', component: PropertyListComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'forgot-password', component: ForgotPasswordComponent },
  { path: 'reset-password', component: ResetPasswordComponent },
  // specific property routes must come before :id
  { path: 'properties/new', component: CreatePropertyComponent, canActivate: [OwnerGuard] },
  { path: 'properties/:id/edit', component: EditPropertyComponent, canActivate: [OwnerGuard] },
  { path: 'properties/:id', component: PropertyDetailComponent },
  { path: 'my-properties', component: MyPropertiesComponent, canActivate: [OwnerGuard] },
  { path: 'my-bookings', component: MyBookingsComponent, canActivate: [AuthGuard] },
  { path: 'bookings/:id/review', component: MakeReviewComponent, canActivate: [AuthGuard] },
  { path: 'bookings/:id', component: BookingDetailComponent, canActivate: [AuthGuard] },
  { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] },
  { path: 'change-password', component: ChangePasswordComponent, canActivate: [AuthGuard] },
  { path: 'become-owner', component: BecomeOwnerComponent, canActivate: [AuthGuard] },
  { path: 'owner-profile', component: OwnerProfileComponent, canActivate: [OwnerGuard] },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
