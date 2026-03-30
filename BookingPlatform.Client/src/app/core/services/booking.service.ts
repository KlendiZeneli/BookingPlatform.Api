import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BookingDetail, BookingInfo } from '../models/models';

@Injectable({ providedIn: 'root' })
export class BookingService {
  private readonly API = 'https://localhost:7153/api';
  constructor(private http: HttpClient) {}

  makeBooking(propertyId: string, startDate: string, endDate: string, guestCount: number): Observable<any> {
    return this.http.post(`${this.API}/properties/${propertyId}/bookings`, { startDate, endDate, guestCount });
  }

  getMyBookings(): Observable<{ bookings: BookingInfo[] }> {
    return this.http.get<{ bookings: BookingInfo[] }>(`${this.API}/bookings/my`);
  }

  getById(id: string): Observable<BookingDetail> {
    return this.http.get<BookingDetail>(`${this.API}/bookings/${id}`);
  }

  cancel(id: string): Observable<any> { return this.http.post(`${this.API}/bookings/${id}/cancel`, {}); }
  verify(id: string): Observable<any> { return this.http.post(`${this.API}/bookings/${id}/verify`, {}); }
  complete(id: string): Observable<any> { return this.http.post(`${this.API}/bookings/${id}/complete`, {}); }
}
