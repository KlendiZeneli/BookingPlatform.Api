// ─── Auth ─────────────────────────────────────────────────────────────────────
export interface LoginRequest { email: string; password: string; }
export interface LoginResponse { token: string; }
export interface RegisterRequest { firstName: string; lastName: string; email: string; password: string; }

// ─── User ─────────────────────────────────────────────────────────────────────
export interface UserProfile {
  id: string; firstName: string; lastName: string; email: string;
  phoneNumber?: string; profileImageUrl?: string; isActive: boolean; roles: string[];
}

// ─── Property ─────────────────────────────────────────────────────────────────
export interface PropertyImage { id: string; base64Data: string; contentType: string; isPrimary: boolean; }

export interface PropertyAddress { country: string; city: string; street: string; postalCode: string; }

export interface PropertyListItem {
  id: string; name: string; city: string; country: string;
  pricePerNight: number; maxGuests: number; bedrooms: number; bathrooms: number;
  averageRating: number; reviewCount: number;
  primaryImageBase64?: string; primaryImageContentType?: string; propertyType: string;
}

export interface PropertyDetail {
  id: string; ownerProfileId: string; name: string; description: string;
  propertyType: number; addressId: string; address?: PropertyAddress;
  maxGuests: number; bedrooms: number; beds: number; bathrooms: number;
  pricePerNight: number; checkInTime: string; checkOutTime: string;
  isActive: boolean; reviewCount: number; averageRating: number;
  lastBookedOnUtc?: string; bookings: BookingInfo[]; images: PropertyImage[];
}

export interface PagedResponse<T> {
  items: T[]; totalCount: number; page: number; pageSize: number;
  totalPages: number; hasNextPage: boolean; hasPreviousPage: boolean;
}

export interface MyProperty {
  id: string; name: string; description: string; propertyType: string;
  city: string; country: string; pricePerNight: number; maxGuests: number;
  bedrooms: number; beds: number; bathrooms: number;
  averageRating: number; reviewCount: number; isActive: boolean;
  pendingBookingsCount: number; confirmedBookingsCount: number;
}

export interface Availability { isAvailable: boolean; blockedPeriods: { startDate: string; endDate: string }[]; }

// ─── Booking ──────────────────────────────────────────────────────────────────
export interface BookingInfo {
  id: string; propertyId: string; propertyName: string;
  propertyCity: string; propertyCountry: string;
  startDate: string; endDate: string; guestCount: number; totalPrice: number;
  bookingStatus: string; createdOnUtc?: string; confirmedOnUtc?: string;
  cancelledOnUtc?: string; hasReview: boolean;
}

export interface BookingDetail {
  id: string; propertyId: string; propertyName: string;
  propertyCity: string; propertyCountry: string; pricePerNight: number;
  guestId: string; guestName: string; startDate: string; endDate: string;
  guestCount: number; cleaningFee: number; amenitiesUpCharge: number;
  priceForPeriod: number; totalPrice: number; bookingStatus: string;
  createdOnUtc?: string; confirmedOnUtc?: string; rejectedOnUtc?: string;
  completedOnUtc?: string; cancelledOnUtc?: string; hasReview: boolean;
}

// ─── Review ───────────────────────────────────────────────────────────────────
export interface Review { id: string; bookingId: string; guestId: string; guestName: string; rating: number; comment?: string; }
export interface ReviewCheck { reviewId?: string; hasReview: boolean; rating?: number; comment?: string; }

// ─── Owner Profile ────────────────────────────────────────────────────────────
export interface OwnerProfile {
  userId: string; identityCardNumber: string; verificationStatus: string;
  verifiedAt?: string; verificationNotes?: string; businessName?: string; createdAt: string;
}

// ─── Shared ───────────────────────────────────────────────────────────────────
export const PROPERTY_TYPES = [
  { value: 0, label: 'Apartment' }, { value: 1, label: 'House' },
  { value: 2, label: 'Villa' },     { value: 3, label: 'Studio' },
  { value: 4, label: 'Cabin' }
];

export const SORT_OPTIONS = [
  { value: 'Relevance', label: 'Relevance' }, { value: 'PriceAsc', label: 'Price ↑' },
  { value: 'PriceDesc', label: 'Price ↓' },   { value: 'Rating', label: 'Top Rated' },
  { value: 'Newest', label: 'Newest' }
];

// ─── Notifications ───────────────────────────────────────────────────────────
export interface Notification {
  id: string;
  title: string;
  message?: string;
  createdOnUtc: string;
  isRead: boolean;
}
