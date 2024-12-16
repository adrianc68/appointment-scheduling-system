using AppointmentSchedulerAPI.layers.BusinessLogicLayer;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.OperatationManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; 
});



builder.Services.AddOpenApi();
builder.Services.AddSingleton<EnvironmentVariableMgr>();

builder.Services.AddScoped<ISchedulerMgt, SchedulerMgr>();
builder.Services.AddScoped<IClientMgt, ClientMgr>();
builder.Services.AddScoped<IAssistantMgt, AssistantMgr>();
builder.Services.AddScoped<IServiceMgt, ServiceMgr>();


builder.Services.AddScoped<ISchedulingInterfaces, AppointmentSchedulingSystemFacade>();
builder.Services.AddScoped<IServiceInterfaces, AppointmentSchedulingSystemFacade>();
builder.Services.AddScoped<IAssistantInterfaces, AppointmentSchedulingSystemFacade>();
builder.Services.AddScoped<IClientInterfaces, AppointmentSchedulingSystemFacade>();

builder.Services.AddScoped<IHttpResponseService, HttpResponseService>();




builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Appointment Scheduling System API",
        Version = "v1",
        Description = "API for managing appointments and scheduling services",
    });

});


var app = builder.Build();


app.Use(async (context, next) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");
    await next.Invoke();
});






var envManager = app.Services.GetRequiredService<EnvironmentVariableMgr>();
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