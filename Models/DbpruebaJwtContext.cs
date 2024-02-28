using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PruebaJWT.Models;

public partial class DbpruebaJwtContext : DbContext
{
    public DbpruebaJwtContext()
    {
    }

    public DbpruebaJwtContext(DbContextOptions<DbpruebaJwtContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__USUARIO__5B65BF976163571A");

            entity.ToTable("USUARIO");

            entity.Property(e => e.Clave)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
