import { Component } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';

@Component({ standalone: false,
  template: `
    <div class="page-sm">
      <div class="flex-between mb-16">
        <h1>Change password</h1>
        <a routerLink="/profile" class="btn btn-secondary btn-sm">← Profile</a>
      </div>
      <div class="card">
        <div class="alert alert-error" *ngIf="error">{{error}}</div>
        <div class="alert alert-success" *ngIf="done">Password changed!</div>
        <ng-container *ngIf="!done">
          <div class="form-group"><label>Current password</label><input type="password" [(ngModel)]="current" placeholder="Current password"></div>
          <div class="form-group"><label>New password</label><input type="password" [(ngModel)]="newPass" placeholder="Min 8 characters"></div>
          <div class="form-group"><label>Confirm new password</label><input type="password" [(ngModel)]="confirm" placeholder="Repeat new password"></div>
          <button class="btn btn-primary btn-full" (click)="change()" [disabled]="loading">{{loading ? 'Changing…' : 'Change password'}}</button>
        </ng-container>
      </div>
    </div>
  `
})
export class ChangePasswordComponent {
  current = ''; newPass = ''; confirm = '';
  error = ''; done = false; loading = false;
  constructor(private auth: AuthService) {}
  change() {
    if (!this.current || !this.newPass) { this.error = 'Fill all fields.'; return; }
    if (this.newPass !== this.confirm) { this.error = 'New passwords must match.'; return; }
    this.loading = true; this.error = '';
    this.auth.changePassword(this.current, this.newPass).subscribe({
      next: () => { this.done = true; this.loading = false; },
      error: e => { this.error = e.error?.description || 'Change failed'; this.loading = false; }
    });
  }
}

