import { Component, OnInit, HostListener } from '@angular/core';
import { AuthService } from './core/services/auth.service';
import { UserProfile, Notification } from './core/models/models';
import { NotificationService } from './core/services/notification.service';

@Component({ standalone: false,
  selector: 'app-root',
  template: `
    <nav class="navbar">
      <a routerLink="/" class="nav-brand">BookingPlatform</a>
      <div class="nav-links">
        <a routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{exact:true}" class="nav-link">Properties</a>
        <ng-container *ngIf="user">
          <a routerLink="/my-bookings" routerLinkActive="active" class="nav-link">My bookings</a>
          <a routerLink="/my-properties" routerLinkActive="active" class="nav-link" *ngIf="auth.isOwner()">My properties</a>
        </ng-container>
      </div>
      <div class="nav-actions">
        <ng-container *ngIf="!user">
          <a routerLink="/login" class="btn btn-secondary btn-sm">Sign in</a>
          <a routerLink="/register" class="btn btn-primary btn-sm">Register</a>
        </ng-container>
        <div class="nav-actions-inner" *ngIf="user">
          <div class="nav-notif">
            <button class="nav-notif-btn" (click)="toggleNotif()">
              🔔
              <span *ngIf="unreadCount>0" class="badge">{{unreadCount}}</span>
            </button>
            <div class="nav-notif-dropdown" *ngIf="notifOpen">
              <div class="notif-dropdown-header" style="display:flex;align-items:center;justify-content:space-between;padding:8px 12px;border-bottom:1px solid rgba(255,255,255,0.03);">
                <strong>Notifications</strong>
                <button class="btn btn-ghost" (click)="notif.markAllRead()">Mark all read</button>
              </div>
              <div *ngIf="notifications.length===0" class="notif-empty">No notifications</div>
              <a *ngFor="let n of notifications" class="notif-item" [class.unread]="!n.isRead" role="button" (click)="closeNotif()">
                <div class="notif-content">
                  <div class="notif-title">{{n.title}}</div>
                  <div *ngIf="n.message" class="notif-message">{{n.message}}</div>
                </div>
                <div class="notif-meta">
                  <div class="notif-time">{{n.createdOnUtc | date:'short'}}</div>
                </div>
              </a>
            </div>
          </div>
          <div class="nav-user">
            <button class="nav-user-btn" (click)="dropOpen=!dropOpen">
              <span>{{user.firstName}} {{user.lastName}}</span>
              <span>▾</span>
            </button>
            <div class="nav-dropdown" *ngIf="dropOpen">
              <a routerLink="/profile" (click)="close()">👤 Profile</a>
              <a routerLink="/my-bookings" (click)="close()">📋 My bookings</a>
              <a routerLink="/my-properties" *ngIf="auth.isOwner()" (click)="close()">🏠 My properties</a>
              <a routerLink="/owner-profile" *ngIf="auth.isOwner()" (click)="close()">🏷 Owner profile</a>
              <a routerLink="/become-owner" *ngIf="!auth.isOwner()" (click)="close()">🚀 Become an owner</a>
              <hr>
              <a routerLink="/change-password" (click)="close()">🔐 Change password</a>
              <button (click)="logout()" class="danger">Sign out</button>
            </div>
          </div>
        </div>
      </div>
    </nav>
    <router-outlet></router-outlet>
  `
})
export class AppComponent implements OnInit {
  user: UserProfile | null = null;
  dropOpen = false;
  notifOpen = false;
  notifications: Notification[] = [];
  constructor(public auth: AuthService, public notif: NotificationService) {}
  ngOnInit() {
    this.auth.user$.subscribe(u => { this.user = u; this.dropOpen = false; });
    this.notif.notifications$.subscribe(n => this.notifications = n);
  }
  toggleNotif() {
    this.notifOpen = !this.notifOpen;
    if (this.notifOpen) this.notif.markAllRead();
  }
  get unreadCount(): number { return this.notifications.filter(n => !n.isRead).length; }
  markAllRead() { this.notif.markAllRead(); }
  closeNotif() { this.notifOpen = false; }
  close() { this.dropOpen = false; }
  logout() { this.close(); this.auth.logout(); }

  @HostListener('document:click', ['$event'])
  onDoc(e: MouseEvent) {
    if (!(e.target as HTMLElement).closest('.nav-user')) this.dropOpen = false;
    if (!(e.target as HTMLElement).closest('.nav-notif')) this.notifOpen = false;
  }
}

