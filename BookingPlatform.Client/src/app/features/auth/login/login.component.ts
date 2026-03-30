import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({ standalone: false,
  template: `
    <div class="page-sm" style="padding-top:64px">
      <div class="card">
        <h2>Sign in</h2>
        <p class="subtitle">Welcome back to BookingPlatform</p>
        <div class="alert alert-error" *ngIf="error">{{error}}</div>
        <form (ngSubmit)="login(emailInput.value, passwordInput.value)">
          <div class="form-group">
            <label>Email</label>
            <input #emailInput type="email" name="email" autocomplete="email" placeholder="you@example.com" [(ngModel)]="email">
          </div>
          <div class="form-group">
            <label>Password</label>
            <input #passwordInput type="password" name="password" autocomplete="current-password" placeholder="Password" [(ngModel)]="password">
          </div>
          <div class="flex-between mt-8 mb-16">
            <a routerLink="/forgot-password" class="text-muted" style="font-size:13px">Forgot password?</a>
          </div>
          <button type="submit" class="btn btn-primary btn-full" [disabled]="loading">
            {{loading ? 'Signing in…' : 'Sign in'}}
          </button>
        </form>
        <hr class="divider">
        <p class="text-muted" style="text-align:center">
          Don't have an account? <a routerLink="/register" class="text-accent">Register</a>
        </p>
      </div>
    </div>
  `
})
export class LoginComponent {
  email = ''; password = ''; error = ''; loading = false;
  constructor(private auth: AuthService, private router: Router) {}
  login(emailFromInput?: string, passwordFromInput?: string) {
    const email = (this.email || emailFromInput || '').trim();
    const password = this.password || passwordFromInput || '';

    this.email = email;
    this.password = password;

    this.loading = true; this.error = '';
    this.auth.login({ email, password }).subscribe({
      next: () => this.router.navigate(['/']),
      error: e => {
        this.error = e.error?.description || e.error || 'Login failed';
        this.loading = false;
      }
    });
  }
}

