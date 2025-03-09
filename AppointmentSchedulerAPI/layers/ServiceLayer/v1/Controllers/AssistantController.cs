using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Authorization.Attributes;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
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

        [HttpPatch("disable")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> DisableAssistant([FromBody] DisableAssistantDTO dto)
        {
            bool isStatusChanged = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.DisableAssistantAsync(dto.Uuid);
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

        [HttpPatch("enable")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> EnableAssistant([FromBody] EnableAssistantDTO dto)
        {
            bool isStatusChanged = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.EnableAssistantAsync(dto.Uuid);
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
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> DeleteAssistant([FromQuery] Guid uuid)
        {
            bool isStatusChanged = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.DeleteAssistantAsync(uuid);
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

        [HttpGet]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> GetAllAssistant()
        {
            List<AssistantDetailsDTO> assistantDtos = [];
            try
            {
                var assistants = await systemFacade.GetAllAssistantsAsync();
                assistantDtos = assistants.Select(a => new AssistantDetailsDTO
                {
                    Uuid = a.Uuid,
                    Email = a.Email,
                    Name = a.Name,
                    PhoneNumber = a.PhoneNumber,
                    Username = a.Username,
                    Status = a.Status.ToString(),
                    CreatedAt = a.CreatedAt
                }).ToList();
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1); ;
            }
            return httpResponseService.OkResponse(assistantDtos, ApiVersionEnum.V1);
        }

        [HttpPost]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> RegisterAssistant([FromBody] CreateAssistantDTO dto)
        {
            Guid? guid;
            try
            {
                BusinessLogicLayer.Model.Assistant assistant = new()
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    Password = dto.Password,
                    Username = dto.Username
                };
                OperationResult<Guid, GenericError> result = await systemFacade.RegisterAssistantAsync(assistant);
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
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> UpdateAssistant([FromBody] UpdateAssistantDTO dto)
        {
            bool isUpdated = false;
            try
            {
                BusinessLogicLayer.Model.Assistant assistant = new()
                {
                    Uuid = dto.Uuid,
                    Name = dto.Name,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    Username = dto.Username
                };
                OperationResult<bool, GenericError> result = await systemFacade.EditAssistantAsync(assistant);
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

        [HttpPost("service")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> AssignServiceToAssistant([FromBody] AssignServiceToAssistantDTO assignServiceDTO)
        {
            bool isAssigned = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.AssignListServicesToAssistantAsync(assignServiceDTO.AssistantUuid, assignServiceDTO.SelectedServices);
                if (result.IsSuccessful)
                {
                    isAssigned = result.Result;
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
            return httpResponseService.OkResponse(isAssigned, ApiVersionEnum.V1);

        }



        [HttpGet("service")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> GetAssignedServicesOfAssistant([FromQuery] Guid uuid)
        {
            List<ServiceOfferDTO> servicesDtos = [];
            try
            {
                var result = await systemFacade.GetAllAssignedServicesAsync(uuid);
                if (!result.IsSuccessful)
                {
                    return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());
                }

                if (result.Result != null)
                {
                    servicesDtos = result.Result.Select(a => new ServiceOfferDTO
                    {
                        Uuid = a.Uuid,
                        Name = a.Service!.Name,
                        Price = a.Service.Price,
                        Minutes = a.Service.Minutes,
                        Description = a.Service.Description,
                        Status = a.Status!.Value,
                    }).ToList();
                }
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1); ;
            }
            return httpResponseService.OkResponse(servicesDtos, ApiVersionEnum.V1);
        }


        [HttpGet("service/all")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> GetAssistantWithTheirServicesOffer()
        {
            List<AssistantWithServicesOfferDTO> servicesDtos = [];
            try
            {
                var result = await systemFacade.GetAllAssistantsAndServicesOffer();
                servicesDtos = result
                                .GroupBy(a => new { a.Assistant!.Uuid, a.Assistant.Name })
                                .Select(g => new AssistantWithServicesOfferDTO
                                {
                                    Assistant = new AssistantServiceOfferDTO
                                    {
                                        Uuid = g.Key.Uuid,
                                        Name = g.Key.Name
                                    },
                                    ServiceOffer = g.Select(a => new ServiceOfferDTO
                                    {
                                        Uuid = a.Uuid,
                                        Name = a.Service!.Name,
                                        Price = a.Service.Price,
                                        Minutes = a.Service.Minutes,
                                        Description = a.Service.Description,
                                        Status = a.Status!.Value
                                    }).ToList()
                                }).ToList();
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1); ;
            }
            return httpResponseService.OkResponse(servicesDtos, ApiVersionEnum.V1);
        }

    }
}
