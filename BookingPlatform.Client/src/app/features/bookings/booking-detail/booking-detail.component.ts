import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BookingService } from '../../../core/services/booking.service';
import { ReviewService } from '../../../core/services/review.service';
import { AuthService } from '../../../core/services/auth.service';
import { BookingDetail } from '../../../core/models/models';

@Component({ standalone: false,
  template: `
    <div class="page-md" *ngIf="booking; else loadTpl">
      <div class="flex-between mb-16">
        <h1>Booking details</h1>
        <a routerLink="/my-bookings" class="btn btn-secondary btn-sm">← My bookings</a>
      </div>

      <div class="alert alert-error" *ngIf="error">{{error}}</div>
      <div class="alert alert-success" *ngIf="actionDone">{{actionDone}}</div>

      <div class="card" style="margin-bottom:16px">
        <div class="flex-between">
          <h2 style="margin:0">{{booking.propertyName}}</h2>
          <span class="badge" [ngClass]="statusClass(booking.bookingStatus)">{{booking.bookingStatus}}</span>
        </div>
        <p class="text-muted">📍 {{booking.propertyCity}}, {{booking.propertyCountry}}</p>
      </div>

      <div class="card" style="margin-bottom:16px">
        <div class="section-title">Booking info</div>
        <div class="detail-row"><span class="detail-label">Guest</span><span class="detail-val">{{booking.guestName}}</span></div>
        <div class="detail-row"><span class="detail-label">Check-in</span><span class="detail-val">{{booking.startDate | date:'mediumDate'}}</span></div>
        <div class="detail-row"><span class="detail-label">Check-out</span><span class="detail-val">{{booking.endDate | date:'mediumDate'}}</span></div>
        <div class="detail-row"><span class="detail-label">Guests</span><span class="detail-val">{{booking.guestCount}}</span></div>
      </div>

      <div class="card" style="margin-bottom:16px">
        <div class="section-title">Price breakdown</div>
        <div class="detail-row"><span class="detail-label">Price per night</span><span class="detail-val">€{{booking.pricePerNight}}</span></div>
        <div class="detail-row"><span class="detail-label">Period subtotal</span><span class="detail-val">€{{booking.priceForPeriod}}</span></div>
        <div class="detail-row" *ngIf="booking.cleaningFee"><span class="detail-label">Cleaning fee</span><span class="detail-val">€{{booking.cleaningFee}}</span></div>
        <div class="detail-row" *ngIf="booking.amenitiesUpCharge"><span class="detail-label">Amenities</span><span class="detail-val">€{{booking.amenitiesUpCharge}}</span></div>
        <div class="detail-row"><span class="detail-label detail-total">Total</span><span class="detail-val detail-total text-accent">€{{booking.totalPrice}}</span></div>
      </div>

      <div class="card">
        <div class="section-title">Timeline</div>
        <div class="detail-row" *ngIf="booking.createdOnUtc"><span class="detail-label">Created</span><span class="text-muted">{{booking.createdOnUtc | date:'medium'}}</span></div>
        <div class="detail-row" *ngIf="booking.confirmedOnUtc"><span class="detail-label">Confirmed</span><span style="color:var(--success)">{{booking.confirmedOnUtc | date:'medium'}}</span></div>
        <div class="detail-row" *ngIf="booking.completedOnUtc"><span class="detail-label">Completed</span><span style="color:var(--accent)">{{booking.completedOnUtc | date:'medium'}}</span></div>
        <div class="detail-row" *ngIf="booking.cancelledOnUtc"><span class="detail-label">Cancelled</span><span style="color:var(--danger)">{{booking.cancelledOnUtc | date:'medium'}}</span></div>
        <div class="detail-row" *ngIf="booking.rejectedOnUtc"><span class="detail-label">Rejected</span><span style="color:var(--danger)">{{booking.rejectedOnUtc | date:'medium'}}</span></div>
      </div>

      <div class="btn-group mt-24">
        <button class="btn btn-danger" *ngIf="booking.bookingStatus==='Created' && isGuest" (click)="cancel()">Cancel booking</button>
        <button class="btn btn-primary" *ngIf="booking.bookingStatus==='Created' && isOwner" (click)="verify()">Verify booking</button>
        <button class="btn btn-secondary" *ngIf="booking.bookingStatus==='Confirmed' && isOwner" (click)="complete()">Mark as completed</button>
        <ng-container *ngIf="booking.bookingStatus==='Completed' && isGuest">
          <a *ngIf="!booking.hasReview" [routerLink]="['/bookings', booking.id, 'review']" class="btn btn-primary">Leave a review</a>
          <span class="text-muted" *ngIf="booking.hasReview">✓ Reviewed</span>
        </ng-container>
        <a [routerLink]="['/properties', booking.propertyId]" class="btn btn-secondary">View property</a>
      </div>
    </div>
    <ng-template #loadTpl><div class="loading">Loading…</div></ng-template>
  `
})
export class BookingDetailComponent implements OnInit {
  booking: BookingDetail | null = null;
  error = ''; actionDone = '';
  constructor(
    private route: ActivatedRoute, private router: Router,
    private svc: BookingService, private reviewSvc: ReviewService,
    public auth: AuthService
  ) {}
  get isGuest() { return this.auth.isAuthenticated() && !this.auth.isOwner(); }
  get isOwner() { return this.auth.isOwner(); }
  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.svc.getById(id).subscribe({ next: b => this.booking = b, error: () => this.error = 'Not found' });
  }
  cancel() {
    if (!confirm('Cancel?') || !this.booking) return;
    this.svc.cancel(this.booking.id).subscribe({ next: () => { this.booking!.bookingStatus = 'Cancelled'; this.actionDone = 'Booking cancelled.'; }, error: e => this.error = e.error?.description || 'Failed' });
  }
  verify() {
    if (!this.booking) return;
    this.svc.verify(this.booking.id).subscribe({ next: () => { this.booking!.bookingStatus = 'Confirmed'; this.actionDone = 'Booking confirmed.'; }, error: e => this.error = e.error?.description || 'Failed' });
  }
  complete() {
    if (!this.booking) return;
    this.svc.complete(this.booking.id).subscribe({ next: () => { this.booking!.bookingStatus = 'Completed'; this.actionDone = 'Booking marked as completed.'; }, error: e => this.error = e.error?.description || 'Failed' });
  }
  statusClass(s: string): string { return `badge-${s.toLowerCase()}`; }
}

