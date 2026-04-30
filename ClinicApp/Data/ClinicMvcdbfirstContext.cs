using System;
using System.Collections.Generic;
using ClinicApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Data;

public partial class ClinicMvcdbfirstContext : DbContext
{
    public ClinicMvcdbfirstContext()
    {
    }

    public ClinicMvcdbfirstContext(DbContextOptions<ClinicMvcdbfirstContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Capability> Capabilities { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<MedicalProgram> MedicalPrograms { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DevConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Greek_100_CI_AI");

        modelBuilder.Entity<Capability>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_Capabilities_Name").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasIndex(e => e.Specialty, "IX_Doctors_Specialty");

            entity.HasIndex(e => e.UserId, "IX_Doctors_UserId").IsUnique();

            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Specialty).HasMaxLength(100);

            entity.HasOne(d => d.User).WithOne(p => p.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Doctors_Users");
        });

        modelBuilder.Entity<MedicalProgram>(entity =>
        {
            entity.HasIndex(e => e.Description, "IX_MedicalPrograms_Description");

            entity.HasIndex(e => e.DoctorId, "IX_MedicalPrograms_DoctorId");

            entity.Property(e => e.Description).HasMaxLength(50);

            entity.HasOne(d => d.Doctor).WithMany(p => p.MedicalPrograms)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK_MedicalPrograms_Doctors");

            entity.HasMany(d => d.Patients).WithMany(p => p.Programs)
                .UsingEntity<Dictionary<string, object>>(
                    "ProgramsPatient",
                    r => r.HasOne<Patient>().WithMany()
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ProgramsPatients_Patients"),
                    l => l.HasOne<MedicalProgram>().WithMany()
                        .HasForeignKey("ProgramId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ProgramsPatients_MedicalPrograms"),
                    j =>
                    {
                        j.HasKey("ProgramId", "PatientId");
                        j.ToTable("ProgramsPatients");
                        j.HasIndex(new[] { "PatientId" }, "IX_ProgramsPatients_PatientId");
                        j.HasIndex(new[] { "ProgramId" }, "IX_ProgramsPatients_ProgramId");
                    });
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasIndex(e => e.Amka, "IX_Patients_AMKA").IsUnique();

            entity.HasIndex(e => e.BloodType, "IX_Patients_BloodType");

            entity.HasIndex(e => e.UserId, "IX_Patients_UserId").IsUnique();

            entity.Property(e => e.Amka)
                .HasMaxLength(11)
                .HasColumnName("AMKA");
            entity.Property(e => e.BloodType).HasMaxLength(5);

            entity.HasOne(d => d.User).WithOne(p => p.Patient)
                .HasForeignKey<Patient>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patients_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_Roles_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasMany(d => d.Capabilities).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolesCapability",
                    r => r.HasOne<Capability>().WithMany()
                        .HasForeignKey("CapabilityId")
                        .HasConstraintName("FK_RolesCapabilities_Capabilities"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_RolesCapabilities_Roles"),
                    j =>
                    {
                        j.HasKey("RoleId", "CapabilityId");
                        j.ToTable("RolesCapabilities");
                        j.HasIndex(new[] { "CapabilityId" }, "IX_RolesCapabilities_CapabilityId");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();

            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");

            entity.HasIndex(e => e.Username, "IX_Users_Username").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Firstname).HasMaxLength(50);
            entity.Property(e => e.Lastname).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
