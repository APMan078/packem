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
    [Migration("20220616185203_AddTables_Items_Vendors_Locations_Bins_PurchaseOrders_Receipts")]
    partial class AddTables_Items_Vendors_Locations_Bins_PurchaseOrders_Receipts
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Packem.Domain.Entities.Bin", b =>
                {
                    b.Property<int>("BinId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("BinId"));

                    b.Property<int?>("CustomerLocationId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.HasKey("BinId");

                    b.HasIndex("CustomerLocationId");

                    b.ToTable("Bins", (string)null);
                });

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

            modelBuilder.Entity("Packem.Domain.Entities.Item", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ItemId"));

                    b.Property<int?>("BinId")
                        .HasColumnType("integer");

                    b.Property<int?>("CustomerLocationId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(550)
                        .HasColumnType("character varying(550)");

                    b.Property<int?>("ItemNo")
                        .HasColumnType("integer");

                    b.Property<int?>("LocationId")
                        .HasColumnType("integer");

                    b.Property<int>("QtyOnHand")
                        .HasColumnType("integer");

                    b.Property<string>("UOM")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<int?>("VendorId")
                        .HasColumnType("integer");

                    b.HasKey("ItemId");

                    b.HasIndex("BinId");

                    b.HasIndex("CustomerLocationId");

                    b.HasIndex("LocationId");

                    b.HasIndex("VendorId");

                    b.ToTable("Items", (string)null);
                });

            modelBuilder.Entity("Packem.Domain.Entities.Location", b =>
                {
                    b.Property<int>("LocationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("LocationId"));

                    b.Property<int?>("CustomerLocationId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.HasKey("LocationId");

                    b.HasIndex("CustomerLocationId");

                    b.ToTable("Locations", (string)null);
                });

            modelBuilder.Entity("Packem.Domain.Entities.PurchaseOrder", b =>
                {
                    b.Property<int>("PurchaseOrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PurchaseOrderId"));

                    b.Property<int?>("CustomerLocationId")
                        .HasColumnType("integer");

                    b.Property<int?>("ItemId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("OrderQty")
                        .HasColumnType("integer");

                    b.Property<int?>("PurchaseOrderNo")
                        .HasColumnType("integer");

                    b.Property<int?>("ReceiptId")
                        .HasColumnType("integer");

                    b.Property<int>("Remaining")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedDateTime")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("PurchaseOrderId");

                    b.HasIndex("CustomerLocationId");

                    b.HasIndex("ItemId");

                    b.HasIndex("ReceiptId");

                    b.ToTable("PurchaseOrders", (string)null);
                });

            modelBuilder.Entity("Packem.Domain.Entities.Receipt", b =>
                {
                    b.Property<int>("ReceiptId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ReceiptId"));

                    b.Property<int?>("CustomerLocationId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(550)
                        .HasColumnType("character varying(550)");

                    b.Property<int>("QtyReceived")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedDateTime")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("ReceiptId");

                    b.HasIndex("CustomerLocationId");

                    b.ToTable("Receipts", (string)null);
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

            modelBuilder.Entity("Packem.Domain.Entities.Vendor", b =>
                {
                    b.Property<int>("VendorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("VendorId"));

                    b.Property<int?>("CustomerLocationId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<int?>("VendorNo")
                        .HasColumnType("integer");

                    b.HasKey("VendorId");

                    b.HasIndex("CustomerLocationId");

                    b.ToTable("Vendors", (string)null);
                });

            modelBuilder.Entity("Packem.Domain.Entities.Bin", b =>
                {
                    b.HasOne("Packem.Domain.Entities.CustomerLocation", "CustomerLocation")
                        .WithMany("Bins")
                        .HasForeignKey("CustomerLocationId")
                        .HasConstraintName("FK_Bins_CustomerLocations");

                    b.Navigation("CustomerLocation");
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

            modelBuilder.Entity("Packem.Domain.Entities.Item", b =>
                {
                    b.HasOne("Packem.Domain.Entities.Bin", "Bin")
                        .WithMany("Items")
                        .HasForeignKey("BinId")
                        .HasConstraintName("FK_Items_Bins");

                    b.HasOne("Packem.Domain.Entities.CustomerLocation", "CustomerLocation")
                        .WithMany("Items")
                        .HasForeignKey("CustomerLocationId")
                        .HasConstraintName("FK_Items_CustomerLocations");

                    b.HasOne("Packem.Domain.Entities.Location", "Location")
                        .WithMany("Items")
                        .HasForeignKey("LocationId")
                        .HasConstraintName("FK_Items_Locations");

                    b.HasOne("Packem.Domain.Entities.Vendor", "Vendor")
                        .WithMany("Items")
                        .HasForeignKey("VendorId")
                        .HasConstraintName("FK_Items_Vendors");

                    b.Navigation("Bin");

                    b.Navigation("CustomerLocation");

                    b.Navigation("Location");

                    b.Navigation("Vendor");
                });

            modelBuilder.Entity("Packem.Domain.Entities.Location", b =>
                {
                    b.HasOne("Packem.Domain.Entities.CustomerLocation", "CustomerLocation")
                        .WithMany("Locations")
                        .HasForeignKey("CustomerLocationId")
                        .HasConstraintName("FK_Locations_CustomerLocations");

                    b.Navigation("CustomerLocation");
                });

            modelBuilder.Entity("Packem.Domain.Entities.PurchaseOrder", b =>
                {
                    b.HasOne("Packem.Domain.Entities.CustomerLocation", "CustomerLocation")
                        .WithMany("PurchaseOrders")
                        .HasForeignKey("CustomerLocationId")
                        .HasConstraintName("FK_PurchaseOrders_CustomerLocations");

                    b.HasOne("Packem.Domain.Entities.Item", "Item")
                        .WithMany("PurchaseOrders")
                        .HasForeignKey("ItemId")
                        .HasConstraintName("FK_PurchaseOrders_Items");

                    b.HasOne("Packem.Domain.Entities.Receipt", "Receipt")
                        .WithMany("PurchaseOrders")
                        .HasForeignKey("ReceiptId")
                        .HasConstraintName("FK_PurchaseOrders_Receipts");

                    b.Navigation("CustomerLocation");

                    b.Navigation("Item");

                    b.Navigation("Receipt");
                });

            modelBuilder.Entity("Packem.Domain.Entities.Receipt", b =>
                {
                    b.HasOne("Packem.Domain.Entities.CustomerLocation", "CustomerLocation")
                        .WithMany("Receipts")
                        .HasForeignKey("CustomerLocationId")
                        .HasConstraintName("FK_Receipts_CustomerLocations");

                    b.Navigation("CustomerLocation");
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

            modelBuilder.Entity("Packem.Domain.Entities.Vendor", b =>
                {
                    b.HasOne("Packem.Domain.Entities.CustomerLocation", "CustomerLocation")
                        .WithMany("Vendors")
                        .HasForeignKey("CustomerLocationId")
                        .HasConstraintName("FK_Vendors_CustomerLocations");

                    b.Navigation("CustomerLocation");
                });

            modelBuilder.Entity("Packem.Domain.Entities.Bin", b =>
                {
                    b.Navigation("Items");
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
                    b.Navigation("Bins");

                    b.Navigation("CustomerDevices");

                    b.Navigation("CustomerFacilities");

                    b.Navigation("Items");

                    b.Navigation("Locations");

                    b.Navigation("PurchaseOrders");

                    b.Navigation("Receipts");

                    b.Navigation("Users");

                    b.Navigation("Vendors");
                });

            modelBuilder.Entity("Packem.Domain.Entities.Item", b =>
                {
                    b.Navigation("PurchaseOrders");
                });

            modelBuilder.Entity("Packem.Domain.Entities.Location", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("Packem.Domain.Entities.Receipt", b =>
                {
                    b.Navigation("PurchaseOrders");
                });

            modelBuilder.Entity("Packem.Domain.Entities.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Packem.Domain.Entities.User", b =>
                {
                    b.Navigation("ErrorLogs");
                });

            modelBuilder.Entity("Packem.Domain.Entities.Vendor", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
