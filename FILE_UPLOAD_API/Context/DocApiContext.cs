using System;
using System.Collections.Generic;
using FILE_UPLOAD_API.Models;
using Microsoft.EntityFrameworkCore;

namespace FILE_UPLOAD_API.Context;

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

    public virtual DbSet<StorageType> StorageTypes { get; set; }

    public virtual DbSet<ViewGetDocumentCategory> ViewGetDocumentCategories { get; set; }

    public virtual DbSet<ViewGetDocumentSection> ViewGetDocumentSections { get; set; }

    public virtual DbSet<ViewGetSavedFile> ViewGetSavedFiles { get; set; }

    public virtual DbSet<ViewGetStorageType> ViewGetStorageTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:XDMSConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DocumentCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Document__19093A0B53CA90BD");

            entity.Property(e => e.CategoryId).ValueGeneratedNever();
            entity.Property(e => e.CreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Section).WithMany(p => p.DocumentCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DocumentC__Secti__5BDA1BFF");
        });

        modelBuilder.Entity<DocumentSection>(entity =>
        {
            entity.HasKey(e => e.SectionId).HasName("PK__Document__80EF08722381F5F7");

            entity.Property(e => e.SectionId).ValueGeneratedNever();
            entity.Property(e => e.CreationDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<SavedFile>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK__SavedFil__6F0F98BF21757C86");

            entity.Property(e => e.CreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.DocumentCategory).WithMany(p => p.SavedFiles).HasConstraintName("FK__SavedFile__Docum__4FFE54FF");
        });

        modelBuilder.Entity<StorageType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__StorageT__516F03B563A691E2");

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
