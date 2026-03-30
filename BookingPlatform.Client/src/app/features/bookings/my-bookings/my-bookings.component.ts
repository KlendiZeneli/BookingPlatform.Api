import { Component, OnInit } from '@angular/core';
import { BookingService } from '../../../core/services/booking.service';
import { BookingInfo } from '../../../core/models/models';

@Component({ standalone: false,
  template: `
    <div class="page">
      <h1>My bookings</h1>
      <p class="subtitle">Your booking history</p>

      <div class="tabs">
        <span class="tab" *ngFor="let t of tabs" [class.active]="tab===t" (click)="tab=t">{{t}}</span>
      </div>

      <div class="loading" *ngIf="loading">Loading…</div>

      <div class="empty" *ngIf="!loading && !filtered().length">
        <div class="empty-icon">📋</div>
        <p>No {{tab.toLowerCase()}} bookings.</p>
        <a routerLink="/" class="btn btn-primary mt-16">Browse properties</a>
      </div>

      <div *ngFor="let b of filtered()" class="card" style="margin-bottom:12px">
        <div class="flex-between">
          <div>
            <div style="font-size:16px;font-weight:600">{{b.propertyName}}</div>
            <div class="text-muted">📍 {{b.propertyCity}}, {{b.propertyCountry}}</div>
            <div class="text-muted" style="margin-top:4px">
              {{b.startDate | date}} – {{b.endDate | date}} &nbsp;·&nbsp; {{b.guestCount}} guests
            </div>
          </div>
          <div style="text-align:right">
            <span class="badge" [ngClass]="statusClass(b.bookingStatus)">{{b.bookingStatus}}</span>
            <div style="font-size:16px;font-weight:700;margin-top:6px">€{{b.totalPrice}}</div>
          </div>
        </div>
        <hr class="divider" style="margin:12px 0">
        <div class="btn-group">
          <a [routerLink]="['/bookings', b.id]" class="btn btn-secondary btn-sm">View details</a>
          <button class="btn btn-danger btn-sm" *ngIf="b.bookingStatus==='Created'" (click)="cancel(b)">Cancel</button>
          <a [routerLink]="['/bookings', b.id, 'review']" class="btn btn-primary btn-sm"
            *ngIf="b.bookingStatus==='Completed' && !b.hasReview">Leave a review</a>
          <span class="text-muted" style="font-size:13px" *ngIf="b.bookingStatus==='Completed' && b.hasReview">✓ Reviewed</span>
        </div>
      </div>
    </div>
  `
})
export class MyBookingsComponent implements OnInit {
  bookings: BookingInfo[] = [];
  loading = false; tab = 'All';
  tabs = ['All', 'Created', 'Confirmed', 'Completed', 'Cancelled'];
  constructor(private svc: BookingService) {}
  ngOnInit() {
    this.loading = true;
    this.svc.getMyBookings().subscribe({ next: r => { this.bookings = r.bookings; this.loading = false; }, error: () => this.loading = false });
  }
  filtered(): BookingInfo[] { return this.tab === 'All' ? this.bookings : this.bookings.filter(b => b.bookingStatus === this.tab); }
  statusClass(s: string): string { return `badge-${s.toLowerCase()}`; }
  cancel(b: BookingInfo) {
    if (!confirm('Cancel this booking?')) return;
    this.svc.cancel(b.id).subscribe({ next: () => b.bookingStatus = 'Cancelled', error: () => alert('Could not cancel') });
  }
}

