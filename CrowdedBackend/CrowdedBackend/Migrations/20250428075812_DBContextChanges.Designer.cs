﻿// <auto-generated />
using CrowdedBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CrowdedBackend.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20250428075812_DBContextChanges")]
    partial class DBContextChanges
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CrowdedBackend.Models.DetectedDevice", b =>
                {
                    b.Property<int>("DetectedDeviceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("DetectedDeviceId"));

                    b.Property<double>("DeviceX")
                        .HasColumnType("double precision");

                    b.Property<double>("DeviceY")
                        .HasColumnType("double precision");

                    b.Property<long>("Timestamp")
                        .HasColumnType("bigint");

                    b.Property<int>("VenueID")
                        .HasColumnType("integer");

                    b.HasKey("DetectedDeviceId");

                    b.HasIndex("VenueID");

                    b.ToTable("DetectedDevice");
                });

            modelBuilder.Entity("CrowdedBackend.Models.RaspData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("MacAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("RaspId")
                        .HasColumnType("integer");

                    b.Property<int>("Rssi")
                        .HasColumnType("integer");

                    b.Property<long>("UnixTimestamp")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("RaspData");
                });

            modelBuilder.Entity("CrowdedBackend.Models.RaspberryPi", b =>
                {
                    b.Property<int>("RaspberryPiID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RaspberryPiID"));

                    b.Property<double>("RaspX")
                        .HasColumnType("double precision");

                    b.Property<double>("RaspY")
                        .HasColumnType("double precision");

                    b.Property<int>("VenueID")
                        .HasColumnType("integer");

                    b.HasKey("RaspberryPiID");

                    b.HasIndex("VenueID");

                    b.ToTable("RaspberryPi");
                });

            modelBuilder.Entity("CrowdedBackend.Models.Venue", b =>
                {
                    b.Property<int>("VenueID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("VenueID"));

                    b.Property<string>("VenueName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("VenueID");

                    b.ToTable("Venue");
                });

            modelBuilder.Entity("CrowdedBackend.Models.DetectedDevice", b =>
                {
                    b.HasOne("CrowdedBackend.Models.Venue", "Venue")
                        .WithMany("DetectedDevices")
                        .HasForeignKey("VenueID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("CrowdedBackend.Models.RaspberryPi", b =>
                {
                    b.HasOne("CrowdedBackend.Models.Venue", "Venue")
                        .WithMany("RaspberryPis")
                        .HasForeignKey("VenueID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("CrowdedBackend.Models.Venue", b =>
                {
                    b.Navigation("DetectedDevices");

                    b.Navigation("RaspberryPis");
                });
#pragma warning restore 612, 618
        }
    }
}
