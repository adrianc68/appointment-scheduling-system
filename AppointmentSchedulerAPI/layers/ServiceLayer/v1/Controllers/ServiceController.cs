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
    [ApiVersion("1")]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceInterfaces systemFacade;
        private readonly IHttpResponseService httpResponseService;

        public ServiceController(IServiceInterfaces systemFacade, IHttpResponseService httpResponseService)
        {
            this.systemFacade = systemFacade;
            this.httpResponseService = httpResponseService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllServices()
        {
            List<ServiceDetailsDTO> serviceDtos = [];
            try
            {
                var services = await systemFacade.GetAllServicesAsync();
                serviceDtos = services.Select(a => new ServiceDetailsDTO
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

        [HttpPatch("enable")]
        [AllowAnonymous]
        public async Task<IActionResult> EnableService([FromBody] EnableServiceDTO dto)
        {
            bool isStatusChanged = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.EnableServiceAsync(dto.Uuid);
                if (result.IsSuccessful)
                {
                    isStatusChanged = result.Result;
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
            return httpResponseService.OkResponse(isStatusChanged, ApiVersionEnum.V1);
        }

        [HttpPatch("disable")]
        [AllowAnonymous]
        public async Task<IActionResult> DisableService([FromBody] DisableServiceDTO dto)
        {
            bool isStatusChanged = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.DisableServiceAsync(dto.Uuid);
                if (result.IsSuccessful)
                {
                    isStatusChanged = result.Result;
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
            return httpResponseService.OkResponse(isStatusChanged, ApiVersionEnum.V1);
        }

        [HttpDelete("delete")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteService([FromBody] DeleteServiceDTO dto)
        {
            bool isStatusChanged = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.DeleteServiceAsync(dto.Uuid);
                if (result.IsSuccessful)
                {
                    isStatusChanged = result.Result;
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
            return httpResponseService.OkResponse(isStatusChanged, ApiVersionEnum.V1);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterService([FromBody] CreateServiceDTO dto)
        {
            Guid? guid;
            try
            {
                BusinessLogicLayer.Model.Service service = new()
                {
                    Description = dto.Description,
                    Minutes = dto.Minutes,
                    Name = dto.Name,
                    Price = dto.Price
                };

                OperationResult<Guid, GenericError> result = await systemFacade.RegisterServiceAsync(service);
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


        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateService([FromBody] UpdateServiceDTO dto)
        {
            bool isUpdated = false;
            try
            {
                BusinessLogicLayer.Model.Service service = new()
                {
                    Description = dto.Description,
                    Minutes = dto.Minutes,
                    Name = dto.Name,
                    Price = dto.Price,
                    Uuid = dto.Uuid
                };

                OperationResult<bool, GenericError> result = await systemFacade.UpdateServiceAsync(service);
                if (result.IsSuccessful)
                {
                    isUpdated = result.Result;
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
            return httpResponseService.OkResponse(isUpdated, ApiVersionEnum.V1);
        }

    }
}