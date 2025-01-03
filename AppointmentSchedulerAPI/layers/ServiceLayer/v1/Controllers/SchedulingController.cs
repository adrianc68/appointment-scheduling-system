using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
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
    public class SchedulingController : ControllerBase
    {
        private readonly ISchedulingInterfaces systemFacade;
        private readonly IHttpResponseService httpResponseService;

        public SchedulingController(ISchedulingInterfaces systemFacade, IHttpResponseService httpResponseService)
        {
            this.systemFacade = systemFacade;
            this.httpResponseService = httpResponseService;
        }

        // public IActionResult EditAppointment()
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
                OperationResult<Guid, GenericError> result = await systemFacade.RegisterAvailabilityTimeSlotAsync(availabilityTimeSlot);
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


        [HttpGet("appointment/scheduler")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAppointmentsFromScheduler([FromQuery] GetAllAppointmentsDTO dto)
        {
            List<AppointmentDTO> appointments = [];
            try
            {
                List<Appointment> result = await systemFacade.GetScheduledOrConfirmedAppoinmentsAsync(dto.StartDate, dto.EndDate);
                appointments = result.Select(app => new AppointmentDTO
                {
                    Uuid = app.Uuid,
                    StartTime = app.StartTime,
                    EndTime = app.EndTime,
                    Date = app.Date,
                    Status = app.Status.ToString(),
                    CreatedAt = app.CreatedAt!.Value,
                    Assistants = app.ServiceOffers.Select(se => new AsisstantOfferDTO
                    {
                        AssistantName = se.Assistant!.Name,
                        AssistantUuid = se.Assistant!.Uuid!.Value,
                        StartTimeOfAssistantOfferingService = se.ServiceStartTime!.Value,
                        EndTimeOfAsisstantOfferingService = se.ServiceEndTime!.Value
                    }).ToList()
                }).ToList();
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(appointments, ApiVersionEnum.V1);
        }

        [HttpGet("appointment/staffOnly")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAppoinments([FromQuery] GetAllAppointmentsDTO dto)
        {
            List<AppointmentDTO> appointments = [];
            try
            {
                List<Appointment> result = await systemFacade.GetAllAppoinments(dto.StartDate, dto.EndDate);
                appointments = result.Select(app => new AppointmentDTO
                {
                    Uuid = app.Uuid,
                    StartTime = app.StartTime,
                    EndTime = app.EndTime,
                    Date = app.Date,
                    TotalCost = app.TotalCost,
                    Status = app.Status.ToString(),
                    CreatedAt = app.CreatedAt!.Value,
                    Client = new ClientDTO
                    {
                        Name = app.Client.Name,
                        Uuid = app.Client.Uuid,
                        PhoneNumber = app.Client.PhoneNumber,
                        Email = app.Client.Email,
                        Status = app.Client.Status!.Value.ToString(),
                        Username = app.Client.Username,
                        CreatedAt = app.Client.CreatedAt
                    }
                }).ToList();
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(appointments, ApiVersionEnum.V1);
        }

        [HttpGet("appointment/staffOnly/{uuid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAppointmentsDetails(Guid uuid)
        {
            Appointment appointment;
            try
            {
                OperationResult<Appointment, GenericError> result = await systemFacade.GetAppointmentDetailsAsync(uuid);
                if (result.IsSuccessful)
                {
                    appointment = result.Result!;
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
            return httpResponseService.OkResponse(appointment, ApiVersionEnum.V1);
        }


        [HttpPost("appointment/scheduler/asClient")]
        [AllowAnonymous]
        public async Task<IActionResult> ScheduleAppointmentAsClientAsync([FromBody] CreateAppointmentAsClientDTO dto)
        {
            Guid? guid;
            try
            {
                Appointment appointment = new Appointment
                {
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
                        ServiceStartTime = serviceOfferUuid.StartTime
                    };
                    appointment.ServiceOffers.Add(serviceOffers);
                }
                OperationResult<Guid, GenericError> result = await systemFacade.ScheduleAppointmentAsClientAsync(appointment);
                if (result.IsSuccessful)
                {
                    guid = result.Result;
                }
                else
                {
                    if (result.Errors != null && result.Errors.Any())
                    {
                        return httpResponseService.Conflict(result.Errors, ApiVersionEnum.V1, result.Code.ToString());
                    }
                    return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());
                }

            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(guid, ApiVersionEnum.V1);
        }

        [HttpPost("appointment/scheduler/asStaff")]
        [AllowAnonymous]
        public async Task<IActionResult> ScheduleAppointmentAsStaff([FromBody] CreateAppointmentAsStaffDTO dto)
        {
            Guid? guid;
            try
            {
                Appointment appointment = new Appointment
                {
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
                        ServiceStartTime = serviceOfferUuid.StartTime
                    };
                    appointment.ServiceOffers.Add(serviceOffers);
                }
                OperationResult<Guid, GenericError> result = await systemFacade.ScheduleAppointmentAsStaffAsync(appointment);
                if (result.IsSuccessful)
                {
                    guid = result.Result;
                }
                else
                {
                    if (result.Errors != null && result.Errors.Any())
                    {
                        return httpResponseService.Conflict(result.Errors, ApiVersionEnum.V1, result.Code.ToString());
                    }
                    return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());
                }

            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(guid, ApiVersionEnum.V1);
        }

        [HttpGet("appointment/scheduler/conflict")]
        [AllowAnonymous]
        public async Task<IActionResult> GetConflictingServiceAppointmentFromRange([FromQuery] DateTimeRangeDTO rangeDTO)
        {
            List<ConflictingServiceOfferDTO> conflictingServiceOfferDTOs = [];
            try
            {
                DateTimeRange range = new DateTimeRange(rangeDTO.Date, rangeDTO.StartTime, rangeDTO.EndTime);
                var conflictingServiceOffers = await systemFacade.GetConflictingServicesByDateTimeRangeAsync(range);
                conflictingServiceOfferDTOs = conflictingServiceOffers.Select(a => new ConflictingServiceOfferDTO
                {
                    ConflictingServiceOfferUuid = a.Uuid,
                    StartTime = a.ServiceStartTime,
                    EndTime = a.ServiceEndTime,
                    AssistantName = a.Assistant!.Name,
                    AssistantUuid = a.Assistant!.Uuid!.Value
                }).ToList();
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1); ;
            }
            return httpResponseService.OkResponse(conflictingServiceOfferDTOs, ApiVersionEnum.V1);
        }

        [HttpPost("appointment/scheduler/confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmAppointment([FromBody] ConfirmAppointmentDTO dto)
        {
            bool isConfirmed = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.ConfirmAppointment(dto.AppointmentUuid);
                if (result.IsSuccessful)
                {
                    isConfirmed = result.Result;
                }
                else
                {
                    return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());
                }
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
                throw;
            }
            return httpResponseService.OkResponse(isConfirmed, ApiVersionEnum.V1);
        }

        [HttpPost("appointment/scheduler/finalize")]
        [AllowAnonymous]
        public async Task<IActionResult> FinalizeAppointment([FromBody] FinalizeAppointmentDTO dto)
        {
            bool isConfirmed = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.FinalizeAppointment(dto.AppointmentUuid);
                if (result.IsSuccessful)
                {
                    isConfirmed = result.Result;
                }
                else
                {
                    return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());
                }
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
                throw;
            }
            return httpResponseService.OkResponse(isConfirmed, ApiVersionEnum.V1);
        }

        [HttpPost("appointment/scheduler/cancel/asClient")]
        [AllowAnonymous]
        public async Task<IActionResult> CancelAppointment([FromBody] CancelAppointmentAsClientDTO dto)
        {
            bool isConfirmed = false;
            try
            {
                // $$$> Get client uuid from authentication service
                OperationResult<bool, GenericError> result = await systemFacade.CancelAppointmentClientSelf(dto.AppointmentUuid, dto.ClientUuid);
                if (result.IsSuccessful)
                {
                    isConfirmed = result.Result;
                }
                else
                {
                    return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());
                }
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
                throw;
            }
            return httpResponseService.OkResponse(isConfirmed, ApiVersionEnum.V1);
        }

        [HttpPost("appointment/scheduler/cancel/asStaff")]
        [AllowAnonymous]
        public async Task<IActionResult> CancelAppointmentAsStaff([FromBody] CancelAppointmentAsStaffDTO dto)
        {
            bool isConfirmed = false;
            try
            {
                // $$$> Get client uuid from authentication service
                OperationResult<bool, GenericError> result = await systemFacade.CancelAppointmentStaffAssisted(dto.AppointmentUuid);
                if (result.IsSuccessful)
                {
                    isConfirmed = result.Result;
                }
                else
                {
                    return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());
                }
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
                throw;
            }
            return httpResponseService.OkResponse(isConfirmed, ApiVersionEnum.V1);
        }
    }
}
