﻿// <auto-generated />
using System;
using CCCSportsCentreGradedUnit.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CCCSportsCentreGradedUnit.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190610154821_All")]
    partial class All
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("City");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Country");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<string>("HouseNumber");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("PostCode");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("Street");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");

                    b.HasDiscriminator<string>("Discriminator").HasValue("ApplicationUser");
                });

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("BookingDate");

                    b.Property<string>("BookingPaymentId");

                    b.Property<double>("BookingTotal");

                    b.Property<bool>("IsPaymentConfirmed");

                    b.Property<string>("MemberId");

                    b.Property<DateTime>("PaymentDate");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.FitnessActivity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Available");

                    b.Property<int>("Duration");

                    b.Property<DateTime>("EndTime");

                    b.Property<int>("FitnessActivityCategoryId");

                    b.Property<double>("Price");

                    b.Property<int>("RoomId");

                    b.Property<DateTime>("StartDate");

                    b.Property<DateTime>("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("FitnessActivityCategoryId");

                    b.HasIndex("RoomId");

                    b.ToTable("FitnessActivities");
                });

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.FitnessActivityBooking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BookingId");

                    b.Property<int>("FitnessActivityId");

                    b.HasKey("Id");

                    b.HasIndex("BookingId");

                    b.HasIndex("FitnessActivityId");

                    b.ToTable("FitnessActivityBookings");
                });

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.FitnessActivityCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("Image");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("FitnessActivityCategories");
                });

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.FitnessClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Available");

                    b.Property<int>("Duration");

                    b.Property<DateTime>("EndTime");

                    b.Property<int>("FitnessClassCategoryId");

                    b.Property<int>("NoOfPeopleBooked");

                    b.Property<double>("Price");

                    b.Property<int>("RoomId");

                    b.Property<DateTime>("StartDate");

                    b.Property<DateTime>("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("FitnessClassCategoryId");

                    b.HasIndex("RoomId");

                    b.ToTable("FitnessClasses");
                });

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.FitnessClassBooking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BookingId");

                    b.Property<int>("FitnessClassId");

                    b.HasKey("Id");

                    b.HasIndex("BookingId");

                    b.HasIndex("FitnessClassId");

                    b.ToTable("FitnessClassBookings");
                });

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.FitnessClassCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("Image");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("FitnessClassCategories");
                });

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.MembershipType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<double>("Price");

                    b.HasKey("Id");

                    b.ToTable("MembershipTypes");
                });

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Capacity");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.Member", b =>
                {
                    b.HasBaseType("CCCSportsCentreGradedUnit.Models.ApplicationUser");

                    b.Property<int>("Age");

                    b.Property<string>("AvatarImage");

                    b.Property<DateTime>("BirthDate");

                    b.Property<bool>("CanMakeBooking");

                    b.Property<DateTime>("ExpiryDate");

                    b.Property<int>("GenderType");

                    b.Property<int>("MemberTitle");

                    b.Property<int>("MembershipTypeId");

                    b.Property<bool>("PaymentConfirmed");

                    b.Property<DateTime>("PaymentDate");

                    b.Property<string>("PaymentId");

                    b.Property<DateTime>("RegistrationDate");

                    b.HasIndex("MembershipTypeId");

                    b.ToTable("Member");

                    b.HasDiscriminator().HasValue("Member");
                });

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.Staff", b =>
                {
                    b.HasBaseType("CCCSportsCentreGradedUnit.Models.ApplicationUser");

                    b.Property<string>("CurrentQualification");

                    b.Property<string>("EmergencyContDetails");

                    b.Property<string>("EmergencyContact");

                    b.Property<string>("JobTitle");

                    b.Property<int>("RoleType");

                    b.ToTable("Staff");

                    b.HasDiscriminator().HasValue("Staff");
                });

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.Booking", b =>
                {
                    b.HasOne("CCCSportsCentreGradedUnit.Models.Member", "Member")
                        .WithMany("Bookings")
                        .HasForeignKey("MemberId");
                });

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.FitnessActivity", b =>
                {
                    b.HasOne("CCCSportsCentreGradedUnit.Models.FitnessActivityCategory", "FitnessActivityCategory")
                        .WithMany()
                        .HasForeignKey("FitnessActivityCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CCCSportsCentreGradedUnit.Models.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.FitnessActivityBooking", b =>
                {
                    b.HasOne("CCCSportsCentreGradedUnit.Models.Booking", "Booking")
                        .WithMany()
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CCCSportsCentreGradedUnit.Models.FitnessActivity", "FitnessActivity")
                        .WithMany()
                        .HasForeignKey("FitnessActivityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.FitnessClass", b =>
                {
                    b.HasOne("CCCSportsCentreGradedUnit.Models.FitnessClassCategory", "FitnessClassCategory")
                        .WithMany()
                        .HasForeignKey("FitnessClassCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CCCSportsCentreGradedUnit.Models.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.FitnessClassBooking", b =>
                {
                    b.HasOne("CCCSportsCentreGradedUnit.Models.Booking", "Booking")
                        .WithMany()
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CCCSportsCentreGradedUnit.Models.FitnessClass", "FitnessClass")
                        .WithMany()
                        .HasForeignKey("FitnessClassId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("CCCSportsCentreGradedUnit.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("CCCSportsCentreGradedUnit.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CCCSportsCentreGradedUnit.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("CCCSportsCentreGradedUnit.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CCCSportsCentreGradedUnit.Models.Member", b =>
                {
                    b.HasOne("CCCSportsCentreGradedUnit.Models.MembershipType", "MembershipType")
                        .WithMany("Members")
                        .HasForeignKey("MembershipTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
