import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../../../core/services/user.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({ standalone: false,
  template: `
    <div class="page-sm">
      <div class="flex-between mb-16">
        <h1>Become an owner</h1>
        <a routerLink="/profile" class="btn btn-secondary btn-sm">← Profile</a>
      </div>
      <div class="card">
        <p class="text-muted" style="margin-bottom:20px">Create an owner profile to start listing properties. Your profile will be reviewed by an admin before you can publish listings.</p>
        <div class="alert alert-error" *ngIf="error">{{error}}</div>
        <div class="alert alert-success" *ngIf="done">Owner profile submitted! Awaiting verification. <a routerLink="/owner-profile" class="text-accent">View profile</a></div>
        <ng-container *ngIf="!done">
          <div class="form-group"><label>Identity card number</label><input type="text" [(ngModel)]="idCard" placeholder="ID123456"></div>
          <div class="form-group"><label>Business name <span class="text-muted">(optional)</span></label><input type="text" [(ngModel)]="business" placeholder="My Rentals Ltd."></div>
          <div class="form-group"><label>Credit card (for payouts)</label><input type="text" [(ngModel)]="credit" placeholder="4242 4242 4242 4242"></div>
          <button class="btn btn-primary btn-full" (click)="submit()" [disabled]="loading">{{loading ? 'Submitting…' : 'Submit owner profile'}}</button>
        </ng-container>
      </div>
    </div>
  `
})
export class BecomeOwnerComponent {
  idCard = ''; business = ''; credit = '';
  error = ''; done = false; loading = false;
  constructor(private svc: UserService, private auth: AuthService, private router: Router) {}
  submit() {
    if (!this.idCard || !this.credit) { this.error = 'Identity card and credit card are required.'; return; }
    this.loading = true; this.error = '';
    this.svc.createOwnerProfile(this.idCard, this.credit, this.business || undefined).subscribe({
      next: () => { this.done = true; this.loading = false; this.auth.loadCurrentUser(); },
      error: e => { this.error = e.error?.description || 'Failed'; this.loading = false; }
    });
  }
}

