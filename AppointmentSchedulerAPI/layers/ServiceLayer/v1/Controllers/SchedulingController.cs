using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    public class SchedulingController : ControllerBase
    {
        private readonly ISchedulingInterfaces systemFacade;
        private readonly IHttpResponseService httpResponseService;

        public SchedulingController(ISchedulingInterfaces systemFacade, IHttpResponseService httpResponseService)
        {
            this.systemFacade = systemFacade;
            this.httpResponseService = httpResponseService;
        }

        // public IActionResult ChangeAppointmentStatus()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult GetAppointmentDetails()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult GetAppointment()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult GetAvailableServices()
        // {
        //     throw new NotImplementedException();
        // }

        [HttpPost("asClient")]
        [AllowAnonymous]
        public IActionResult ScheduleAppointmentAsClient()
        {
            throw new NotImplementedException();
        }

        // public IActionResult ScheduleAppointmentAsStaff()
        // {
        //     throw new NotImplementedException();
        // }

        [HttpPost("assign")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAvailabilityTimeSlot([FromBody] CreateAvailabilityTimeSlotDTO availabilityDTO)
        {
            Guid? guid;
            try
            {
                BusinessLogicLayer.Model.AvailabilityTimeSlot availabilityTimeSlot = new()
                {
                    Date = availabilityDTO.Date,
                    EndTime = availabilityDTO.EndTime,
                    StartTime = availabilityDTO.StartTime
                };
                guid = await systemFacade.RegisterAvailabilityTimeSlotAsync(availabilityTimeSlot, availabilityDTO.AssistantUuid);
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(guid, ApiVersionEnum.V1);
        }


        [HttpGet("services/available")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAvailableServicesByDate([FromQuery] GetAvailableServicesByDateDTO getByDateDTO)
        {
            List<AssistantServiceDTO> assistantServiceDTO = [];
            try
            {
                var assistantsAvailable = await systemFacade.GetAvailableServicesClientAsync(getByDateDTO.Date);

                assistantServiceDTO = assistantsAvailable.Select(a => new AssistantServiceDTO
                {
                    Assistant = new AssistantDTO
                    {
                        Uuid = a.Assistant?.Uuid,
                        Name = a.Assistant?.Name,
                    },
                    Services = a.Services?.Select(service => new ServiceDTO
                    {
                        Uuid = service.Uuid,
                        Name = service.Name,
                        Price = service.Price,
                        Minutes = service.Minutes
                    }).ToList()
                }).ToList();
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1); ;
            }
            return httpResponseService.OkResponse(assistantServiceDTO, ApiVersionEnum.V1);
        }


        [HttpGet("availabilityTimeSlot")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAvailabilityTimeslot([FromQuery] DateOnlyRangeDTO rangeDTO)
        {
            List<AvailabilityTimeSlotDTO> availabilityTimeslotsDTO = [];
            try
            {
                var availabilityTimeSlots = await systemFacade.GetAllAvailabilityTimeSlots(rangeDTO.StartDate, rangeDTO.EndDate);

                availabilityTimeslotsDTO = availabilityTimeSlots.Select(a => new AvailabilityTimeSlotDTO
                {
                    Date = a.Date,
                    EndTime = a.EndTime,
                    StartTime = a.StartTime,
                    Uuid = a.Uuid
                    
                }).ToList();
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1); ;
            }
            return httpResponseService.OkResponse(availabilityTimeslotsDTO, ApiVersionEnum.V1);
        }


        // public IActionResult EditAppointment()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult FinalizeAppointment()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult CancelAppointClientSelf()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult CancelAppointmentStaffAssisted()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult DeleteAvailabilityTimeSlot()
        // {
        //     throw new NotImplementedException();
        // }

        // public IActionResult EditAvailabilityTimeSlot()
        // {
        //     throw new NotImplementedException();
        // }
    }
}
