import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../core/services/user.service';
import { OwnerProfile } from '../../../core/models/models';

@Component({ standalone: false,
  template: `
    <div class="page-sm">
      <div class="flex-between mb-16">
        <h1>Owner profile</h1>
        <a routerLink="/profile" class="btn btn-secondary btn-sm">← Profile</a>
      </div>
      <div class="loading" *ngIf="!profile && !error">Loading…</div>
      <div class="alert alert-error" *ngIf="error">{{error}}</div>
      <div class="card" *ngIf="profile">
        <div class="detail-row">
          <span class="detail-label">Verification status</span>
          <span class="badge" [ngClass]="statusClass(profile.verificationStatus)">{{profile.verificationStatus}}</span>
        </div>
        <div class="detail-row"><span class="detail-label">Business name</span><span>{{profile.businessName || '—'}}</span></div>
        <div class="detail-row"><span class="detail-label">Identity card</span><span>{{profile.identityCardNumber}}</span></div>
        <div class="detail-row" *ngIf="profile.verifiedAt"><span class="detail-label">Verified on</span><span>{{profile.verifiedAt | date:'mediumDate'}}</span></div>
        <div class="detail-row" *ngIf="profile.verificationNotes"><span class="detail-label">Notes</span><span>{{profile.verificationNotes}}</span></div>
        <div class="detail-row"><span class="detail-label">Member since</span><span>{{profile.createdAt | date:'mediumDate'}}</span></div>
        <hr class="divider">
        <div class="alert alert-info" *ngIf="profile.verificationStatus==='Pending'">Your profile is under review. You'll be able to list properties once verified.</div>
        <a routerLink="/my-properties" class="btn btn-primary" *ngIf="profile.verificationStatus==='Verified'">Manage properties</a>
      </div>
    </div>
  `
})
export class OwnerProfileComponent implements OnInit {
  profile: OwnerProfile | null = null; error = '';
  constructor(private svc: UserService) {}
  ngOnInit() { this.svc.getOwnerProfile().subscribe({ next: p => this.profile = p, error: e => this.error = e.error?.description || 'Profile not found' }); }
  statusClass(s: string): string { return `badge-${s.toLowerCase()}`; }
}

