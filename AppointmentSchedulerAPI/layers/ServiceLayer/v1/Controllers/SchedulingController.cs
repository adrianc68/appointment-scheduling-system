using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
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

        [HttpPost("appointment")]
        [AllowAnonymous]
        public async Task<IActionResult> ScheduleAppointmentAsClientAsync([FromBody] CreateAppointmentAsClientDTO dto)
        {
            Guid? guid;
            try
            {
                Appointment appointment = new Appointment
                {
                    // StartTime = dto.StartTime,
                    Date = dto.Date,
                    Client = new Client { Uuid = dto.ClientUuid },
                    ServiceOffers = [],
                    Uuid = Guid.CreateVersion7()
                };

                var selectedServicesStartTimes = dto.SelectedServices
                .Select(service => service.StartTime)
                .ToList();
                appointment.StartTime = selectedServicesStartTimes.Min(); 

                foreach (var serviceOfferUuid in dto.SelectedServices)
                {
                    var serviceOffers = new ServiceOffer
                    {
                        Uuid = serviceOfferUuid.Uuid,
                        StartTime = serviceOfferUuid.StartTime
                    };
                    PropToString.PrintData(serviceOffers);
                    appointment.ServiceOffers.Add(serviceOffers);
                }
                OperationResult<Guid?> result = await systemFacade.ScheduleAppointmentAsClientAsync(appointment);
                if (result.IsSuccessful)
                {
                    guid = result.Result;
                }
                else
                {
                    return httpResponseService.Conflict(result.Result!.Value, ApiVersionEnum.V1, result.Code.ToString());
                }

            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(guid, ApiVersionEnum.V1);
        }

        // public IActionResult ScheduleAppointmentAsStaff()
        // {
        //     throw new NotImplementedException();
        // }

        [HttpPost("availabilityTimeSlot")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAvailabilityTimeSlot([FromBody] CreateAvailabilityTimeSlotDTO availabilityDTO)
        {
            Guid? guid;
            try
            {
                AvailabilityTimeSlot availabilityTimeSlot = new()
                {
                    Date = availabilityDTO.Date,
                    EndTime = availabilityDTO.EndTime,
                    StartTime = availabilityDTO.StartTime,
                    Assistant = new Assistant
                    {
                        Uuid = availabilityDTO.AssistantUuid
                    }
                };
                OperationResult<Guid?> result = await systemFacade.RegisterAvailabilityTimeSlotAsync(availabilityTimeSlot);
                if (result.IsSuccessful)
                {
                    guid = result.Result;
                }
                else
                {
                    return httpResponseService.Conflict(ApiVersionEnum.V1, result.Code.ToString());
                }
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(guid, ApiVersionEnum.V1);
        }


        [HttpGet("services/available")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAvailableServicesByDate([FromQuery] AvailableServicesByDateDTO getByDateDTO)
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
        public async Task<IActionResult> GetAllAvailabilityTimeSlot([FromQuery] DateOnlyRangeDTO rangeDTO)
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
                    Uuid = a.Uuid,
                    AssistantUuid = a.Assistant!.Uuid,
                    AssistantName = a.Assistant.Name

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
