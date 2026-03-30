import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PropertyService } from '../../../core/services/property.service';
import { PropertyDetail, PROPERTY_TYPES } from '../../../core/models/models';

interface ImagePreview { base64Data: string; contentType: string; isPrimary: boolean; }

@Component({ standalone: false,
  template: `
    <div class="page-md">
      <div class="flex-between mb-16">
        <h1>Edit property</h1>
        <a routerLink="/my-properties" class="btn btn-secondary btn-sm">← My properties</a>
      </div>

      <div class="loading" *ngIf="!loaded && !error">Loading…</div>
      <div class="alert alert-error" *ngIf="error">{{error}}</div>
      <div class="alert alert-success" *ngIf="saved">Changes saved!</div>

      <ng-container *ngIf="loaded">
        <div class="card">
          <div class="section-title">Basic info</div>
          <div class="form-group"><label>Name</label><input type="text" [(ngModel)]="form.name"></div>
          <div class="form-group"><label>Description</label><textarea [(ngModel)]="form.description"></textarea></div>
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
          <div class="form-group">
            <label style="display:flex;align-items:center;gap:8px;cursor:pointer">
              <input type="checkbox" [(ngModel)]="form.isActive" style="width:auto"> Active listing
            </label>
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
          <p class="input-note">Uploading new images will replace the current ones.</p>
          <div style="display:flex;gap:8px;flex-wrap:wrap;margin-top:8px">
            <div *ngFor="let img of images; let i=index" style="position:relative">
              <img [src]="'data:'+img.contentType+';base64,'+img.base64Data" style="width:100px;height:80px;object-fit:cover;border-radius:8px;border:2px solid"
                [style.border-color]="img.isPrimary ? 'var(--accent)' : 'var(--border)'">
              <button style="position:absolute;top:2px;right:2px;background:rgba(0,0,0,.6);color:#fff;border:none;border-radius:50%;width:20px;height:20px;cursor:pointer;font-size:11px" (click)="removeImg(i)">✕</button>
              <button style="position:absolute;bottom:2px;left:50%;transform:translateX(-50%);background:rgba(0,0,0,.6);color:#fff;border:none;border-radius:4px;padding:1px 4px;cursor:pointer;font-size:10px" (click)="setPrimary(i)">{{img.isPrimary?'✓ Primary':'Set primary'}}</button>
            </div>
          </div>
        </div>

        <div class="mt-24">
          <button class="btn btn-primary" (click)="save()" [disabled]="saving">{{saving ? 'Saving…' : 'Save changes'}}</button>
          <a routerLink="/my-properties" class="btn btn-secondary" style="margin-left:8px">Cancel</a>
        </div>
      </ng-container>
    </div>
  `
})
export class EditPropertyComponent implements OnInit {
  propertyTypes = PROPERTY_TYPES;
  allAmenities: string[] = [];
  selectedAmenities: string[] = [];
  images: ImagePreview[] = [];
  newImages: ImagePreview[] = [];
  loaded = false; saving = false; saved = false; error = '';
  propertyId = '';
  form: any = {};

  constructor(private route: ActivatedRoute, private router: Router, private svc: PropertyService) {}

  ngOnInit() {
    this.propertyId = this.route.snapshot.paramMap.get('id')!;
    this.svc.getAmenities().subscribe(a => this.allAmenities = a);
    this.svc.getById(this.propertyId).subscribe({
      next: r => {
        const p = r.property;
        this.form = {
          name: p.name, description: p.description, propertyType: p.propertyType,
          pricePerNight: p.pricePerNight, maxGuests: p.maxGuests, bedrooms: p.bedrooms,
          beds: p.beds, bathrooms: p.bathrooms, checkInTime: p.checkInTime,
          checkOutTime: p.checkOutTime, isActive: p.isActive
        };
        this.images = p.images.map(i => ({ base64Data: i.base64Data, contentType: i.contentType, isPrimary: i.isPrimary }));
        this.loaded = true;
      },
      error: () => this.error = 'Failed to load property.'
    });
  }

  isSelected(a: string) { return this.selectedAmenities.includes(a); }
  toggleAmenity(a: string) {
    const i = this.selectedAmenities.indexOf(a);
    if (i > -1) this.selectedAmenities.splice(i, 1); else this.selectedAmenities.push(a);
  }

  onFiles(e: Event) {
    const files = Array.from((e.target as HTMLInputElement).files || []);
    this.newImages = [];
    files.forEach(f => {
      const reader = new FileReader();
      reader.onload = ev => {
        const result = ev.target?.result as string;
        this.newImages.push({ base64Data: result.split(',')[1], contentType: f.type, isPrimary: this.newImages.length === 0 });
        this.images = [...this.newImages];
      };
      reader.readAsDataURL(f);
    });
  }

  removeImg(i: number) {
    this.images.splice(i, 1);
    if (this.images.length && !this.images.find(img => img.isPrimary)) this.images[0].isPrimary = true;
  }

  setPrimary(i: number) { this.images.forEach((img, idx) => img.isPrimary = idx === i); }

  save() {
    this.saving = true; this.error = ''; this.saved = false;
    const payload = { ...this.form, amenityNames: this.selectedAmenities, images: this.newImages.length ? this.images : undefined };
    this.svc.update(this.propertyId, payload).subscribe({
      next: () => { this.saved = true; this.saving = false; this.newImages = []; },
      error: e => { this.error = e.error?.description || 'Save failed'; this.saving = false; }
    });
  }
}

