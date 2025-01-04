using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces;
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
    public class ClientController : ControllerBase
    {
        private readonly IClientInterfaces systemFacade;
        private readonly IHttpResponseService httpResponseService;

        public ClientController(IClientInterfaces systemFacade, IHttpResponseService httpResponseService)
        {
            this.systemFacade = systemFacade;
            this.httpResponseService = httpResponseService;
        }

        // public IActionResult EditClient()
        // {
        //     throw new NotImplementedException();
        // }

        [HttpPatch("disable")]
        [AllowAnonymous]
        public async Task<IActionResult> DisableClient([FromBody] DisableClientDTO dto)
        {
            bool isStatusChanged = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.DisableClientAsync(dto.ClientUuid);
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
        [AllowAnonymous]
        public async Task<IActionResult> EnableClient([FromBody] EnableClientDTO dto)
        {
            bool isStatusChanged = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.EnableClientAsync(dto.ClientUuid);
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
        public async Task<IActionResult> DeleteClient([FromBody] DeleteClientDTO dto)
        {
            bool isStatusChanged = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.DeleteClientAsync(dto.ClientUuid);
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
        [AllowAnonymous]
        public async Task<IActionResult> GetAllClient()
        {
            List<ClientDTO> clientDTO = [];
            try
            {
                var clients = await systemFacade.GetAllClientsAsync();
                clientDTO = clients.Select(a => new ClientDTO
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
            return httpResponseService.OkResponse(clientDTO, ApiVersionEnum.V1);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterClient([FromBody] CreateClientDTO clientDTO)
        {
            Guid? guid;
            try
            {
                BusinessLogicLayer.Model.Client client = new()
                {
                    Name = clientDTO.Name,
                    Email = clientDTO.Email,
                    PhoneNumber = clientDTO.PhoneNumber,
                    Password = clientDTO.Password,
                    Username = clientDTO.Username
                };

                OperationResult<Guid, GenericError> result = await systemFacade.RegisterClientAsync(client);
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
        public async Task<IActionResult> UpdateClient([FromBody] UpdateClientDTO dto)
        {
            bool isUpdated = false;
            try
            {
                BusinessLogicLayer.Model.Client client = new()
                {
                    Uuid = dto.Uuid,
                    Name = dto.Name,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    Password = dto.Password,
                    Username = dto.Username
                };
                OperationResult<bool, GenericError> result = await systemFacade.UpdateClientAsync(client);
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
