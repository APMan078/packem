﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Packem.Data;

#nullable disable

namespace Packem.WebApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20220613192825_RemoveStateColumnAtCustomerLocation")]
    partial class RemoveStateColumnAtCustomerLocation
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Packem.Domain.Entities.Customer", b =>
                {
                    b.Property<int>("CustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CustomerId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.HasKey("CustomerId");

                    b.ToTable("Customers", (string)null);
                });

            modelBuilder.Entity("Packem.Domain.Entities.CustomerDevice", b =>
                {
                    b.Property<int>("CustomerDeviceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CustomerDeviceId"));

                    b.Property<DateTime>("AddedDateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("CustomerLocationId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("DeactivedDateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastLoginDateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("SerialNumber")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.HasKey("CustomerDeviceId");

                    b.HasIndex("CustomerLocationId");

                    b.ToTable("CustomerDevices", (string)null);
                });

            modelBuilder.Entity("Packem.Domain.Entities.CustomerDeviceToken", b =>
                {
                    b.Property<int>("CustomerDeviceTokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CustomerDeviceTokenId"));

                    b.Property<DateTime>("AddedDateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("CustomerDeviceId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("DeactivedDateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("DeviceToken")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsValidated")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("ValidatedDateTime")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("CustomerDeviceTokenId");

                    b.HasIndex("CustomerDeviceId");

                    b.ToTable("CustomerDeviceTokens", (string)null);
                });

            modelBuilder.Entity("Packem.Domain.Entities.CustomerFacility", b =>
                {
                    b.Property<int>("CustomerFacilityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CustomerFacilityId"));

                    b.Property<int?>("CustomerLocationId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.HasKey("CustomerFacilityId");

                    b.HasIndex("CustomerLocationId");

                    b.ToTable("CustomerFacilities", (string)null);
                });

            modelBuilder.Entity("Packem.Domain.Entities.CustomerLocation", b =>
                {
                    b.Property<int>("CustomerLocationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CustomerLocationId"));

                    b.Property<int?>("CustomerId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.HasKey("CustomerLocationId");

                    b.HasIndex("CustomerId");

                    b.ToTable("CustomerLocations", (string)null);
                });

            modelBuilder.Entity("Packem.Domain.Entities.ErrorLog", b =>
                {
                    b.Property<int>("ErrorLogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ErrorLogId"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StackTrace")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("ErrorLogId");

                    b.HasIndex("UserId");

                    b.ToTable("ErrorLogs", (string)null);
                });

            modelBuilder.Entity("Packem.Domain.Entities.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RoleId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("RoleId");

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("Packem.Domain.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserId"));

                    b.Property<int?>("CustomerId")
                        .HasColumnType("integer");

                    b.Property<int?>("CustomerLocationId")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<int?>("RoleId")
                        .HasColumnType("integer");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("UserId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("CustomerLocationId");

                    b.HasIndex("RoleId");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("Packem.Domain.Entities.CustomerDevice", b =>
                {
                    b.HasOne("Packem.Domain.Entities.CustomerLocation", "CustomerLocation")
                        .WithMany("CustomerDevices")
                        .HasForeignKey("CustomerLocationId")
                        .HasConstraintName("FK_CustomerDevices_CustomerLocations");

                    b.Navigation("CustomerLocation");
                });

            modelBuilder.Entity("Packem.Domain.Entities.CustomerDeviceToken", b =>
                {
                    b.HasOne("Packem.Domain.Entities.CustomerDevice", "CustomerDevice")
                        .WithMany("CustomerDeviceTokens")
                        .HasForeignKey("CustomerDeviceId")
                        .HasConstraintName("FK_CustomerDeviceTokens_CustomerDevices");

                    b.Navigation("CustomerDevice");
                });

            modelBuilder.Entity("Packem.Domain.Entities.CustomerFacility", b =>
                {
                    b.HasOne("Packem.Domain.Entities.CustomerLocation", "CustomerLocation")
                        .WithMany("CustomerFacilities")
                        .HasForeignKey("CustomerLocationId")
                        .HasConstraintName("FK_CustomerFacilities_CustomerLocations");

                    b.Navigation("CustomerLocation");
                });

            modelBuilder.Entity("Packem.Domain.Entities.CustomerLocation", b =>
                {
                    b.HasOne("Packem.Domain.Entities.Customer", "Customer")
                        .WithMany("CustomerLocations")
                        .HasForeignKey("CustomerId")
                        .HasConstraintName("FK_CustomerLocations_Customers");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Packem.Domain.Entities.ErrorLog", b =>
                {
                    b.HasOne("Packem.Domain.Entities.User", "User")
                        .WithMany("ErrorLogs")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_ErrorLogs_Users");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Packem.Domain.Entities.User", b =>
                {
                    b.HasOne("Packem.Domain.Entities.Customer", "Customer")
                        .WithMany("Users")
                        .HasForeignKey("CustomerId")
                        .HasConstraintName("FK_Users_Customers");

                    b.HasOne("Packem.Domain.Entities.CustomerLocation", "CustomerLocation")
                        .WithMany("Users")
                        .HasForeignKey("CustomerLocationId")
                        .HasConstraintName("FK_Users_CustomerLocations");

                    b.HasOne("Packem.Domain.Entities.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_Users_Roles");

                    b.Navigation("Customer");

                    b.Navigation("CustomerLocation");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Packem.Domain.Entities.Customer", b =>
                {
                    b.Navigation("CustomerLocations");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Packem.Domain.Entities.CustomerDevice", b =>
                {
                    b.Navigation("CustomerDeviceTokens");
                });

            modelBuilder.Entity("Packem.Domain.Entities.CustomerLocation", b =>
                {
                    b.Navigation("CustomerDevices");

                    b.Navigation("CustomerFacilities");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Packem.Domain.Entities.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Packem.Domain.Entities.User", b =>
                {
                    b.Navigation("ErrorLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
