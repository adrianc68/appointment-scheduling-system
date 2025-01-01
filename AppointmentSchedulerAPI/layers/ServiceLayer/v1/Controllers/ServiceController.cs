using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ApiVersion(ApiVersionEnum.V1)]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceInterfaces systemFacade;
        private readonly IHttpResponseService httpResponseService;

        public ServiceController(IServiceInterfaces systemFacade, IHttpResponseService httpResponseService)
        {
            this.systemFacade = systemFacade;
            this.httpResponseService = httpResponseService;
        }

        // public IActionResult DisableService()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult EnableService()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult DeleteService()
        // {
        //     throw new NotImplementedException();
        // }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllServices()
        {
            List<ServiceDTO> serviceDtos = [];
            try
            {
                var services = await systemFacade.GetAllServicesAsync();
                serviceDtos = services.Select(a => new ServiceDTO
                {
                    Uuid = a.Uuid,
                    Status = a.Status.ToString(),
                    Description = a.Description,
                    Name = a.Name,
                    Minutes = a.Minutes,
                    Price = a.Price,
                    CreatedAt = a.CreatedAt
                }).ToList();
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1); ;
            }
            return httpResponseService.OkResponse(serviceDtos, ApiVersionEnum.V1);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterService([FromBody] CreateServiceDTO serviceDTO)
        {
            Guid? guid;
            try
            {
                BusinessLogicLayer.Model.Service service = new()
                {
                    Description = serviceDTO.Description,
                    Minutes = serviceDTO.Minutes,
                    Name = serviceDTO.Name,
                    Price = serviceDTO.Price
                };

                OperationResult<Guid, GenericError> result = await systemFacade.RegisterService(service);
                if (result.IsSuccessful)
                {
                    guid = result.Result;
                }
                else
                {
                    return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());
                }
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(guid, ApiVersionEnum.V1);
        }

        [HttpDelete("{idService}")]
        public IActionResult DeleteService(int idService)
        {
            var appointments = systemFacade.DeleteService(idService);
            return Ok(appointments);
        }


        // public IActionResult EditService()
        // {
        //     throw new NotImplementedException();
        // }


    }
}