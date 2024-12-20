using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult RegisterAssistant()
        {
            // systemFacade.RegisterAssistant(new BusinessLogicLayer.Model.Assistant());
            try
            {

                var userAccount = new DataLayer.DatabaseComponents.Model.UserAccount
                {
                    Email = "test@hotmail.com",
                    Password = "hellowolrd",
                    Username = "Testing",
                    // Role = "ADMINISTRATOR"
                };

                db.UserAccounts.Add(userAccount);
                db.SaveChanges();
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
