using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;
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

    public required virtual DbSet<UserAccount> UserAccounts { get; set; }

    public required virtual DbSet<UserInformation> UserInformations { get; set; }
    public required virtual DbSet<Assistant> Assistants { get; set; }
    public required virtual DbSet<Client> Clients { get; set; }
    public required virtual DbSet<Appointment> Appointments { get; set; }
    public required virtual DbSet<Service> Services { get; set; }
    public required virtual DbSet<AssistantService> AssistantServices { get; set; }
    public required virtual DbSet<AvailabilityTimeSlot> AvailabilityTimeSlots { get; set; }
    public required virtual DbSet<AppointmentAssistant> AppointmentAssistants { get; set; }
    public required virtual DbSet<AppointmentService> AppointmentServices { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum<AppointmentStatusType>("AppointmentStatusType")
            .HasPostgresEnum<ClientStatusType>("ClientStatusType")
            .HasPostgresEnum<AssistantStatusType>("AssistantType")
            .HasPostgresEnum<RoleType>("RoleType")
            .HasPostgresEnum<ServiceStatusType>("ServiceStatusType");

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
            entity.Property(e => e.Uuid)
                .HasColumnName("uuid");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(16)
                .HasColumnName("username");
            entity.Property(e => e.Role)
                .HasColumnName("role")
                .HasColumnType("RoleType");

        });

        modelBuilder.Entity<UserInformation>(entity =>
        {
            entity.ToTable("UserInformation");
            entity.HasKey(e => e.IdUser);

            entity.Property(e => e.Filepath)
                .HasMaxLength(255)
                .HasColumnName("filepath");
            entity.Property(e => e.IdUser)
                .HasColumnName("id_user");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phone_number");

            entity.HasOne(ui => ui.UserAccount)
            .WithOne(ua => ua.UserInformation)
            .HasForeignKey<UserInformation>(ui => ui.IdUser)
            .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<Assistant>(entity =>
        {
            entity.ToTable("Assistant");
            entity.HasKey(e => e.IdUserAccount);

            entity.Property(e => e.IdUserAccount)
                .HasColumnName("id_user_account");
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasColumnType("AssistantStatusType");

            entity.HasOne(d => d.UserAccount)
            .WithOne(ua => ua.Assistant)
            .HasForeignKey<Assistant>(a => a.IdUserAccount)
            .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Client");
            entity.HasKey(e => e.IdUserAccount);

            entity.Property(e => e.IdUserAccount)
                .HasColumnName("id_user_account");
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasColumnType("ClientStatusType");

            entity.HasOne(d => d.UserAccount)
            .WithOne(ua => ua.Client)
            .HasForeignKey<Client>(a => a.IdUserAccount)
            .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Appointment>(entity =>
       {
           entity.ToTable("Appointment");
           entity.HasKey(e => e.Id);

           entity.Property(e => e.Id)
               .HasDefaultValueSql("nextval(('\"appointment_id_seq\"'::text)::regclass)")
               .HasColumnName("id");
           entity.Property(e => e.CreatedAt)
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .HasColumnType("timestamp without time zone")
               .HasColumnName("created_at");
           entity.Property(e => e.Date)
                .HasColumnName("date");
           entity.Property(e => e.EndTime)
                .HasColumnName("end_time");
           entity.Property(e => e.IdClient)
                .HasColumnName("id_client");
           entity.Property(e => e.StartTime)
                .HasColumnName("start_time");
           entity.Property(e => e.TotalCost)
                .HasColumnName("total_cost");
           entity.Property(e => e.Uuid)
                .HasColumnName("uuid");
           entity.Property(e => e.Status)
               .HasColumnName("status")
               .HasColumnType("AppointmentStatusType");

           entity.HasOne(e => e.Client)
               .WithMany(c => c.Appointments)
               .HasForeignKey(e => e.IdClient)
               .OnDelete(DeleteBehavior.Cascade);
       });


        modelBuilder.Entity<Service>(entity =>
        {
            entity.ToTable("Service");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval(('\"service_id_seq\"'::text)::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnName("description");
            entity.Property(e => e.Minutes)
                .HasColumnName("minutes");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnName("price");
            entity.Property(e => e.Uuid)
                .HasColumnName("uuid");
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasColumnType("ServiceStatusType");
        });

        modelBuilder.Entity<AssistantService>(entity =>
        {
            entity.ToTable("AssistantService");
            entity.HasKey(e => new { e.IdAssistant, e.IdService });

            entity.Property(e => e.IdAssistant)
                .HasColumnName("id_assistant");
            entity.Property(e => e.IdService)
                .HasColumnName("id_service");

            entity.HasOne(e => e.Assistant)
                .WithMany(a => a.AssistantServices)
                .HasForeignKey(ase => ase.IdAssistant);

            entity.HasOne(e => e.Service)
                .WithMany(se => se.AssistantServices)
                .HasForeignKey(sse => sse.IdService);
        });

        modelBuilder.Entity<AppointmentService>(entity =>
        {
            entity.ToTable("AppointmentService");
            entity.HasKey(e => new { e.IdAppointment, e.IdService });

            entity.Property(e => e.IdAppointment)
                .HasColumnName("id_appointment");
            entity.Property(e => e.IdService)
                .HasColumnName("id_service");

            entity.HasOne(e => e.Appointment)
                .WithMany(a => a.AppointmentServices)
                .HasForeignKey(ase => ase.IdAppointment);

            entity.HasOne(e => e.Service)
                .WithMany(se => se.AppointmentServices)
                .HasForeignKey(sse => sse.IdService);
        });


        modelBuilder.Entity<AppointmentAssistant>(entity =>
        {
            entity.ToTable("AppointmentAssistant");
            entity.HasKey(e => new { e.IdAppointment, e.IdAssistant });

            entity.Property(e => e.IdAppointment)
                .HasColumnName("id_appointment");
            entity.Property(e => e.IdAssistant)
                .HasColumnName("id_assistant");

            entity.HasOne(e => e.Appointment)
                .WithMany(a => a.AppointmentAssistants)
                .HasForeignKey(ase => ase.IdAppointment);

            entity.HasOne(e => e.Assistant)
                .WithMany(se => se.AppointmentAssistants)
                .HasForeignKey(sse => sse.IdAssistant);
        });


        modelBuilder.Entity<AvailabilityTimeSlot>(entity =>
        {
            entity.ToTable("AvailabilityTimeSlot");
            entity.HasKey(e => new { e.IdAssistant, e.Id });

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
                .HasColumnName("end_time");
            entity.Property(e => e.IdAssistant)
                .HasColumnName("id_assistant");
            entity.Property(e => e.StartTime)
                .HasMaxLength(50)
                .HasColumnName("start_time");
            entity.Property(e => e.Uuid).HasColumnName("uuid");

            entity.HasOne(d => d.Assistant)
                .WithMany(p => p.AvailabilityTimeSlots)
                .HasForeignKey(d => d.IdAssistant);
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
