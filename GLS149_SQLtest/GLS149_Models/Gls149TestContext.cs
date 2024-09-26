using System;
using System.Collections.Generic;
using System.Configuration;
using MyCompiledModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql;
using System.IO;

namespace GLS149_SQLtest.Models;

public partial class Gls149TestContext : DbContext
{
    private string _connectionString;
    private ConectionTypeEnum _conectionTypeEnum;
    public enum ConectionTypeEnum 
    { 
        MySQL = 0, 
        SQLServer = 1 
    }
    public Gls149TestContext(DbContextOptions<Gls149TestContext> options)
        : base(options)
    {
    }
    public void SetConnectionType(string connectionstring, ConectionTypeEnum conectionTypeEnum)
    {
        _connectionString = connectionstring;
        _conectionTypeEnum = conectionTypeEnum;
    }
    public virtual DbSet<Univerre> Univerres { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Univerre>(entity =>
        {
            entity.HasKey(e => new { e.UVRTabla, e.UVRKeyN01, e.UvrkeyN02, e.UvrkeyC01, e.UvrkeyC02 }).HasName("PRIMARY");

            entity.ToTable("univerre");

            entity.Property(e => e.UVRTabla)
                .HasMaxLength(12)
                .IsFixedLength()
                .HasColumnName("UVRTabla");
            entity.Property(e => e.UVRKeyN01).HasColumnName("UVRKeyN01");
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
            entity.Property(e => e.UVRBar01)
                .HasPrecision(10)
                .HasColumnName("UVRBar01");
            entity.Property(e => e.UVRBar02)
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
            entity.Property(e => e.UVRNum01).HasColumnName("UVRNum01");
            entity.Property(e => e.UVRNum02).HasColumnName("UVRNum02");
            entity.Property(e => e.UVRTxt01)
                .HasMaxLength(100)
                .HasColumnName("UVRTxt01");
            entity.Property(e => e.Uvrtxt02)
                .HasMaxLength(100)
                .HasColumnName("UVRTxt02");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableServiceProviderCaching(false);
        //optionsBuilder.UseModel(Gls149TestContextModel.Instance);
        if (_conectionTypeEnum == ConectionTypeEnum.MySQL)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Set the connection string - you need to specify it here
                optionsBuilder.UseMySql(
                    _connectionString,
                    new MySqlServerVersion(new Version(8, 0, 36))); // Specify the MySQL version you're using
            }
        }
        if (_conectionTypeEnum == ConectionTypeEnum.SQLServer)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}



public class Gls149TestContextFactory : IDesignTimeDbContextFactory<Gls149TestContext>
{
    public Gls149TestContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<Gls149TestContext>();

        // Build configuration from appsettings.json
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        if (config == null)
        {
            throw new Exception("appsettings.json not found");
        }
        // Get the connection string
        string? connectionString = config.GetConnectionString("MySqlConnection2");
        if (connectionString == null)
        {
            throw new Exception("Connection string not found in appsettings.json");
        }

        // Specify your connection string here
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)));

        return new Gls149TestContext(optionsBuilder.Options);
    }
}


