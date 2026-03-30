import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { filter, map, take } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class OwnerGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}
  canActivate(): Observable<boolean> {
    return this.auth.initialized$.pipe(
      filter(ready => ready),
      take(1),
      map(() => {
        if (!this.auth.isAuthenticated()) {
          this.router.navigate(['/login']);
          return false;
        }
        if (this.auth.isOwner()) return true;
        this.router.navigate(['/']);
        return false;
      })
    );
  }
}
