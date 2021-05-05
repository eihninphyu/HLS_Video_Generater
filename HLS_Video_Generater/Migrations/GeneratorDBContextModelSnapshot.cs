﻿// <auto-generated />
using System;
using HLS_Video_Generater.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HLS_Video_Generater.Migrations
{
    [DbContext(typeof(GeneratorDBContext))]
    partial class GeneratorDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("HLS_Video_Generater.Model.M3U8Info", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("MediaSequence")
                        .HasColumnType("int");

                    b.Property<string>("PlaylistType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("M3U8Info");
                });

            modelBuilder.Entity("HLS_Video_Generater.Model.MediaSegments", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ByteIndex")
                        .HasColumnType("int");

                    b.Property<double>("Duration")
                        .HasColumnType("float");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("FileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("M3U8InfoId")
                        .HasColumnType("int");

                    b.Property<int>("Size")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("StreamId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("M3U8InfoId");

                    b.ToTable("MediaSegments");
                });

            modelBuilder.Entity("HLS_Video_Generater.Model.MediaSegments", b =>
                {
                    b.HasOne("HLS_Video_Generater.Model.M3U8Info", "M3U8Info")
                        .WithMany("MediaSegments")
                        .HasForeignKey("M3U8InfoId");

                    b.Navigation("M3U8Info");
                });

            modelBuilder.Entity("HLS_Video_Generater.Model.M3U8Info", b =>
                {
                    b.Navigation("MediaSegments");
                });
#pragma warning restore 612, 618
        }
    }
}
