using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public partial class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alert> Alerts { get; set; }

    public virtual DbSet<Telemetry> Telemetries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("alerts_pkey");

            entity.ToTable("alerts", "windmillinspection");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Farmid).HasColumnName("farmid");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Severity).HasColumnName("severity");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.Property(e => e.Turbineid).HasColumnName("turbineid");
        });

        modelBuilder.Entity<Telemetry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("telemetry_pkey");

            entity.ToTable("telemetry", "windmillinspection");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ambienttemperature).HasColumnName("ambienttemperature");
            entity.Property(e => e.Bladepitch).HasColumnName("bladepitch");
            entity.Property(e => e.Farmid).HasColumnName("farmid");
            entity.Property(e => e.Gearboxtemp).HasColumnName("gearboxtemp");
            entity.Property(e => e.Generatortemp).HasColumnName("generatortemp");
            entity.Property(e => e.Nacelledirection).HasColumnName("nacelledirection");
            entity.Property(e => e.Poweroutput).HasColumnName("poweroutput");
            entity.Property(e => e.Rotorspeed).HasColumnName("rotorspeed");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.Property(e => e.Turbineid).HasColumnName("turbineid");
            entity.Property(e => e.Turbinename).HasColumnName("turbinename");
            entity.Property(e => e.Vibration).HasColumnName("vibration");
            entity.Property(e => e.Winddirection).HasColumnName("winddirection");
            entity.Property(e => e.Windspeed).HasColumnName("windspeed");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
