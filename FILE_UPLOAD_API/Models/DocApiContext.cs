﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FILE_UPLOAD_API.Models;

public partial class DocApiContext : DbContext
{
    public DocApiContext()
    {
    }

    public DocApiContext(DbContextOptions<DocApiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DocumentCategory> DocumentCategories { get; set; }

    public virtual DbSet<DocumentSection> DocumentSections { get; set; }

    public virtual DbSet<SavedFile> SavedFiles { get; set; }

    public virtual DbSet<ServicesLog> ServicesLogs { get; set; }

    public virtual DbSet<StorageType> StorageTypes { get; set; }

    public virtual DbSet<ViewGetDocumentCategory> ViewGetDocumentCategories { get; set; }

    public virtual DbSet<ViewGetDocumentSection> ViewGetDocumentSections { get; set; }

    public virtual DbSet<ViewGetSavedFile> ViewGetSavedFiles { get; set; }

    public virtual DbSet<ViewGetStorageType> ViewGetStorageTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DBConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DocumentCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Document__19093A0B97864180");

            entity.Property(e => e.CategoryId).ValueGeneratedNever();
            entity.Property(e => e.CreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Section).WithMany(p => p.DocumentCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DocumentC__Secti__48CFD27E");
        });

        modelBuilder.Entity<DocumentSection>(entity =>
        {
            entity.HasKey(e => e.SectionId).HasName("PK__Document__80EF0872E47F3741");

            entity.Property(e => e.SectionId).ValueGeneratedNever();
            entity.Property(e => e.CreationDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<SavedFile>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK__SavedFil__6F0F98BFD66AE85F");

            entity.Property(e => e.CreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.DocumentCategory).WithMany(p => p.SavedFiles).HasConstraintName("FK__SavedFile__Docum__49C3F6B7");
        });

        modelBuilder.Entity<ServicesLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Services__5E548648018C8685");

            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<StorageType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__StorageT__516F03B5885D0B28");

            entity.Property(e => e.TypeId).ValueGeneratedNever();
            entity.Property(e => e.CreationDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<ViewGetDocumentCategory>(entity =>
        {
            entity.ToView("View_GetDocumentCategories", "XDMS");
        });

        modelBuilder.Entity<ViewGetDocumentSection>(entity =>
        {
            entity.ToView("View_GetDocumentSections", "XDMS");
        });

        modelBuilder.Entity<ViewGetSavedFile>(entity =>
        {
            entity.ToView("View_GetSavedFiles", "XDMS");
        });

        modelBuilder.Entity<ViewGetStorageType>(entity =>
        {
            entity.ToView("View_GetStorageTypes", "XDMS");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
