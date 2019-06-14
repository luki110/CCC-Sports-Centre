using System;
using System.Collections.Generic;
using System.Text;
using CCCSportsCentreGradedUnit.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CCCSportsCentreGradedUnit.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<FitnessClass> FitnessClasses { get; set; }

        public DbSet<FitnessActivity> FitnessActivities { get; set; }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Member> Members { get; set; }

        public DbSet<Staff> Staffs { get; set; }

        public DbSet<FitnessActivityBooking> FitnessActivityBookings { get; set; }

        public DbSet<FitnessClassBooking> FitnessClassBookings { get; set; }

        public DbSet<FitnessClassCategory> FitnessClassCategories { get; set; }

        public DbSet<FitnessActivityCategory> FitnessActivityCategories { get; set; }

        public DbSet<MembershipType> MembershipTypes { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.HasSequence<int>("FitnessClassIds")
        //                .StartsAt(2)
        //                .IncrementsBy(2);

        //    modelBuilder.Entity<FitnessClass>()
        //                .Property(o => o.Id)
        //                .HasDefaultValueSql("nextval('\"FitnessClassIds\"')");
        //}


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.HasSequence<int>("FitnessActivityIds")
        //        .StartsAt(1000)
        //        .IncrementsBy(2);

        //    modelBuilder.Entity<FitnessActivity>()
        //        .Property(a => a.Id)
        //        .ValueGeneratedOnAdd();

        //    modelBuilder.HasSequence<int>("FitnessClassIds")
        //       .StartsAt(1)
        //       .IncrementsBy(2);

        //    modelBuilder.Entity<FitnessClass>()
        //        .Property(c => c.Id)
        //        .ValueGeneratedOnAdd();


        //}

    }
}
