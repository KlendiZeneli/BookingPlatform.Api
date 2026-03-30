import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';

import { AppComponent } from './app.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { ForgotPasswordComponent } from './features/auth/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './features/auth/reset-password/reset-password.component';
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
import { BecomeOwnerComponent } from './features/owner/become-owner/become-owner.component';
import { OwnerProfileComponent } from './features/owner/owner-profile/owner-profile.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent, RegisterComponent, ForgotPasswordComponent, ResetPasswordComponent,
    PropertyListComponent, PropertyDetailComponent, CreatePropertyComponent, EditPropertyComponent, MyPropertiesComponent,
    MyBookingsComponent, BookingDetailComponent, MakeReviewComponent,
    ProfileComponent, ChangePasswordComponent,
    BecomeOwnerComponent, OwnerProfileComponent,
  ],
  imports: [BrowserModule, FormsModule, HttpClientModule, AppRoutingModule],
  providers: [{ provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }],
  bootstrap: [AppComponent]
})
export class AppModule {}
