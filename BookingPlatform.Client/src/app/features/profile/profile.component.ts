import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { UserService } from '../../core/services/user.service';
import { UserProfile } from '../../core/models/models';
import { Router } from '@angular/router';

@Component({ standalone: false,
  template: `
    <div class="page-md">
      <h1>My profile</h1>
      <p class="subtitle">Manage your account information</p>

      <div class="alert alert-error" *ngIf="error">{{error}}</div>
      <div class="alert alert-success" *ngIf="saved">Profile updated!</div>

      <div class="card" style="margin-bottom:16px" *ngIf="user">
        <div class="section-title">Account</div>
        <div class="detail-row"><span class="detail-label">Email</span><span>{{user.email}}</span></div>
        <div class="detail-row"><span class="detail-label">Roles</span>
          <div style="display:flex;gap:6px">
            <span class="badge badge-confirmed" *ngFor="let r of user.roles">{{r}}</span>
          </div>
        </div>
        <div class="detail-row"><span class="detail-label">Status</span>
          <span class="badge" [class.badge-confirmed]="user.isActive" [class.badge-cancelled]="!user.isActive">{{user.isActive ? 'Active' : 'Deactivated'}}</span>
        </div>
      </div>

      <div class="card" style="margin-bottom:16px">
        <div class="section-title">Edit info</div>
        <div class="form-row">
          <div class="form-group"><label>First name</label><input type="text" [(ngModel)]="firstName"></div>
          <div class="form-group"><label>Last name</label><input type="text" [(ngModel)]="lastName"></div>
        </div>
        <div class="form-group"><label>Phone number</label><input type="text" [(ngModel)]="phone" placeholder="+1 555 000 0000"></div>
        <button class="btn btn-primary" (click)="save()" [disabled]="saving">{{saving ? 'Saving…' : 'Save changes'}}</button>
      </div>

      <div class="card">
        <div class="section-title">Security & account actions</div>
        <div class="btn-group">
          <a routerLink="/change-password" class="btn btn-secondary">Change password</a>
          <a routerLink="/become-owner" class="btn btn-secondary" *ngIf="!isOwner">Become an owner</a>
          <a routerLink="/owner-profile" class="btn btn-secondary" *ngIf="isOwner">Owner profile</a>
          <button class="btn btn-danger" (click)="deleteAccount()">Delete account</button>
        </div>
      </div>
    </div>
  `
})
export class ProfileComponent implements OnInit {
  user: UserProfile | null = null;
  firstName = ''; lastName = ''; phone = '';
  saving = false; saved = false; error = '';
  get isOwner() { return this.auth.isOwner(); }

  constructor(private auth: AuthService, private userSvc: UserService, private router: Router) {}

  ngOnInit() {
    this.auth.user$.subscribe(u => {
      if (!u) return;
      this.user = u;
      this.firstName = u.firstName; this.lastName = u.lastName; this.phone = u.phoneNumber || '';
    });
  }

  save() {
    this.saving = true; this.error = ''; this.saved = false;
    this.userSvc.updateMe(this.firstName, this.lastName, this.phone || undefined).subscribe({
      next: () => { this.saved = true; this.saving = false; this.auth.loadCurrentUser(); },
      error: e => { this.error = e.error?.description || 'Save failed'; this.saving = false; }
    });
  }

  deleteAccount() {
    if (!confirm('This will deactivate your account. Continue?')) return;
    this.userSvc.deleteMe().subscribe({ next: () => this.auth.logout(), error: e => this.error = e.error?.description || 'Failed' });
  }
}

