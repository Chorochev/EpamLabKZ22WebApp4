using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EpamLabKZ22WebApp4.Models;

public partial class EpamLabTestDbContext : DbContext
{
    private string _connectionString;
    public EpamLabTestDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public EpamLabTestDbContext(string connectionString, DbContextOptions<EpamLabTestDbContext> options)
        : base(options)
    {
        _connectionString = connectionString;
    }

    public virtual DbSet<KeyValue> KeyValues { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-NCHTQ75;Database=EpamLabTestDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KeyValue>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("KeyValue");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Info)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
