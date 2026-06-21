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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\sqlexpress;Database=ClinicMVCDBFirst;User=stef;Password=12345;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Greek_100_CI_AI");

        modelBuilder.Entity<Capability>(entity =>
        {
            entity.HasIndex(e => e.Name, "IX_Capabilities_Name");

            entity.HasIndex(e => e.Name, "UQ_Capabilities_Name").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasMany(d => d.Roles).WithMany(p => p.Capabilities)
                .UsingEntity<Dictionary<string, object>>(
                    "RolesCapability",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RolesId")
                        .HasConstraintName("FK_RolesCapabilities_Roles"),
                    l => l.HasOne<Capability>().WithMany()
                        .HasForeignKey("CapabilitiesId")
                        .HasConstraintName("FK_RolesCapabilities_Capabilities"),
                    j =>
                    {
                        j.HasKey("CapabilitiesId", "RolesId");
                        j.ToTable("RolesCapabilities");
                        j.HasIndex(new[] { "CapabilitiesId" }, "IX_RolesCapabilities_CapabilityId");
                    });
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasIndex(e => e.Specialty, "IX_Doctors_Specialty");

            entity.HasIndex(e => e.UserId, "IX_Doctors_UserId").IsUnique();

            entity.HasIndex(e => e.Uuid, "IX_Doctors_Uuid").IsUnique();

            entity.Property(e => e.InsertedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Doctors_InsertedAt");
            entity.Property(e => e.ModifiedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Doctors_ModifiedAt");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Specialty).HasMaxLength(50);
            entity.Property(e => e.Uuid).HasDefaultValueSql("(newid())", "DF_Doctors_Uuid");

            entity.HasOne(d => d.User).WithOne(p => p.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .HasConstraintName("FK_Doctors_UserId");
        });

        modelBuilder.Entity<MedicalProgram>(entity =>
        {
            entity.HasIndex(e => e.Description, "IX_MedicalPrograms_Description");

            entity.HasIndex(e => e.DoctorId, "IX_MedicalPrograms_DoctorId");

            entity.HasIndex(e => e.Title, "IX_MedicalPrograms_Title");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Doctor).WithMany(p => p.MedicalPrograms)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Programs_DoctorId");

            entity.HasMany(d => d.Patients).WithMany(p => p.Programs)
                .UsingEntity<Dictionary<string, object>>(
                    "PatientsProgram",
                    r => r.HasOne<Patient>().WithMany()
                        .HasForeignKey("PatientsId")
                        .HasConstraintName("FK_PatientsPrograms_Patients"),
                    l => l.HasOne<MedicalProgram>().WithMany()
                        .HasForeignKey("ProgramsId")
                        .HasConstraintName("FK_PatientsPrograms_MedicalPrograms"),
                    j =>
                    {
                        j.HasKey("ProgramsId", "PatientsId");
                        j.ToTable("PatientsPrograms");
                        j.HasIndex(new[] { "PatientsId" }, "IX_PatientsPrograms_PatientsId");
                    });
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasIndex(e => e.Amka, "IX_Patients_AMKA").IsUnique();

            entity.HasIndex(e => e.BloodType, "IX_Patients_BloodType");

            entity.HasIndex(e => e.UserId, "IX_Patients_UserId").IsUnique();

            entity.HasIndex(e => e.Uuid, "IX_Patients_Uuid").IsUnique();

            entity.Property(e => e.Amka)
                .HasMaxLength(11)
                .HasColumnName("AMKA");
            entity.Property(e => e.BloodType).HasMaxLength(20);
            entity.Property(e => e.InsertedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Patients_InsertedAt");
            entity.Property(e => e.ModifiedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Patients_ModifiedAt");
            entity.Property(e => e.Uuid).HasDefaultValueSql("(newid())", "DF_Patients_Uuid");

            entity.HasOne(d => d.User).WithOne(p => p.Patient)
                .HasForeignKey<Patient>(d => d.UserId)
                .HasConstraintName("FK_Patients_UserId");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.Name, "IX_Roles_Name");

            entity.HasIndex(e => e.Name, "UQ_Roles_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();

            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");

            entity.HasIndex(e => e.Username, "IX_Users_Username").IsUnique();

            entity.HasIndex(e => e.Uuid, "IX_Users_Uuid").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Firstname).HasMaxLength(50);
            entity.Property(e => e.InsertedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Users_InsertedAt");
            entity.Property(e => e.Lastname).HasMaxLength(50);
            entity.Property(e => e.ModifiedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Users_ModifiedAt");
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.Uuid).HasDefaultValueSql("(newid())", "DF_Users_Uuid");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_RoleId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
