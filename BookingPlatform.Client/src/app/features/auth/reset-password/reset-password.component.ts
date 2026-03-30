import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({ standalone: false,
  template: `
    <div class="page-sm" style="padding-top:64px">
      <div class="card">
        <h2>Reset password</h2>
        <p class="subtitle">Enter your new password</p>
        <div class="alert alert-error" *ngIf="error">{{error}}</div>
        <div class="alert alert-success" *ngIf="done">Password reset! <a routerLink="/login" class="text-accent">Sign in</a></div>
        <ng-container *ngIf="!done">
          <div class="form-group">
            <label>New password</label>
            <input type="password" placeholder="Min 8 characters" [(ngModel)]="password">
          </div>
          <div class="form-group">
            <label>Confirm password</label>
            <input type="password" placeholder="Repeat password" [(ngModel)]="confirm">
          </div>
          <button class="btn btn-primary btn-full" (click)="reset()" [disabled]="loading || !token">
            {{loading ? 'Resetting…' : 'Reset password'}}
          </button>
        </ng-container>
      </div>
    </div>
  `
})
export class ResetPasswordComponent implements OnInit {
  token = ''; password = ''; confirm = ''; error = ''; done = false; loading = false;
  constructor(private route: ActivatedRoute, private auth: AuthService, private router: Router) {}
  ngOnInit() { this.token = this.route.snapshot.queryParamMap.get('token') || ''; }
  reset() {
    if (!this.password || this.password !== this.confirm) { this.error = 'Passwords must match and not be empty.'; return; }
    this.loading = true; this.error = '';
    this.auth.resetPassword(this.token, this.password).subscribe({
      next: () => { this.done = true; this.loading = false; },
      error: e => { this.error = e.error?.description || 'Reset failed'; this.loading = false; }
    });
  }
}

