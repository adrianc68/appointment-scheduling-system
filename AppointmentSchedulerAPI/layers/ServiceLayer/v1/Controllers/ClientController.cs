using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
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

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterService([FromBody] CreateClientDTO clientDTO)
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
                guid = await systemFacade.RegisterClientAsync(client);
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(guid, ApiVersionEnum.V1);
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


        // public IActionResult DisableClient()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult EnableClient()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult DeleteClient()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult GetClientStatusType()
        // {
        //     throw new NotImplementedException();
        // }


        // public IActionResult RegisterClient()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult EditClient()
        // {
        //     throw new NotImplementedException();
        // }



    }
}
