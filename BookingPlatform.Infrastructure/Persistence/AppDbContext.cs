using BookingPlatform.Domain.Entities;
using BookingPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;



namespace BookingPlatform.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<OwnerProfile> OwnerProfiles => Set<OwnerProfile>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<PropertyAmenity> PropertyAmenities => Set<PropertyAmenity>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Use fixed timestamps for seeded roles so model is deterministic for EF migrations
        builder.Entity<Role>().HasData(
        new Role
        {
            Id = new Guid("11111111-1111-1111-1111-111111111111"),
            Name = "Guest",
            Description = "Default platform user",
            CreatedAt = new DateTime(2026, 3, 6, 20, 11, 30, 63, DateTimeKind.Utc).AddTicks(5115),
            LastModifiedAt = new DateTime(2026, 3, 6, 20, 11, 30, 63, DateTimeKind.Utc).AddTicks(5243)
        },
        new Role
        {
            Id = new Guid("22222222-2222-2222-2222-222222222222"),
            Name = "Owner",
            Description = "Property owner",
            CreatedAt = new DateTime(2026, 3, 6, 20, 11, 30, 63, DateTimeKind.Utc).AddTicks(5481),
            LastModifiedAt = new DateTime(2026, 3, 6, 20, 11, 30, 63, DateTimeKind.Utc).AddTicks(5482),
        },
        new Role
        {
            Id = new Guid("33333333-3333-3333-3333-333333333333"),
            Name = "Admin",
            Description = "Platform administrator",
            CreatedAt = new DateTime(2026, 3, 6, 20, 11, 30, 63, DateTimeKind.Utc).AddTicks(5483),
            LastModifiedAt = new DateTime(2026, 3, 6, 20, 11, 30, 63, DateTimeKind.Utc).AddTicks(5483),
        }
    );

        builder.Entity<UserRole>()
            .HasKey(x => new { x.UserId, x.RoleId });

        builder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();

        builder.Entity<OwnerProfile>()
            .HasKey(x => x.UserId);

        builder.Entity<OwnerProfile>()
            .HasOne(x => x.User)
            .WithOne(x => x.OwnerProfile)
            .HasForeignKey<OwnerProfile>(x => x.UserId);

        builder.Entity<Booking>()
       .HasOne(b => b.Guest)
       .WithMany()
       .HasForeignKey(b => b.GuestId)
       .OnDelete(DeleteBehavior.Restrict);  // NO cascade

        // Bookings → Properties (PropertyId)
        builder.Entity<Booking>()
            .HasOne(b => b.Property)
            .WithMany(p => p.Bookings)
            .HasForeignKey(b => b.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Booking>()
        .HasOne(b => b.Guest)
        .WithMany()
        .HasForeignKey(b => b.GuestId)
        .OnDelete(DeleteBehavior.Restrict); // no cascade

        // Reviews → Users
        builder.Entity<Review>()
            .HasOne(r => r.Guest)
            .WithMany()
            .HasForeignKey(r => r.GuestId)
            .OnDelete(DeleteBehavior.Restrict); // no cascade

        // Bookings → Properties (keep cascade)
        builder.Entity<Booking>()
            .HasOne(b => b.Property)
            .WithMany(p => p.Bookings)
            .HasForeignKey(b => b.PropertyId)
            .OnDelete(DeleteBehavior.Cascade); // OK

        builder.Entity<Review>()
    .HasOne(r => r.Property)
    .WithMany(p => p.Reviews)
    .HasForeignKey(r => r.PropertyId)
    .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Booking>(b =>
        {
            b.Property(x => x.AmenitiesUpCharge).HasPrecision(18, 2);
            b.Property(x => x.CleaningFee).HasPrecision(18, 2);
            b.Property(x => x.PriceForPeriod).HasPrecision(18, 2);
            b.Property(x => x.TotalPrice).HasPrecision(18, 2);
        });

        // Ensure price precision for properties to avoid silent truncation
        builder.Entity<Property>(p =>
        {
            p.Property(x => x.PricePerNight).HasPrecision(18, 2);
        });

        // Amenities and many-to-many mapping
        builder.Entity<Amenity>(a =>
        {
            a.HasKey(x => x.Id);
            a.Property(x => x.Name).HasConversion<int>().IsRequired();

            a.HasData(
                new Amenity { Id = new Guid("00000000-0000-0000-0000-000000000001"), Name = AmenitiesEnum.WiFi, CreatedAt = new DateTime(2026,3,6,20,11,30, DateTimeKind.Utc) },
                new Amenity { Id = new Guid("00000000-0000-0000-0000-000000000002"), Name = AmenitiesEnum.AirConditioning, CreatedAt = new DateTime(2026,3,6,20,11,30, DateTimeKind.Utc) },
                new Amenity { Id = new Guid("00000000-0000-0000-0000-000000000003"), Name = AmenitiesEnum.Parking, CreatedAt = new DateTime(2026,3,6,20,11,30, DateTimeKind.Utc) },
                new Amenity { Id = new Guid("00000000-0000-0000-0000-000000000004"), Name = AmenitiesEnum.Kitchen, CreatedAt = new DateTime(2026,3,6,20,11,30, DateTimeKind.Utc) },
                new Amenity { Id = new Guid("00000000-0000-0000-0000-000000000005"), Name = AmenitiesEnum.TV, CreatedAt = new DateTime(2026,3,6,20,11,30, DateTimeKind.Utc) },
                new Amenity { Id = new Guid("00000000-0000-0000-0000-000000000006"), Name = AmenitiesEnum.WashingMachine, CreatedAt = new DateTime(2026,3,6,20,11,30, DateTimeKind.Utc) },
                new Amenity { Id = new Guid("00000000-0000-0000-0000-000000000007"), Name = AmenitiesEnum.Balcony, CreatedAt = new DateTime(2026,3,6,20,11,30, DateTimeKind.Utc) },
                new Amenity { Id = new Guid("00000000-0000-0000-0000-000000000008"), Name = AmenitiesEnum.SeaView, CreatedAt = new DateTime(2026,3,6,20,11,30, DateTimeKind.Utc) },
                new Amenity { Id = new Guid("00000000-0000-0000-0000-000000000009"), Name = AmenitiesEnum.Pool, CreatedAt = new DateTime(2026,3,6,20,11,30, DateTimeKind.Utc) },
                new Amenity { Id = new Guid("00000000-0000-0000-0000-00000000000a"), Name = AmenitiesEnum.Breakfast, CreatedAt = new DateTime(2026,3,6,20,11,30, DateTimeKind.Utc) },
                new Amenity { Id = new Guid("00000000-0000-0000-0000-00000000000b"), Name = AmenitiesEnum.PetsAllowed, CreatedAt = new DateTime(2026,3,6,20,11,30, DateTimeKind.Utc) }
            );
        });

        builder.Entity<PropertyAmenity>(pa =>
        {
            pa.HasKey(x => new { x.PropertyId, x.AmenityId });
            pa.HasOne(x => x.Property).WithMany(p => p.PropertyAmenities).HasForeignKey(x => x.PropertyId);
            pa.HasOne(x => x.Amenity).WithMany(a => a.PropertyAmenities).HasForeignKey(x => x.AmenityId);
        });
    }

}

