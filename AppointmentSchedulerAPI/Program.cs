using System.Text;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
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
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = envManager.Get("JWT_ISSUER"),
        ValidAudience = envManager.Get("JWT_AUDIENCE"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(envManager.Get("JWT_SECRET_KEY")))
    };
});

builder.Services.AddOpenApi();
builder.Services.AddSingleton<EnvironmentVariableService>();

builder.Services.AddScoped<ISchedulerMgt, SchedulerMgr>();
builder.Services.AddScoped<IClientMgt, ClientMgr>();
builder.Services.AddScoped<IAssistantMgt, AssistantMgr>();
builder.Services.AddScoped<IServiceMgt, ServiceMgr>();

builder.Services.AddScoped<ISchedulingInterfaces, AppointmentSchedulingSystemFacade>();
builder.Services.AddScoped<IServiceInterfaces, AppointmentSchedulingSystemFacade>();
builder.Services.AddScoped<IAssistantInterfaces, AppointmentSchedulingSystemFacade>();
builder.Services.AddScoped<IClientInterfaces, AppointmentSchedulingSystemFacade>();

builder.Services.AddScoped<ISchedulerRepository, SchedulerRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAssistantRepository, AssistantRepository>();

builder.Services.AddScoped<IExceptionHandlerService, ExceptionHandlerService>();
builder.Services.AddScoped<IHttpResponseService, HttpResponseService>();

builder.Services.AddSingleton<IAuthenticationService<JwtUserCredentials, JwtTokenResult>>(provider =>
{
    return new JwtAuthenticationService(
        envManager.Get("JWT_ISSUER"),
        envManager.Get("JWT_AUDIENCE"),
        envManager.Get("JWT_SECRET_KEY")
    );
});

builder.Services.AddControllers();
var app = builder.Build();

// $$$>> This middlewares causes problems with authorization! Fix it 
// app.UseMiddleware<HttpResponseAuthorizationMiddleware>(); 
app.UseAuthorization();
app.UseAuthentication();


app.Use(async (context, next) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");
    await next.Invoke();
});

var port = envManager.Get("SERVER_PORT", "8000");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");

        options.RoutePrefix = string.Empty;
    });
}


app.MapControllers();
app.Run($"http://0.0.0.0:{port}");