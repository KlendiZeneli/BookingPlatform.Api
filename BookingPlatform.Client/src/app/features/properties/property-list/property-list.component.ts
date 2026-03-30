import { Component, OnInit } from '@angular/core';
import { PropertyService } from '../../../core/services/property.service';
import { PropertyListItem, PagedResponse, PROPERTY_TYPES, SORT_OPTIONS } from '../../../core/models/models';

@Component({ standalone: false,
  template: `
    <div class="page">
      <!-- Search bar -->
      <div class="search-bar">
        <div class="form-row-3">
          <div class="form-group" style="margin:0">
            <label>Location</label>
            <input type="text" placeholder="City or country" [(ngModel)]="filters.city">
          </div>
          <div class="form-group" style="margin:0">
            <label>Check-in</label>
            <input type="date" [(ngModel)]="filters.checkIn">
          </div>
          <div class="form-group" style="margin:0">
            <label>Check-out</label>
            <input type="date" [(ngModel)]="filters.checkOut">
          </div>
        </div>
        <div style="margin-top:10px;display:flex;gap:8px;flex-wrap:wrap;align-items:center">
          <button class="btn btn-secondary btn-sm" (click)="showFilters=!showFilters">
            {{showFilters ? '▲ Hide filters' : '▼ More filters'}}
          </button>
          <button class="btn btn-primary btn-sm" (click)="search()">Search</button>
          <button class="btn btn-secondary btn-sm" (click)="clearFilters()">Clear</button>
          <span class="text-muted" *ngIf="result">{{result.totalCount}} results</span>
        </div>

        <div class="filters-expanded" *ngIf="showFilters">
          <div class="form-row">
            <div class="form-group" style="margin:0">
              <label>Guests</label>
              <input type="number" min="1" placeholder="How many guests?" [(ngModel)]="filters.guests">
            </div>
            <div class="form-group" style="margin:0">
              <label>Min price / night</label>
              <input type="number" min="0" placeholder="€0" [(ngModel)]="filters.minPrice">
            </div>
            <div class="form-group" style="margin:0">
              <label>Max price / night</label>
              <input type="number" min="0" placeholder="Any" [(ngModel)]="filters.maxPrice">
            </div>
            <div class="form-group" style="margin:0">
              <label>Property type</label>
              <select [(ngModel)]="filters.propertyType">
                <option value="">Any</option>
                <option *ngFor="let t of propertyTypes" [value]="t.value">{{t.label}}</option>
              </select>
            </div>
            <div class="form-group" style="margin:0">
              <label>Min bedrooms</label>
              <input type="number" min="0" [(ngModel)]="filters.minBedrooms">
            </div>
            <div class="form-group" style="margin:0">
              <label>Min rating</label>
              <input type="number" min="0" max="5" step="0.1" [(ngModel)]="filters.minRating">
            </div>
          </div>
          <div class="form-group mt-8" style="margin-bottom:0">
            <label>Sort by</label>
            <div style="display:flex;gap:6px;flex-wrap:wrap;margin-top:4px">
              <button *ngFor="let s of sortOptions"
                class="chip-check" [class.selected]="filters.sortBy===s.value"
                (click)="filters.sortBy=s.value">{{s.label}}</button>
            </div>
          </div>
          <div class="form-group mt-8" style="margin-bottom:0" *ngIf="allAmenities.length">
            <label>Amenities</label>
            <div style="display:flex;gap:6px;flex-wrap:wrap;margin-top:4px">
              <button *ngFor="let a of allAmenities"
                class="chip-check" [class.selected]="isAmenitySelected(a)"
                (click)="toggleAmenity(a)">{{a}}</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Results -->
      <div class="loading" *ngIf="loading">Loading properties…</div>

      <div *ngIf="!loading && result">
        <div class="property-grid" *ngIf="result.items.length; else noResults">
          <a class="property-card" *ngFor="let p of result.items" [routerLink]="['/properties', p.id]">
            <div class="property-card-img">
              <img *ngIf="p.primaryImageBase64" [src]="imgSrc(p.primaryImageBase64, p.primaryImageContentType!)" [alt]="p.name">
              <span *ngIf="!p.primaryImageBase64">🏠</span>
            </div>
            <div class="property-card-body">
              <div class="property-card-name">{{p.name}}</div>
              <div class="property-card-location">📍 {{p.city}}, {{p.country}}</div>
              <div class="property-card-meta">
                <span>🛏 {{p.bedrooms}} bed</span>
                <span>🚿 {{p.bathrooms}} bath</span>
                <span>👥 up to {{p.maxGuests}}</span>
              </div>
              <div class="flex-between mt-8">
                <span class="property-card-price">€{{p.pricePerNight}}<span class="text-muted" style="font-weight:400">/night</span></span>
                <span class="stars">★ {{p.averageRating | number:'1.1-1'}} <span class="text-muted">({{p.reviewCount}})</span></span>
              </div>
            </div>
          </a>
        </div>

        <ng-template #noResults>
          <div class="empty"><div class="empty-icon">🔍</div><p>No properties found.<br>Try adjusting your filters.</p></div>
        </ng-template>

        <!-- Pagination -->
        <div class="pagination" *ngIf="result.totalPages > 1">
          <button class="page-btn" (click)="goPage(currentPage-1)" [disabled]="!result.hasPreviousPage">‹</button>
          <ng-container *ngFor="let p of pages()">
            <button class="page-btn" [class.active]="p===currentPage" (click)="goPage(p)">{{p}}</button>
          </ng-container>
          <button class="page-btn" (click)="goPage(currentPage+1)" [disabled]="!result.hasNextPage">›</button>
        </div>
      </div>
    </div>
  `
})
export class PropertyListComponent implements OnInit {
  result: PagedResponse<PropertyListItem> | null = null;
  loading = false; showFilters = false; currentPage = 1;
  allAmenities: string[] = [];
  selectedAmenities: string[] = [];
  propertyTypes = PROPERTY_TYPES; sortOptions = SORT_OPTIONS;
  filters: Record<string, any> = { sortBy: 'Relevance', pageSize: 12 };

  constructor(private svc: PropertyService) {}

  ngOnInit() {
    this.svc.getAmenities().subscribe(a => this.allAmenities = a);
    this.search();
  }

  search(page = 1) {
    this.currentPage = page; this.loading = true;
    const params = { ...this.filters, page, amenities: this.selectedAmenities };
    this.svc.search(params).subscribe({ next: r => { this.result = r; this.loading = false; }, error: () => this.loading = false });
  }

  clearFilters() { this.filters = { sortBy: 'Relevance', pageSize: 12 }; this.selectedAmenities = []; this.search(); }
  goPage(p: number) { this.search(p); window.scrollTo(0, 0); }
  pages(): number[] { if (!this.result) return []; return Array.from({ length: this.result.totalPages }, (_, i) => i + 1); }
  isAmenitySelected(a: string): boolean { return this.selectedAmenities.includes(a); }
  toggleAmenity(a: string) {
    const i = this.selectedAmenities.indexOf(a);
    if (i > -1) this.selectedAmenities.splice(i, 1); else this.selectedAmenities.push(a);
  }
  imgSrc(b64: string, ct: string): string { return `data:${ct};base64,${b64}`; }
}

