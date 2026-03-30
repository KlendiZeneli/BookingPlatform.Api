import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ReviewService } from '../../../core/services/review.service';

@Component({ standalone: false,
  template: `
    <div class="page-sm">
      <div class="flex-between mb-16">
        <h1>Leave a review</h1>
        <a [routerLink]="['/bookings', bookingId]" class="btn btn-secondary btn-sm">← Back</a>
      </div>
      <div class="card">
        <div class="alert alert-error" *ngIf="error">{{error}}</div>
        <div class="alert alert-success" *ngIf="done">Review submitted! <a routerLink="/my-bookings" class="text-accent">My bookings</a></div>
        <ng-container *ngIf="!done">
          <div class="form-group">
            <label>Rating</label>
            <div style="display:flex;gap:8px;margin-top:4px">
              <button *ngFor="let s of [1,2,3,4,5]" (click)="rating=s"
                style="font-size:24px;background:none;border:none;cursor:pointer;transition:transform .1s"
                [style.transform]="rating>=s ? 'scale(1.15)' : 'scale(1)'">
                {{rating>=s ? '★' : '☆'}}
              </button>
            </div>
          </div>
          <div class="form-group">
            <label>Comment (optional)</label>
            <textarea [(ngModel)]="comment" placeholder="Share your experience…"></textarea>
          </div>
          <button class="btn btn-primary btn-full" (click)="submit()" [disabled]="loading || rating===0">
            {{loading ? 'Submitting…' : 'Submit review'}}
          </button>
        </ng-container>
      </div>
    </div>
  `
})
export class MakeReviewComponent implements OnInit {
  bookingId = ''; rating = 0; comment = '';
  loading = false; done = false; error = '';
  constructor(private route: ActivatedRoute, private svc: ReviewService, private router: Router) {}
  ngOnInit() { this.bookingId = this.route.snapshot.paramMap.get('id')!; }
  submit() {
    if (this.rating === 0) { this.error = 'Select a rating.'; return; }
    this.loading = true; this.error = '';
    this.svc.makeReview(this.bookingId, this.rating, this.comment || undefined).subscribe({
      next: () => { this.done = true; this.loading = false; },
      error: e => { this.error = e.error?.description || 'Submit failed'; this.loading = false; }
    });
  }
}

