using System;
using System.Collections.Generic;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.OperatationManagement;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class AppointmentDbContext : DbContext
{

    public AppointmentDbContext()
    {
    }

    public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentService> AppointmentServices { get; set; }

    public virtual DbSet<Assistant> Assistants { get; set; }

    public virtual DbSet<AssistantService> AssistantServices { get; set; }

    public virtual DbSet<AvailabilityTimeSlot> AvailabilityTimeSlots { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    public virtual DbSet<UserInformation> UserInformations { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("AppointmentStatusType", new[] { "SCHEDULED", "CONFIRMED", "CANCELED", "FINISHED" })
            .HasPostgresEnum("AssistantStatusType", new[] { "ENABLED", "DISABLED", "DELETED" })
            .HasPostgresEnum("ClientStatusType", new[] { "ENABLED", "DISABLED", "DELETED" })
            .HasPostgresEnum("RoleType", new[] { "ADMINISTRATOR", "CLIENT", "ASSISTANT" })
            .HasPostgresEnum("ServiceStatusType", new[] { "ENABLED", "DISABLED", "DELETED" });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("Appointment");

            entity.HasIndex(e => e.IdAssistant, "IXFK_Appointment_Assistant");

            entity.HasIndex(e => e.IdClient, "IXFK_Appointment_Client");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval(('\"appointment_id_seq\"'::text)::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.EndTime).HasColumnName("endTime");
            entity.Property(e => e.IdAssistant).HasColumnName("id_assistant");
            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.StartTime).HasColumnName("startTime");
            entity.Property(e => e.TotalCost).HasColumnName("totalCost");
            entity.Property(e => e.Uuid).HasColumnName("uuid");

            entity.HasOne(d => d.IdAssistantNavigation).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.IdAssistant)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Appointment_Assistant");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.IdClient)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Appointment_Client");
        });

        modelBuilder.Entity<AppointmentService>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AppointmentService");

            entity.HasIndex(e => e.IdAppointment, "IXFK_AppointmentService_Appointment");

            entity.HasIndex(e => e.IdService, "IXFK_AppointmentService_Service");

            entity.Property(e => e.IdAppointment).HasColumnName("id_appointment");
            entity.Property(e => e.IdService).HasColumnName("id_service");

            entity.HasOne(d => d.IdAppointmentNavigation).WithMany()
                .HasForeignKey(d => d.IdAppointment)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_AppointmentService_Appointment");

            entity.HasOne(d => d.IdServiceNavigation).WithMany()
                .HasForeignKey(d => d.IdService)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_AppointmentService_Service");
        });

        modelBuilder.Entity<Assistant>(entity =>
        {
            entity.ToTable("Assistant");

            entity.HasIndex(e => e.IdUser, "IXFK_Assistant_UserAccount");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval(('\"assistant_id_seq\"'::text)::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Uuid).HasColumnName("uuid");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Assistants)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK_Assistant_UserAccount");
        });

        modelBuilder.Entity<AssistantService>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AssistantService");

            entity.HasIndex(e => e.IdAssistant, "IXFK_AssistantService_Assistant");

            entity.HasIndex(e => e.IdService, "IXFK_AssistantService_Service");

            entity.Property(e => e.IdAssistant).HasColumnName("id_assistant");
            entity.Property(e => e.IdService).HasColumnName("id_service");

            entity.HasOne(d => d.IdAssistantNavigation).WithMany()
                .HasForeignKey(d => d.IdAssistant)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_AssistantService_Assistant");

            entity.HasOne(d => d.IdServiceNavigation).WithMany()
                .HasForeignKey(d => d.IdService)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_AssistantService_Service");
        });

        modelBuilder.Entity<AvailabilityTimeSlot>(entity =>
        {
            entity.ToTable("AvailabilityTimeSlot");

            entity.HasIndex(e => e.IdAssistant, "IXFK_AvailabilityTimeSlot_Assistant");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval(('\"availabilitytimeslot_id_seq\"'::text)::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Date)
                .HasMaxLength(50)
                .HasColumnName("date");
            entity.Property(e => e.EndTime)
                .HasMaxLength(50)
                .HasColumnName("endTime");
            entity.Property(e => e.IdAssistant).HasColumnName("id_assistant");
            entity.Property(e => e.StartTime)
                .HasMaxLength(50)
                .HasColumnName("startTime");
            entity.Property(e => e.Uuid).HasColumnName("uuid");

            entity.HasOne(d => d.IdAssistantNavigation).WithMany(p => p.AvailabilityTimeSlots)
                .HasForeignKey(d => d.IdAssistant)
                .HasConstraintName("FK_AvailabilityTimeSlot_Assistant");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Client");

            entity.HasIndex(e => e.IdUser, "IXFK_Client_UserAccount");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval(('\"client_id_seq\"'::text)::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Uuid).HasColumnName("uuid");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Clients)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK_Client_UserAccount");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.ToTable("Service");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval(('\"service_id_seq\"'::text)::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Minutes).HasColumnName("minutes");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Uuid).HasColumnName("uuid");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.ToTable("UserAccount");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval(('\"useraccount_id_seq\"'::text)::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(16)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserInformation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("UserInformation");

            entity.HasIndex(e => e.IdUser, "IXFK_UserInformation_UserAccount");

            entity.Property(e => e.Filepath)
                .HasMaxLength(255)
                .HasColumnName("filepath");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phoneNumber");

            entity.HasOne(d => d.IdUserNavigation).WithMany()
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK_UserInformation_UserAccount");
        });
        modelBuilder.HasSequence("appointment_id_seq");
        modelBuilder.HasSequence("assistant_id_seq");
        modelBuilder.HasSequence("availabilitytimeslot_id_seq");
        modelBuilder.HasSequence("client_id_seq");
        modelBuilder.HasSequence("service_id_seq");
        modelBuilder.HasSequence("useraccount_id_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
