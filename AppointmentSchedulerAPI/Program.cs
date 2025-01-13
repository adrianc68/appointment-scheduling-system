using System.Text;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AccountInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.AccountMgr.Component;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.AccountMgr.Interfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Component;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Interfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.SignalRNotifier;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeSlotLock.Component;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeSlotLock.Interfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.OperatationManagement;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.OperatationManagement.ExceptionHandlerService;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Authentication;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryComponents;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContextFactory<AppointmentDbContext>((provider, options) =>
{
    var envService = provider.GetRequiredService<EnvironmentVariableService>();
    var connectionString = envService.Get("DEFAULT_DB_CONNECTION");
    options.UseNpgsql(connectionString, o =>
    {
        o.MapEnum<RoleType>("RoleType");
        o.MapEnum<AssistantStatusType>("AssistantStatusType");
        o.MapEnum<ClientStatusType>("ClientStatusType");
        o.MapEnum<AppointmentStatusType>("AppointmentStatusType");
        o.MapEnum<ServiceStatusType>("ServiceStatusType");
        o.MapEnum<ServiceOfferStatusType>("ServiceOfferStatusType");
        o.MapEnum<AvailabilityTimeSlotStatusType>("AvailabilityTimeSlotStatusType");
        o.MapEnum<AccountStatusType>("AccountStatusType");
    });
});


builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Appointment Scheduling System API",
        Version = "v1",
        Description = "API for managing appointments and scheduling services",
    });

});

var envManager = new EnvironmentVariableService();


builder.Services.AddOpenApi();
builder.Services.AddSignalR(opt =>
{
    opt.EnableDetailedErrors = true;
});

builder.Services.AddSingleton<EnvironmentVariableService>();

builder.Services.AddScoped<IAccountEvent<AssistantEvent>, AccountMgr>(); // Para eventos de Assistant
builder.Services.AddScoped<IAccountEvent<ClientEvent>, AccountMgr>();    // Para eventos de Client

builder.Services.AddScoped<IClientEvent, ClientMgr>();
builder.Services.AddScoped<IClientObserver, SchedulerMgr>();

builder.Services.AddScoped<IServiceEvent, ServiceMgr>();
builder.Services.AddScoped<IServiceObserver, SchedulerMgr>();

builder.Services.AddScoped<IAssistantEvent, AssistantMgr>();
builder.Services.AddScoped<IAssistantObserver, SchedulerMgr>();

builder.Services.AddScoped<ISchedulerEvent, SchedulerMgr>();
builder.Services.AddScoped<ISchedulerObserver, SchedulerMgr>();

builder.Services.AddScoped<INotificationMgt, NotificationMgr>();
builder.Services.AddScoped<INotifier, NotificationHub>();

builder.Services.AddScoped<ISchedulerMgt, SchedulerMgr>();
builder.Services.AddScoped<IClientMgt, ClientMgr>();
builder.Services.AddScoped<IAssistantMgt, AssistantMgr>();
builder.Services.AddScoped<IServiceMgt, ServiceMgr>();
builder.Services.AddScoped<IAccountMgt, AccountMgr>();

builder.Services.AddScoped<ISchedulingInterfaces, AppointmentSchedulingSystemFacade>();
builder.Services.AddScoped<IServiceInterfaces, AppointmentSchedulingSystemFacade>();
builder.Services.AddScoped<IAssistantInterfaces, AppointmentSchedulingSystemFacade>();
builder.Services.AddScoped<IClientInterfaces, AppointmentSchedulingSystemFacade>();
builder.Services.AddScoped<IAccountInterfaces, AppointmentSchedulingSystemFacade>();

builder.Services.AddScoped<ISchedulerRepository, SchedulerRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAssistantRepository, AssistantRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<IExceptionHandlerService, ExceptionHandlerService>();
builder.Services.AddScoped<IHttpResponseService, HttpResponseService>();
builder.Services.AddScoped<ITimeSlotLockMgt, TimeSlotLockMgr>();


builder.Services.AddSingleton<IAuthenticationService<JwtUserCredentials, JwtTokenResult, JwtTokenData>>(provider =>
{
    return new JwtAuthenticationService(
        envManager.Get("JWT_ISSUER"),
        envManager.Get("JWT_AUDIENCE"),
        envManager.Get("JWT_SECRET_KEY")
    );
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = envManager.Get("JWT_ISSUER"),
            ValidateAudience = true,
            ValidAudience = envManager.Get("JWT_AUDIENCE"),
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(envManager.Get("JWT_SECRET_KEY")))
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var userClaims = context.Principal!.Claims;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllers();



var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var clientMgrEventPublisher = scope.ServiceProvider.GetRequiredService<IClientEvent>() as ClientMgr;
    var serviceMgrEventPublisher = scope.ServiceProvider.GetRequiredService<IServiceEvent>() as ServiceMgr;
    var assistantMgrEventPublisher = scope.ServiceProvider.GetRequiredService<IAssistantEvent>() as AssistantMgr;
    var schedulerEventPublisher = scope.ServiceProvider.GetRequiredService<ISchedulerEvent>() as SchedulerMgr;


    var accountAssistantEventPublisher = scope.ServiceProvider.GetRequiredService<IAccountEvent<AssistantEvent>>();
    var accountClientEventPublisher = scope.ServiceProvider.GetRequiredService<IAccountEvent<ClientEvent>>();

    var schedulerMgr = scope.ServiceProvider.GetRequiredService<ISchedulerMgt>() as SchedulerMgr;

    clientMgrEventPublisher?.Suscribe(schedulerMgr!);
    serviceMgrEventPublisher?.Suscribe(schedulerMgr!);
    assistantMgrEventPublisher?.Suscribe(schedulerMgr!);
    schedulerEventPublisher?.Suscribe(schedulerMgr!);
    accountAssistantEventPublisher?.Subscribe(schedulerMgr!);
    accountClientEventPublisher?.Subscribe(schedulerMgr!);
}


app.UseAuthentication();
app.UseMiddleware<HttpResponseAuthorizationMiddleware>();
app.UseAuthorization();



app.Use(async (context, next) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");
    await next.Invoke();
});

var port = envManager.Get("SERVER_PORT");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");

        options.RoutePrefix = string.Empty;
    });
}


app.UseCors(policy => policy
    // $$$>> Resolve this! Use .env file or something else
    .WithOrigins("http://localhost:8080")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());



app.MapHub<NotificationHub>("/notificationHub");
app.MapControllers();

app.Run($"http://0.0.0.0:{port}");