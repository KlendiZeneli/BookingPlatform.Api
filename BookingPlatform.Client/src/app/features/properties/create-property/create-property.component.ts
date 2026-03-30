import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PropertyService } from '../../../core/services/property.service';
import { PROPERTY_TYPES } from '../../../core/models/models';

interface ImagePreview { base64Data: string; contentType: string; isPrimary: boolean; }

@Component({ standalone: false,
  template: `
    <div class="page-md">
      <div class="flex-between mb-16">
        <h1>Create property</h1>
        <a routerLink="/my-properties" class="btn btn-secondary btn-sm">← My properties</a>
      </div>

      <div class="alert alert-error" *ngIf="error">{{error}}</div>
      <div class="alert alert-success" *ngIf="done">Property created! <a routerLink="/my-properties" class="text-accent">View properties</a></div>

      <div class="card">
        <div class="section-title">Basic info</div>
        <div class="form-group"><label>Name</label><input type="text" [(ngModel)]="form.name" placeholder="Beautiful apartment in Paris"></div>
        <div class="form-group"><label>Description</label><textarea [(ngModel)]="form.description" placeholder="Describe your property…"></textarea></div>
        <div class="form-row">
          <div class="form-group">
            <label>Property type</label>
            <select [(ngModel)]="form.propertyType">
              <option *ngFor="let t of propertyTypes" [value]="t.value">{{t.label}}</option>
            </select>
          </div>
          <div class="form-group"><label>Price per night (€)</label><input type="number" min="1" [(ngModel)]="form.pricePerNight"></div>
        </div>
        <div class="form-row-3">
          <div class="form-group"><label>Max guests</label><input type="number" min="1" [(ngModel)]="form.maxGuests"></div>
          <div class="form-group"><label>Bedrooms</label><input type="number" min="0" [(ngModel)]="form.bedrooms"></div>
          <div class="form-group"><label>Beds</label><input type="number" min="0" [(ngModel)]="form.beds"></div>
          <div class="form-group"><label>Bathrooms</label><input type="number" min="0" [(ngModel)]="form.bathrooms"></div>
          <div class="form-group"><label>Check-in time</label><input type="time" [(ngModel)]="form.checkInTime"></div>
          <div class="form-group"><label>Check-out time</label><input type="time" [(ngModel)]="form.checkOutTime"></div>
        </div>
      </div>

      <div class="card mt-16">
        <div class="section-title">Address</div>
        <div class="form-row">
          <div class="form-group"><label>Country</label><input type="text" [(ngModel)]="form.address.country" placeholder="France"></div>
          <div class="form-group"><label>City</label><input type="text" [(ngModel)]="form.address.city" placeholder="Paris"></div>
          <div class="form-group"><label>Street</label><input type="text" [(ngModel)]="form.address.street" placeholder="123 Rue de Rivoli"></div>
          <div class="form-group"><label>Postal code</label><input type="text" [(ngModel)]="form.address.postalCode" placeholder="75001"></div>
        </div>
      </div>

      <div class="card mt-16" *ngIf="allAmenities.length">
        <div class="section-title">Amenities</div>
        <div class="chips">
          <button *ngFor="let a of allAmenities" class="chip-check" [class.selected]="isSelected(a)" (click)="toggleAmenity(a)">{{a}}</button>
        </div>
      </div>

      <div class="card mt-16">
        <div class="section-title">Photos</div>
        <input type="file" accept="image/*" multiple (change)="onFiles($event)" style="margin-bottom:12px">
        <div style="display:flex;gap:8px;flex-wrap:wrap">
          <div *ngFor="let img of images; let i=index" style="position:relative">
            <img [src]="'data:'+img.contentType+';base64,'+img.base64Data" style="width:100px;height:80px;object-fit:cover;border-radius:8px;border:2px solid"
              [style.border-color]="img.isPrimary ? 'var(--accent)' : 'var(--border)'">
            <button style="position:absolute;top:2px;right:2px;background:rgba(0,0,0,.6);color:#fff;border:none;border-radius:50%;width:20px;height:20px;cursor:pointer;font-size:11px" (click)="removeImg(i)">✕</button>
            <button style="position:absolute;bottom:2px;left:50%;transform:translateX(-50%);background:rgba(0,0,0,.6);color:#fff;border:none;border-radius:4px;padding:1px 4px;cursor:pointer;font-size:10px" (click)="setPrimary(i)">{{img.isPrimary?'✓ Primary':'Set primary'}}</button>
          </div>
        </div>
      </div>

      <div class="mt-24">
        <button class="btn btn-primary" (click)="create()" [disabled]="loading">{{loading ? 'Creating…' : 'Create property'}}</button>
        <a routerLink="/my-properties" class="btn btn-secondary" style="margin-left:8px">Cancel</a>
      </div>
    </div>
  `
})
export class CreatePropertyComponent implements OnInit {
  propertyTypes = PROPERTY_TYPES;
  allAmenities: string[] = [];
  selectedAmenities: string[] = [];
  images: ImagePreview[] = [];
  error = ''; done = false; loading = false;
  form = {
    name: '', description: '', propertyType: 0, pricePerNight: 0,
    maxGuests: 1, bedrooms: 1, beds: 1, bathrooms: 1,
    checkInTime: '14:00', checkOutTime: '11:00',
    address: { country: '', city: '', street: '', postalCode: '' }
  };

  constructor(private svc: PropertyService, private router: Router) {}

  ngOnInit() { this.svc.getAmenities().subscribe(a => this.allAmenities = a); }

  isSelected(a: string) { return this.selectedAmenities.includes(a); }
  toggleAmenity(a: string) {
    const i = this.selectedAmenities.indexOf(a);
    if (i > -1) this.selectedAmenities.splice(i, 1); else this.selectedAmenities.push(a);
  }

  onFiles(e: Event) {
    const files = Array.from((e.target as HTMLInputElement).files || []);
    files.forEach(f => {
      const reader = new FileReader();
      reader.onload = ev => {
        const result = ev.target?.result as string;
        const base64 = result.split(',')[1];
        this.images.push({ base64Data: base64, contentType: f.type, isPrimary: this.images.length === 0 });
      };
      reader.readAsDataURL(f);
    });
  }

  removeImg(i: number) {
    this.images.splice(i, 1);
    if (this.images.length && !this.images.find(img => img.isPrimary)) this.images[0].isPrimary = true;
  }

  setPrimary(i: number) { this.images.forEach((img, idx) => img.isPrimary = idx === i); }

  create() {
    const { name, description, propertyType, pricePerNight, address } = this.form;
    if (!name || !description || !address.country || !address.city) { this.error = 'Fill all required fields.'; return; }
    this.loading = true; this.error = '';
    const payload = { ...this.form, amenityNames: this.selectedAmenities, images: this.images };
    this.svc.create(payload).subscribe({
      next: () => { this.done = true; this.loading = false; },
      error: e => { this.error = e.error?.description || 'Create failed'; this.loading = false; }
    });
  }
}

