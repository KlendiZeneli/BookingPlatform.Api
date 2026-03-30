import { Component, OnInit } from '@angular/core';
import { PropertyService } from '../../../core/services/property.service';
import { MyProperty, PROPERTY_TYPES } from '../../../core/models/models';

@Component({ standalone: false,
  template: `
    <div class="page">
      <div class="flex-between mb-16">
        <div><h1>My properties</h1><p class="subtitle">Manage your listings</p></div>
        <a routerLink="/properties/new" class="btn btn-primary">+ New property</a>
      </div>

      <div class="loading" *ngIf="loading">Loading…</div>

      <div class="empty" *ngIf="!loading && !properties.length">
        <div class="empty-icon">🏠</div>
        <p>You haven't listed any properties yet.</p>
        <a routerLink="/properties/new" class="btn btn-primary mt-16">Create your first listing</a>
      </div>

      <div class="property-grid" *ngIf="!loading && properties.length">
        <div class="property-card" style="cursor:default" *ngFor="let p of properties">
          <div class="property-card-img" style="background:var(--surface-2);display:flex;align-items:center;justify-content:center;height:100px">
            <span style="font-size:32px">🏠</span>
          </div>
          <div class="property-card-body">
            <div class="flex-between">
              <div class="property-card-name">{{p.name}}</div>
              <span class="badge" [class.badge-confirmed]="p.isActive" [class.badge-cancelled]="!p.isActive">
                {{p.isActive ? 'Active' : 'Inactive'}}
              </span>
            </div>
            <div class="property-card-location">📍 {{p.city}}, {{p.country}}</div>
            <div class="property-card-location">{{typeLabel(p.propertyType)}}</div>
            <div class="property-card-meta">
              <span>🛏 {{p.bedrooms}} bed</span>
              <span>👥 {{p.maxGuests}} guests</span>
              <span>★ {{p.averageRating | number:'1.1-1'}}</span>
            </div>
            <div style="display:flex;gap:8px;font-size:13px;margin-top:4px">
              <span style="color:var(--warning)">⏳ {{p.pendingBookingsCount}} pending</span>
              <span style="color:var(--success)">✓ {{p.confirmedBookingsCount}} confirmed</span>
            </div>
            <div class="property-card-price mt-8">€{{p.pricePerNight}}/night</div>
            <div class="btn-group mt-8">
              <a [routerLink]="['/properties', p.id]" class="btn btn-secondary btn-sm">View</a>
              <a [routerLink]="['/properties', p.id, 'edit']" class="btn btn-secondary btn-sm">Edit</a>
              <button class="btn btn-danger btn-sm" (click)="del(p.id)" [disabled]="deleting===p.id">Delete</button>
            </div>
          </div>
        </div>
      </div>

      <div class="alert alert-error mt-16" *ngIf="delError">{{delError}}</div>
    </div>
  `
})
export class MyPropertiesComponent implements OnInit {
  properties: MyProperty[] = [];
  loading = false; deleting = ''; delError = '';
  constructor(private svc: PropertyService) {}
  ngOnInit() {
    this.loading = true;
    this.svc.getMyProperties().subscribe({ next: r => { this.properties = r.properties; this.loading = false; }, error: () => this.loading = false });
  }
  typeLabel(t: string): string { return PROPERTY_TYPES.find(p => p.label.toLowerCase() === t.toLowerCase())?.label ?? t; }
  del(id: string) {
    if (!confirm('Delete this property?')) return;
    this.deleting = id; this.delError = '';
    this.svc.delete(id).subscribe({
      next: () => { this.properties = this.properties.filter(p => p.id !== id); this.deleting = ''; },
      error: e => { this.delError = e.error?.description || 'Delete failed'; this.deleting = ''; }
    });
  }
}

