using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
    public required virtual DbSet<ServiceOffer> ServiceOffers { get; set; }
    public required virtual DbSet<AvailabilityTimeSlot> AvailabilityTimeSlots { get; set; }
    public required virtual DbSet<UnavailableTimeSlot> UnavailableTimeSlots { get; set; }
    public required virtual DbSet<ScheduledService> ScheduledServices { get; set; }
    public required virtual DbSet<AppointmentNotification> AppointmentNotifications { get; set; }
    public required virtual DbSet<SystemNotification> SystemNotifications { get; set; }
    public required virtual DbSet<GeneralNotification> GeneralNotifications { get; set; }
    public required virtual DbSet<NotificationBase> NotificationBases { get; set; }
    public required virtual DbSet<NotificationRecipient> NotificationRecipients { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder
            .HasPostgresEnum<AppointmentStatusType>("AppointmentStatusType")
            .HasPostgresEnum<ClientStatusType>("ClientStatusType")
            .HasPostgresEnum<AssistantStatusType>("AssistantType")
            .HasPostgresEnum<RoleType>("RoleType")
            .HasPostgresEnum<ServiceStatusType>("ServiceStatusType")
            .HasPostgresEnum<ServiceOfferStatusType>("ServiceOfferStatusType")
            .HasPostgresEnum<AvailabilityTimeSlotStatusType>("AvailabilityTimeSlotStatusType")
            .HasPostgresEnum<AccountStatusType>("AccountStatusType")
            .HasPostgresEnum<NotificationStatusType>("NotificationStatusType")

            .HasPostgresEnum<AppointmentNotificationCodeType>("AppointmentNotificationCodeType")
            .HasPostgresEnum<GeneralNotificationCodeType>("GeneralNotificationCodeType")
            .HasPostgresEnum<SystemNotificationCodeType>("SystemNotificationCodeType")
            .HasPostgresEnum<SystemNotificationSeverityCodeType>("SystemNotificationSeverityCodeType");


        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),          // Al guardar: UTC
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // Al leer: UTC
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType.GetProperties()
                .Where(p => p.PropertyType == typeof(DateTime));

            foreach (var property in properties)
            {
                modelBuilder.Entity(entityType.Name)
                            .Property(property.Name)
                            .HasConversion(dateTimeConverter);
            }
        };

        // base.OnModelCreating(modelBuilder);


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
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasColumnType("AccountStatusType");
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

        modelBuilder.Entity<NotificationBase>(entity =>
         {
             entity.ToTable("NotificationBase");
             entity.HasKey(e => e.Id);

             entity.Property(e => e.Id)
                 .HasColumnName("id");
             entity.Property(e => e.Uuid)
                 .HasColumnName("uuid");
             entity.Property(e => e.CreatedAt)
                 .HasColumnName("created_at");

             entity.Property(e => e.Message)
                 .HasColumnName("message");
             entity.Property(e => e.Type)
                 .HasColumnType("NotificationType")
                 .HasColumnName("type");
         });

        modelBuilder.Entity<NotificationRecipient>(entity =>
        {
            entity.ToTable("NotificationRecipient");
            entity.HasKey(e => new { e.IdUserAccount, e.IdNotificationBase });

            entity.Property(e => e.IdNotificationBase)
                .HasColumnName("id_notification");
            entity.Property(e => e.IdUserAccount)
                .HasColumnName("id_user_account");
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasColumnType("NotificationStatusType");
            entity.Property(e => e.ChangedAt)
                .HasColumnName("changed_at");

            entity.HasOne(e => e.UserAccount)
                .WithMany(a => a.NotificationRecipients)
                .HasForeignKey(e => e.IdUserAccount);

            entity.HasOne(e => e.NotificationBase)
                .WithMany(e => e.NotificationRecipients)
                .HasForeignKey(e => e.IdNotificationBase);
        });


        modelBuilder.Entity<AppointmentNotification>(entity =>
        {
            entity.ToTable("AppointmentNotification");
            entity.HasKey(e => new { e.IdAppointment, e.IdNotificationBase });

            entity.Property(e => e.IdAppointment)
                .HasColumnName("id_appointment");
            entity.Property(e => e.IdNotificationBase)
                .HasColumnName("id_notification");
            entity.Property(e => e.Code)
                .HasColumnName("code")
                .HasColumnType("AppointmentNotificationCodeType");

            entity.HasOne(a => a.Appointment)
                .WithMany(a => a.AppointmentNotifications)
                .HasForeignKey(a => a.IdAppointment)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.NotificationBase)
                  .WithOne(e => e.AppointmentNotification)
                  .HasForeignKey<AppointmentNotification>(x => x.IdNotificationBase)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SystemNotification>(entity =>
        {
            entity.ToTable("SystemNotification");
            entity.HasKey(e => e.IdNotificationBase);

            entity.Property(e => e.IdNotificationBase)
                .HasColumnName("id_notification");
            entity.Property(e => e.Code)
                .HasColumnName("code")
                .HasColumnType("SystemNotificationCodeType");
            entity.Property(e => e.Severity)
                .HasColumnName("severity")
                .HasColumnType("SystemNotificationSeverityCodeType");

            entity.HasOne(a => a.NotificationBase)
                .WithOne(e => e.SystemNotification)
                .HasForeignKey<SystemNotification>(x => x.IdNotificationBase)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GeneralNotification>(entity =>
        {
            entity.ToTable("GeneralNotification");
            entity.HasKey(e => e.IdNotificationBase);

            entity.Property(e => e.IdNotificationBase)
                .HasColumnName("id_notification");
            entity.Property(e => e.Code)
                .HasColumnName("code")
                .HasColumnType("GeneralNotificationCodeType");

            entity.HasOne(a => a.NotificationBase)
                .WithOne(e => e.GeneralNotification)
                .HasForeignKey<GeneralNotification>(x => x.IdNotificationBase)
                .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<Assistant>(entity =>
        {
            entity.ToTable("Assistant");
            entity.HasKey(e => e.IdUserAccount);

            entity.Property(e => e.IdUserAccount)
                .HasColumnName("id_user_account");

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
           entity.Property(e => e.StartDate)
                .HasColumnName("start_date");
           entity.Property(e => e.EndDate)
                .HasColumnName("end_date");
           entity.Property(e => e.IdClient)
                .HasColumnName("id_client");
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

        modelBuilder.Entity<ServiceOffer>(entity =>
        {
            entity.ToTable("ServiceOffer");
            entity.HasKey(e => e.Id);


            entity.Property(e => e.IdAssistant)
                .HasColumnName("id_assistant");
            entity.Property(e => e.IdService)
                .HasColumnName("id_service");
            entity.Property(e => e.Uuid)
                .HasColumnName("uuid");
            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval(('\"service_id_seq\"'::text)::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasColumnType("ServiceOfferStatusType");

            entity.HasOne(e => e.Assistant)
                .WithMany(a => a.ServiceOffers)
                .HasForeignKey(ase => ase.IdAssistant);

            entity.HasOne(e => e.Service)
                .WithMany(se => se.ServiceOffers)
                .HasForeignKey(sse => sse.IdService);
        });

        modelBuilder.Entity<ScheduledService>(entity =>
        {
            entity.ToTable("ScheduledService");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.IdAppointment)
                .HasColumnName("id_appointment");
            entity.Property(e => e.IdServiceOffer)
                .HasColumnName("id_serviceOffer");
            entity.Property(e => e.ServiceStartDate)
                .HasColumnName("start_date");
            entity.Property(e => e.ServiceEndDate)
                .HasColumnName("end_date");
            entity.Property(e => e.ServicePrice)
                .HasColumnName("service_price");
            entity.Property(e => e.ServiceName)
                .HasColumnName("service_name");
            entity.Property(e => e.ServicesMinutes)
                .HasColumnName("service_minutes");
            entity.Property(e => e.Uuid)
                .HasColumnName("uuid");

            entity.HasOne(e => e.Appointment)
                .WithMany(a => a.ScheduledServices)
                .HasForeignKey(ase => ase.IdAppointment);

            entity.HasOne(e => e.ServiceOffer)
                .WithMany(se => se.ScheduledServices)
                .HasForeignKey(sse => sse.IdServiceOffer);
        });

        modelBuilder.Entity<AvailabilityTimeSlot>(entity =>
        {
            entity.ToTable("AvailabilityTimeSlot");
            entity.HasKey(e => new { e.Id });

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval(('\"availabilitytimeslot_id_seq\"'::text)::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.StartDate)
                .HasMaxLength(50)
                .HasColumnName("start_date");
            entity.Property(e => e.EndDate)
                .HasMaxLength(50)
                .HasColumnName("end_date");
            entity.Property(e => e.IdAssistant)
                .HasColumnName("id_assistant");
            entity.Property(e => e.Status)
                .HasColumnName("status");

            entity.Property(e => e.Uuid).HasColumnName("uuid");

            entity.HasOne(d => d.Assistant)
                .WithMany(p => p.AvailabilityTimeSlots)
                .HasForeignKey(d => d.IdAssistant);

            entity.HasMany(d => d.UnavailableTimeSlots)
                .WithOne(p => p.AvailabilityTimeSlot)
                .HasForeignKey(e => e.IdAvailabilityTimeSlot);
        });

        modelBuilder.Entity<UnavailableTimeSlot>(entity =>
        {
            entity.ToTable("UnavailableTimeSlot");
            entity.HasKey(e => new { e.StartDate, e.EndDate, e.IdAvailabilityTimeSlot });

            entity.Property(e => e.IdAvailabilityTimeSlot)
                .HasColumnName("id_availability_time_slot");
            entity.Property(e => e.StartDate)
                .HasColumnName("start_date");
            entity.Property(e => e.EndDate)
            .HasColumnName("end_date");
        });


        modelBuilder.HasSequence("appointment_id_seq");
        modelBuilder.HasSequence("availabilitytimeslot_id_seq");
        modelBuilder.HasSequence("notificationbase_id_seq");
        modelBuilder.HasSequence("scheduledservice_id_seq");
        modelBuilder.HasSequence("service_id_seq");
        modelBuilder.HasSequence("serviceoffer_id_seq");
        modelBuilder.HasSequence("useraccount_id_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
