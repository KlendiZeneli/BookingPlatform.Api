import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, finalize, map, of, switchMap, tap, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { LoginRequest, LoginResponse, RegisterRequest, UserProfile } from '../models/models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly API = 'https://localhost:7153/api';
  private readonly TOKEN_KEY = 'token';
  private userSubject = new BehaviorSubject<UserProfile | null>(null);
  user$ = this.userSubject.asObservable();
  private initializedSubject = new BehaviorSubject<boolean>(false);
  initialized$ = this.initializedSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {
    this.initializeSession();
  }

  login(req: LoginRequest): Observable<LoginResponse> {
    return this.http.post<any>(`${this.API}/auth/login`, req).pipe(
      map(res => ({ token: res?.token ?? res?.Token } as LoginResponse)),
      switchMap(res => {
        if (!res.token) return throwError(() => new Error('Invalid login response'));
        localStorage.setItem(this.TOKEN_KEY, res.token);
        return this.fetchCurrentUser().pipe(map(() => res));
      })
    );
  }

  register(req: RegisterRequest): Observable<any> {
    return this.http.post(`${this.API}/auth/register`, req);
  }

  logout(): void {
    this.clearSession();
    this.router.navigate(['/login']);
  }

  handleUnauthorized(): void {
    this.clearSession();
    this.router.navigate(['/login']);
  }

  private clearSession(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.userSubject.next(null);
  }

  getToken(): string | null { return localStorage.getItem(this.TOKEN_KEY); }
  isAuthenticated(): boolean { return !!this.getToken(); }
  getUser(): UserProfile | null { return this.userSubject.value; }
  hasRole(role: string): boolean { return this.userSubject.value?.roles?.includes(role) ?? false; }
  isOwner(): boolean { return this.hasRole('Owner'); }
  isGuest(): boolean { return this.hasRole('Guest'); }

  private initializeSession(): void {
    const token = this.getToken();
    if (!token) {
      this.userSubject.next(null);
      this.initializedSubject.next(true);
      return;
    }

    const tokenUser = this.getUserFromToken(token);
    if (tokenUser) {
      this.userSubject.next(tokenUser);
    }

    this.fetchCurrentUser().pipe(
      catchError(err => {
        if (err?.status === 401 && this.isTokenExpired(token)) {
          this.clearSession();
        }
        return of(null);
      }),
      finalize(() => this.initializedSubject.next(true))
    ).subscribe();
  }

  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      if (!payload?.exp) return false;
      return Date.now() >= payload.exp * 1000;
    } catch {
      return false;
    }
  }

  private getUserFromToken(token: string): UserProfile | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const roleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
      const idClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
      const emailClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';
      const givenNameClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname';
      const surnameClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname';

      const rolesRaw = payload?.[roleClaim] ?? payload?.role ?? [];
      const roles = Array.isArray(rolesRaw) ? rolesRaw : [rolesRaw].filter(Boolean);

      return {
        id: payload?.[idClaim] ?? '',
        firstName: payload?.[givenNameClaim] ?? '',
        lastName: payload?.[surnameClaim] ?? '',
        email: payload?.[emailClaim] ?? '',
        isActive: true,
        roles
      };
    } catch {
      return null;
    }
  }

  loadCurrentUser(): void {
    this.fetchCurrentUser().subscribe({
      error: err => {
        if (err?.status === 401) this.handleUnauthorized();
      }
    });
  }

  private fetchCurrentUser(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.API}/users/me`).pipe(
      tap(u => this.userSubject.next(u))
    );
  }

  requestPasswordReset(email: string): Observable<any> {
    return this.http.post(`${this.API}/auth/request-password-reset`, { email });
  }

  resetPassword(token: string, newPassword: string): Observable<any> {
    return this.http.post(`${this.API}/auth/reset-password`, { token, newPassword });
  }

  changePassword(currentPassword: string, newPassword: string): Observable<any> {
    return this.http.post(`${this.API}/auth/change-password`, { currentPassword, newPassword });
  }
}
