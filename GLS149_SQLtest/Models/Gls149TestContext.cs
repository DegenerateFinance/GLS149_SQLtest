using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace GLS149_SQLtest.TestModels;

public partial class Gls149TestContext : DbContext
{
    private string _connectionString;
    private Action<string> _loggerAction;
    public Gls149TestContext(string connectionstring)
    {
        _connectionString = connectionstring;
        SetLogger(Console.WriteLine);
    }
public Gls149TestContext(DbContextOptions<Gls149TestContext> options)
        : base(options)
    {
        SetLogger(Console.WriteLine);
    }

    public virtual DbSet<GlobalTest> GlobalTests { get; set; }

    public virtual DbSet<Univerre> Univerres { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
         if (!optionsBuilder.IsConfigured)
         {
             // Set the connection string - you need to specify it here
             optionsBuilder
                .UseMySql(
                 _connectionString,
                 new MySqlServerVersion(new Version(8, 0, 36)))
                .LogTo(_loggerAction, new[] { DbLoggerCategory.Database.Command.Name, }, LogLevel.Information);

             optionsBuilder.EnableSensitiveDataLogging();
            ; // Specify the MySQL version you're using
         }
    }
    public void SetLogger(Action<string> loggerAction)
    {
        _loggerAction = loggerAction;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<GlobalTest>(entity =>
        {
            entity.HasKey(e => e.GlobalId).HasName("PRIMARY");

            entity.ToTable("global_test");

            entity.Property(e => e.GlobalId).HasColumnName("Global_id");
            entity.Property(e => e.Dt3).HasColumnType("datetime(3)");
            entity.Property(e => e.property1).HasColumnName("property1");
            entity.Property(e => e.Property2)
                .HasMaxLength(45)
                .HasColumnName("property2");
        });

        modelBuilder.Entity<Univerre>(entity =>
        {
            entity.HasKey(e => new { e.Uvrtabla, e.UvrkeyN01, e.UvrkeyN02, e.UvrkeyC01, e.UvrkeyC02 })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0, 0, 0 });

            entity
                .ToTable("univerre")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Uvrtabla)
                .HasMaxLength(12)
                .IsFixedLength()
                .HasColumnName("UVRTabla");
            entity.Property(e => e.UvrkeyN01).HasColumnName("UVRKeyN01");
            entity.Property(e => e.UvrkeyN02).HasColumnName("UVRKeyN02");
            entity.Property(e => e.UvrkeyC01)
                .HasMaxLength(30)
                .HasDefaultValueSql("''")
                .IsFixedLength()
                .HasColumnName("UVRKeyC01");
            entity.Property(e => e.UvrkeyC02)
                .HasMaxLength(30)
                .HasDefaultValueSql("''")
                .IsFixedLength()
                .HasColumnName("UVRKeyC02");
            entity.Property(e => e.Uvrbar01)
                .HasPrecision(10)
                .HasColumnName("UVRBar01");
            entity.Property(e => e.Uvrbar02)
                .HasPrecision(10)
                .HasColumnName("UVRBar02");
            entity.Property(e => e.Uvrdt01)
                .HasColumnType("datetime")
                .HasColumnName("UVRDT01");
            entity.Property(e => e.Uvrdt02)
                .HasColumnType("datetime")
                .HasColumnName("UVRDT02");
            entity.Property(e => e.Uvrfec01)
                .HasColumnType("datetime")
                .HasColumnName("UVRFec01");
            entity.Property(e => e.Uvrfec02)
                .HasColumnType("datetime")
                .HasColumnName("UVRFec02");
            entity.Property(e => e.Uvrnum01).HasColumnName("UVRNum01");
            entity.Property(e => e.Uvrnum02).HasColumnName("UVRNum02");
            entity.Property(e => e.Uvrtxt01)
                .HasMaxLength(100)
                .HasColumnName("UVRTxt01");
            entity.Property(e => e.Uvrtxt02)
                .HasMaxLength(100)
                .HasColumnName("UVRTxt02");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
