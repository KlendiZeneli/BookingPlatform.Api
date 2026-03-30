import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OwnerProfile, UserProfile } from '../models/models';

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly API = 'https://localhost:7153/api';
  constructor(private http: HttpClient) {}

  getMe(): Observable<UserProfile> { return this.http.get<UserProfile>(`${this.API}/users/me`); }

  updateMe(firstName: string, lastName: string, phoneNumber?: string): Observable<any> {
    return this.http.put(`${this.API}/users/me`, { firstName, lastName, phoneNumber });
  }

  deleteMe(): Observable<any> { return this.http.delete(`${this.API}/users/me`); }

  createOwnerProfile(identityCardNumber: string, creditCard: string, businessName?: string): Observable<any> {
    return this.http.post(`${this.API}/owner-profiles`, { identityCardNumber, creditCard, businessName });
  }

  getOwnerProfile(): Observable<OwnerProfile> {
    return this.http.get<OwnerProfile>(`${this.API}/owner-profiles/me`);
  }
}
