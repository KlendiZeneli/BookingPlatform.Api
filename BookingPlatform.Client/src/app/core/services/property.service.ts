import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Availability, MyProperty, PagedResponse, PropertyDetail, PropertyListItem } from '../models/models';

@Injectable({ providedIn: 'root' })
export class PropertyService {
  private readonly API = 'https://localhost:7153/api';
  constructor(private http: HttpClient) {}

  search(filters: Record<string, any> = {}): Observable<PagedResponse<PropertyListItem>> {
    let params = new HttpParams();
    Object.entries(filters).forEach(([k, v]) => {
      if (v === null || v === undefined || v === '') return;
      if (Array.isArray(v)) { v.forEach(i => { params = params.append(k, i); }); }
      else { params = params.set(k, String(v)); }
    });
    return this.http.get<PagedResponse<PropertyListItem>>(`${this.API}/properties`, { params });
  }

  getById(id: string): Observable<{ property: PropertyDetail }> {
    return this.http.get<{ property: PropertyDetail }>(`${this.API}/properties/${id}`);
  }

  getMyProperties(): Observable<{ properties: MyProperty[] }> {
    return this.http.get<{ properties: MyProperty[] }>(`${this.API}/properties/my`);
  }

  getAvailability(id: string, checkIn: string, checkOut: string): Observable<Availability> {
    const params = new HttpParams().set('checkIn', checkIn).set('checkOut', checkOut);
    return this.http.get<Availability>(`${this.API}/properties/${id}/availability`, { params });
  }

  getAmenities(): Observable<string[]> {
    return this.http.get<string[]>(`${this.API}/amenities`);
  }

  create(data: any): Observable<any> { return this.http.post(`${this.API}/properties`, data); }

  update(id: string, data: any): Observable<any> {
    return this.http.put(`${this.API}/properties/${id}`, { ...data, propertyId: id });
  }

  delete(id: string): Observable<any> { return this.http.delete(`${this.API}/properties/${id}`); }
}
