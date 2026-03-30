import { Component } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';

@Component({ standalone: false,
  template: `
    <div class="page-sm" style="padding-top:64px">
      <div class="card">
        <h2>Forgot password</h2>
        <p class="subtitle">Enter your email and we'll send a reset link</p>
        <div class="alert alert-error" *ngIf="error">{{error}}</div>
        <div class="alert alert-success" *ngIf="sent">Check your email for a reset link.</div>
        <ng-container *ngIf="!sent">
          <div class="form-group">
            <label>Email</label>
            <input type="email" placeholder="you@example.com" [(ngModel)]="email">
          </div>
          <button class="btn btn-primary btn-full" (click)="send()" [disabled]="loading">
            {{loading ? 'Sending…' : 'Send reset link'}}
          </button>
        </ng-container>
        <hr class="divider">
        <p class="text-muted" style="text-align:center"><a routerLink="/login" class="text-accent">Back to sign in</a></p>
      </div>
    </div>
  `
})
export class ForgotPasswordComponent {
  email = ''; error = ''; sent = false; loading = false;
  constructor(private auth: AuthService) {}
  send() {
    if (!this.email) { this.error = 'Please enter your email.'; return; }
    this.loading = true; this.error = '';
    this.auth.requestPasswordReset(this.email).subscribe({
      next: () => { this.sent = true; this.loading = false; },
      error: e => { this.error = e.error?.description || 'Failed to send'; this.loading = false; }
    });
  }
}

