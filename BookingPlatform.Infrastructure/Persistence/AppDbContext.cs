using BookingPlatform.Domain.Entities;
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
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Role>().HasData(
        new Role
        {
            Id = new Guid("11111111-1111-1111-1111-111111111111"),
            Name = "Guest",
            Description = "Default platform user",
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow
        },
        new Role
        {
            Id = new Guid("22222222-2222-2222-2222-222222222222"),
            Name = "Owner",
            Description = "Property owner",
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow,
        },
        new Role
        {
            Id = new Guid("33333333-3333-3333-3333-333333333333"),
            Name = "Admin",
            Description = "Platform administrator",
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow,
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
    }

}

