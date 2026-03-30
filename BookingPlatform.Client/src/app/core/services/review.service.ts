import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Review, ReviewCheck } from '../models/models';

@Injectable({ providedIn: 'root' })
export class ReviewService {
  private readonly API = 'https://localhost:7153/api';
  constructor(private http: HttpClient) {}

  getPropertyReviews(propertyId: string): Observable<{ reviews: Review[] }> {
    return this.http.get<{ reviews: Review[] }>(`${this.API}/properties/${propertyId}/reviews`);
  }

  getBookingReview(bookingId: string): Observable<ReviewCheck> {
    return this.http.get<ReviewCheck>(`${this.API}/bookings/${bookingId}/review`);
  }

  makeReview(bookingId: string, rating: number, comment?: string): Observable<any> {
    return this.http.post(`${this.API}/bookings/${bookingId}/review`, { bookingId, rating, comment });
  }
}
