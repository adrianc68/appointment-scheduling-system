using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    public class AssistantController : ControllerBase
    {
        private readonly IAssistantInterfaces systemFacade;
        private readonly IHttpResponseService httpResponseService;
        private readonly AppointmentDbContext db;

        public AssistantController(IAssistantInterfaces systemFacade, IHttpResponseService httpResponseService, AppointmentDbContext db)
        {
            this.systemFacade = systemFacade;
            this.httpResponseService = httpResponseService;
            this.db = db;
        }

        // public IActionResult DisableAssistant()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult EnableAssistant()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult DeleteAssistant()
        // {
        //     throw new NotImplementedException();
        // }

        [HttpGet("test")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAssistant()
        {
            // systemFacade.RegisterAssistant(new BusinessLogicLayer.Model.Assistant());
            try
            {


                // var userAccount = new UserAccount
                // {
                //     Email = "test2@example.com",
                //     Password = "securepassword",
                //     Username = "testuser34",
                //     Role = RoleType.ASSISTANT
                // };

                // db.UserAccounts.Add(userAccount);
                // await db.SaveChangesAsync();

                // var userInformation = new UserInformation
                // {
                //     Name = "Assistant User",
                //     PhoneNumber = "123456789",
                //     Filepath = null,
                //     IdUser = userAccount.Id 
                // };

                // db.UserInformations.Add(userInformation);
                // await db.SaveChangesAsync();

                // var assistant = new DataLayer.DatabaseComponents.Model.Assistant
                // {
                //     IdUserAccount = userAccount.Id,
                //     Status = AssistantStatusType.ENABLED
                // };

                // var client = new DataLayer.DatabaseComponents.Model.Client
                // {
                //     IdUserAccount = userAccount.Id,
                //     Status = ClientStatusType.ENABLED
                // };

                // db.Assistants.Add(assistant);
                // await db.SaveChangesAsync();



                // db.Cients.Add(client);
                // await db.SaveChangesAsync();

                // var userData = await db.UserAccounts
                // .Where(u => u.Id == 17)
                // .Include(u => u.UserInformation)
                // .Include(u => u.Client)
                // .FirstOrDefaultAsync();


                // if (userData?.Client?.UserAccount?.Email != null)
                // {
                //     string email = userData.Client.UserAccount.Email;
                //     Console.WriteLine(userData.UserInformation.Name);
                //     Console.WriteLine(userData.Email);
                //     Console.WriteLine(userData.Client.Uuid);
                //     Console.WriteLine(userData.Client.Status);
                //     Console.WriteLine(email);
                // }
                // else
                // {
                //     Console.WriteLine("Email not found.");
                // }

                // var idAssistant = 1;
                // var idClient = 1;
                // var appointment = new DataLayer.DatabaseComponents.Model.Appointment
                // {
                //     Date = DateTime.UtcNow.AddDays(1), 
                //     StartTime = new TimeOnly(10, 0),  
                //     EndTime = new TimeOnly(11, 0),    
                //     TotalCost = 100.0m,               
                //     Uuid = Guid.NewGuid(),            
                //     Status = AppointmentStatusType.SCHEDULED,
                //     IdAssistant = idAssistant,      
                //     IdClient = idClient
                // };

                // db.Appointments.Add(appointment);
                // await db.SaveChangesAsync();


                // Eager Loading Entity Framework
                //     var appointments = db.Appointments
                // .Include(a => a.Assistant)
                // .ThenInclude(a => a.UserAccount)
                // .ThenInclude(ua => ua.UserInformation)
                // .Include(a => a.Client)
                // .ThenInclude(c => c.UserAccount)
                // .ThenInclude(ua => ua.UserInformation)
                // .ToList();
                // var appointments = db.Appointments
                //     .Include(a => a.Assistant)
                //     .Include(a => a.Client)
                //     .Select(a => new
                //     {
                //         a.Id,
                //         a.Date,
                //         AssistantId = a.Assistant.Id,
                //         AssistantName = a.Assistant.UserAccount.UserInformation.Name,
                //         ClientId = a.Client.Id,
                //         ClientName = a.Client.UserAccount.UserInformation.Name
                //     })
                //     .ToList();

                // PropToString.PrintListData(appointments);
                // PropToString.PrintData(appointments[0].Assistant);
                // PropToString.PrintData(appointments[0].Client);

                // var service = new DataLayer.DatabaseComponents.Model.Service
                // {
                //     Description = "Servicio para responder el problema 1",
                //     Minutes = 60,
                //     Name = "Servicio 1",
                //     Price = 500,
                //     Uuid =Guid.NewGuid(),
                //     Status = ServiceStatusType.ENABLED
                // };

                // db.Services.Add(service);
                // await db.SaveChangesAsync(); 

                // Crear una nueva instancia de AssistantService
                // var assistantService = new AssistantService
                // {
                //     IdAssistant = 1,
                //     IdService = 1
                // };

                // db.AssistantServices.Add(assistantService);

                // db.SaveChanges();


                //     var assistantId = 1;
                //     var assistantWithServices = db.Assistants
                //         .Include(a => a.AssistantServices)  
                //         .ThenInclude(se => se.Service)
                //         .FirstOrDefault(a => a.Id == assistantId);

                // PropToString.PrintData(assistantWithServices);
                // PropToString.PrintData(assistantWithServices.AssistantServices.First().Service);


                // var serviceId = 1; 
                // var serviceWithAssistants = db.Services
                //     .Include(s => s.AssistantServices) 
                //     .ThenInclude(se => se.Assistant)   
                //     .FirstOrDefault(s => s.Id == serviceId);

                // PropToString.PrintData(serviceWithAssistants);


                // var idappointment = 2;
                // var appointmentWithServices = db.Appointments
                //     .Include(a => a.AppointmentServices)
                //     .ThenInclude(ase => ase.Service)
                //     .FirstOrDefault(a => a.Id == idappointment);


                // PropToString.PrintData(appointmentWithServices);


                // Avalability Time Slot has wrong types
                // $$$>>


                // string date = "2024-12-25";  
                // string startTime = "09:00 AM";
                // string endTime = "11:00 AM";  

                // // Crear el AvailabilityTimeSlot con valores
                // var availabilityTimeSlot = new AvailabilityTimeSlot
                // {
                //     IdAssistant = 1,          
                //     Date = date,              
                //     StartTime = startTime,    
                //     EndTime = endTime,        
                //     Uuid = Guid.NewGuid(),    
                //     CreatedAt = DateTime.Now  
                // };

                // db.AvailabilityTimeSlots.Add(availabilityTimeSlot);
                // db.SaveChanges();


            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
            return httpResponseService.OkResponse("test", ApiVersionEnum.V1);
            throw new NotImplementedException();
        }

        // public IActionResult EditAssistant()
        // {
        //     throw new NotImplementedException();
        // }



    }
}
