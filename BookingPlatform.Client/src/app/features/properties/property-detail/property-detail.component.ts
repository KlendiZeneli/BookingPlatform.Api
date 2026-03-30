import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PropertyService } from '../../../core/services/property.service';
import { BookingService } from '../../../core/services/booking.service';
import { ReviewService } from '../../../core/services/review.service';
import { AuthService } from '../../../core/services/auth.service';
import { PropertyDetail, Review, PROPERTY_TYPES } from '../../../core/models/models';

@Component({ standalone: false,
  template: `
    <div class="page" *ngIf="property; else loadingTpl">
      <!-- Images carousel -->
      <div class="carousel" style="margin-bottom:24px">
        <ng-container *ngIf="property.images.length; else noImg">
          <img [src]="currentImgSrc()" [alt]="property.name">
          <button class="carousel-btn carousel-btn-prev" *ngIf="property.images.length>1" (click)="prevImg()">‹</button>
          <button class="carousel-btn carousel-btn-next" *ngIf="property.images.length>1" (click)="nextImg()">›</button>
          <div class="carousel-dots" *ngIf="property.images.length>1">
            <span class="carousel-dot" *ngFor="let img of property.images; let i=index"
              [class.active]="i===imgIdx" (click)="imgIdx=i"></span>
          </div>
        </ng-container>
        <ng-template #noImg><div class="carousel-no-img">🏠</div></ng-template>
      </div>

      <div class="two-col">
        <!-- Left: details -->
        <div>
          <div class="flex-between mb-16">
            <div>
              <h1>{{property.name}}</h1>
              <p class="text-muted">📍 {{property.address?.city}}, {{property.address?.country}} &nbsp;·&nbsp;
                {{propertyTypeLabel(property.propertyType)}}</p>
            </div>
            <span class="stars" style="font-size:16px">★ {{property.averageRating | number:'1.1-1'}}<span class="text-muted" style="font-size:13px"> ({{property.reviewCount}} reviews)</span></span>
          </div>

          <p style="color:var(--muted);font-size:15px;line-height:1.7;margin-bottom:20px">{{property.description}}</p>

          <div class="card" style="margin-bottom:16px">
            <div class="section-title">Property details</div>
            <div style="display:grid;grid-template-columns:repeat(auto-fill,minmax(140px,1fr));gap:12px">
              <div class="detail-stat">👥 Up to {{property.maxGuests}} guests</div>
              <div class="detail-stat">🛏 {{property.bedrooms}} bedrooms</div>
              <div class="detail-stat">🛏 {{property.beds}} beds</div>
              <div class="detail-stat">🚿 {{property.bathrooms}} bathrooms</div>
              <div class="detail-stat">🕐 Check-in: {{property.checkInTime}}</div>
              <div class="detail-stat">🕐 Check-out: {{property.checkOutTime}}</div>
            </div>
          </div>

          <!-- Availability checker -->
          <div class="card" style="margin-bottom:16px">
            <div class="section-title">Check availability</div>
            <div class="form-row">
              <div class="form-group" style="margin:0">
                <label>Check-in</label>
                <input type="date" [(ngModel)]="avCheckIn">
              </div>
              <div class="form-group" style="margin:0">
                <label>Check-out</label>
                <input type="date" [(ngModel)]="avCheckOut">
              </div>
            </div>
            <button class="btn btn-secondary btn-sm mt-8" (click)="checkAvailability()">Check</button>
            <div class="alert alert-success mt-8" *ngIf="avResult===true">✓ Available for those dates</div>
            <div class="alert alert-error mt-8" *ngIf="avResult===false">✗ Not available — {{blockedMsg}}</div>
          </div>

          <!-- Reviews -->
          <div class="card">
            <div class="section-title">Reviews ({{reviews.length}})</div>
            <div class="empty" *ngIf="!reviews.length"><p>No reviews yet.</p></div>
            <div class="review-item" *ngFor="let r of reviews">
              <div class="review-header">
                <span class="review-author">{{r.guestName}}</span>
                <span class="stars">{{starStr(r.rating)}}</span>
              </div>
              <p class="review-comment" *ngIf="r.comment">{{r.comment}}</p>
            </div>
          </div>

          <!-- Owner: booking management -->
          <div class="card mt-16" *ngIf="isOwner && property.bookings.length">
            <div class="section-title">Bookings for this property</div>
            <div class="card" *ngFor="let b of property.bookings" style="margin-bottom:8px;padding:12px">
              <div class="flex-between">
                <div>
                  <span class="badge" [ngClass]="statusClass(b.bookingStatus)">{{b.bookingStatus}}</span>
                  <span class="text-muted" style="font-size:13px;margin-left:8px">{{b.startDate | date}} – {{b.endDate | date}}</span>
                </div>
                <div class="btn-group">
                  <button class="btn btn-primary btn-sm" *ngIf="b.bookingStatus==='Created'" (click)="verifyBooking(b.id)">Verify</button>
                  <button class="btn btn-secondary btn-sm" *ngIf="b.bookingStatus==='Confirmed'" (click)="completeBooking(b.id)">Complete</button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Right: booking widget -->
        <div class="sticky-side">
          <div class="card" *ngIf="!isOwner">
            <div style="font-size:22px;font-weight:700;margin-bottom:16px">€{{property.pricePerNight}}<span class="text-muted" style="font-size:14px;font-weight:400"> /night</span></div>
            <ng-container *ngIf="isGuest; else loginPrompt">
              <div class="alert alert-error" *ngIf="bookError">{{bookError}}</div>
              <div class="alert alert-success" *ngIf="bookSuccess">Booking placed! <a [routerLink]="['/bookings', lastBookingId]" class="text-accent">View booking</a></div>
              <div class="form-group">
                <label>Check-in</label>
                <input type="date" [(ngModel)]="bookIn">
              </div>
              <div class="form-group">
                <label>Check-out</label>
                <input type="date" [(ngModel)]="bookOut">
              </div>
              <div class="form-group">
                <label>Guests</label>
                <input type="number" min="1" [max]="property.maxGuests" [(ngModel)]="bookGuests">
              </div>
              <button class="btn btn-primary btn-full" (click)="book()" [disabled]="booking">
                {{booking ? 'Booking…' : 'Book now'}}
              </button>
            </ng-container>
            <ng-template #loginPrompt>
              <p class="text-muted" style="text-align:center;margin-bottom:12px">Sign in to book this property</p>
              <a routerLink="/login" class="btn btn-primary btn-full">Sign in to book</a>
            </ng-template>
          </div>
        </div>
      </div>
    </div>

    <ng-template #loadingTpl>
      <div class="loading" *ngIf="!err">Loading property…</div>
      <div class="page"><div class="alert alert-error" *ngIf="err">{{err}}</div></div>
    </ng-template>
  `,
  styles: [`.detail-stat { font-size: 14px; padding: 10px; background: var(--surface-2); border-radius: 8px; }`]
})
export class PropertyDetailComponent implements OnInit {
  property: PropertyDetail | null = null;
  reviews: Review[] = [];
  imgIdx = 0;
  avCheckIn = ''; avCheckOut = ''; avResult: boolean | null = null; blockedMsg = '';
  bookIn = ''; bookOut = ''; bookGuests = 1;
  booking = false; bookError = ''; bookSuccess = false; lastBookingId = '';
  err = '';
  propertyTypes = PROPERTY_TYPES;

  constructor(
    private route: ActivatedRoute, private router: Router,
    private propertySvc: PropertyService, private bookingSvc: BookingService,
    private reviewSvc: ReviewService, public auth: AuthService
  ) {}

  get isOwner() { return this.auth.isOwner(); }
  get isGuest() { return this.auth.isAuthenticated(); }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.propertySvc.getById(id).subscribe({
      next: r => { this.property = r.property; this.imgIdx = 0; },
      error: () => this.err = 'Property not found.'
    });
    this.reviewSvc.getPropertyReviews(id).subscribe({ next: r => this.reviews = r.reviews, error: () => {} });
  }

  currentImgSrc(): string {
    if (!this.property?.images.length) return '';
    const img = this.property.images[this.imgIdx];
    return `data:${img.contentType};base64,${img.base64Data}`;
  }
  prevImg() { this.imgIdx = (this.imgIdx - 1 + (this.property?.images.length || 1)) % (this.property?.images.length || 1); }
  nextImg() { this.imgIdx = (this.imgIdx + 1) % (this.property?.images.length || 1); }

  checkAvailability() {
    if (!this.avCheckIn || !this.avCheckOut || !this.property) return;
    this.propertySvc.getAvailability(this.property.id, this.avCheckIn, this.avCheckOut).subscribe({
      next: r => {
        this.avResult = r.isAvailable;
        if (!r.isAvailable && r.blockedPeriods.length) {
          this.blockedMsg = r.blockedPeriods.map(b => `${new Date(b.startDate).toLocaleDateString()} – ${new Date(b.endDate).toLocaleDateString()}`).join(', ');
        }
      }
    });
  }

  book() {
    if (!this.bookIn || !this.bookOut || !this.property) { this.bookError = 'Select dates.'; return; }
    this.booking = true; this.bookError = '';
    this.bookingSvc.makeBooking(this.property.id, this.bookIn, this.bookOut, this.bookGuests).subscribe({
      next: r => { this.bookSuccess = true; this.booking = false; this.lastBookingId = r?.bookingId || ''; },
      error: e => { this.bookError = e.error?.description || 'Booking failed'; this.booking = false; }
    });
  }

  verifyBooking(id: string) {
    this.bookingSvc.verify(id).subscribe({ next: () => location.reload(), error: () => {} });
  }
  completeBooking(id: string) {
    this.bookingSvc.complete(id).subscribe({ next: () => location.reload(), error: () => {} });
  }

  propertyTypeLabel(val: number): string { return PROPERTY_TYPES.find(t => t.value === val)?.label ?? ''; }
  statusClass(s: string): string { return `badge-${s.toLowerCase()}`; }
  starStr(r: number): string { return '★'.repeat(r) + '☆'.repeat(5 - r); }
}

