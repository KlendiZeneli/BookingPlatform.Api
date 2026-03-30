import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({ standalone: false,
  template: `
    <div class="page-sm" style="padding-top:64px">
      <div class="card">
        <h2>Create account</h2>
        <p class="subtitle">Join BookingPlatform today</p>
        <div class="alert alert-error" *ngIf="error">{{error}}</div>
        <div class="alert alert-success" *ngIf="success">Account created! <a routerLink="/login" class="text-accent">Sign in</a></div>
        <div class="form-row">
          <div class="form-group">
            <label>First name</label>
            <input type="text" placeholder="John" [(ngModel)]="firstName">
          </div>
          <div class="form-group">
            <label>Last name</label>
            <input type="text" placeholder="Doe" [(ngModel)]="lastName">
          </div>
        </div>
        <div class="form-group">
          <label>Email</label>
          <input type="email" placeholder="you@example.com" [(ngModel)]="email">
        </div>
        <div class="form-group">
          <label>Password</label>
          <input type="password" placeholder="Min 8 characters" [(ngModel)]="password">
        </div>
        <button class="btn btn-primary btn-full mt-16" (click)="register()" [disabled]="loading">
          {{loading ? 'Creating…' : 'Create account'}}
        </button>
        <hr class="divider">
        <p class="text-muted" style="text-align:center">
          Already have an account? <a routerLink="/login" class="text-accent">Sign in</a>
        </p>
      </div>
    </div>
  `
})
export class RegisterComponent {
  firstName = ''; lastName = ''; email = ''; password = '';
  error = ''; success = false; loading = false;
  constructor(private auth: AuthService, private router: Router) {}
  register() {
    if (!this.firstName || !this.lastName || !this.email || !this.password) { this.error = 'All fields required.'; return; }
    this.loading = true; this.error = '';
    this.auth.register({ firstName: this.firstName, lastName: this.lastName, email: this.email, password: this.password }).subscribe({
      next: () => { this.success = true; this.loading = false; },
      error: e => { this.error = e.error?.description || e.error || 'Registration failed'; this.loading = false; }
    });
  }
}

