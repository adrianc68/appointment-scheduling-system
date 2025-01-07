using System.ComponentModel.DataAnnotations;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeRangeLock.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
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


        [HttpGet("appointment/range")]
        public IActionResult GetSchedulingBlocks([FromQuery] DateOnlyDTO dto)
        {
            DateOnly date = dto.Date;
            OperationResult<List<SchedulingBlock>, GenericError> result = systemFacade.GetSchedulingBlockRanges(date);
            if (!result.IsSuccessful)
            {
                if (result.Errors != null && result.Errors.Any())
                {
                    return httpResponseService.Conflict(result.Errors, ApiVersionEnum.V1, result.Code.ToString());
                }
                return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());

            }
            List<SchedulingBlockDTO> rangesBlocked = result.Result!.Select(slot => new SchedulingBlockDTO
            {
                Range = slot.Range,
                Services = slot.Services,
                LockEndtime = slot.LockEndTime
            }).ToList();
            return httpResponseService.OkResponse(rangesBlocked, ApiVersionEnum.V1);
        }


        [HttpPost("appointment/range/block")]
        [AllowAnonymous]
        public async Task<IActionResult> BlockTimeRange([FromBody] BlockTimeRangeDTO dto)
        {
            DateTimeRange range = new DateTimeRange
            {
                Date = dto.Date,
            };

            List<ScheduledService> services = dto.SelectedServices.Select(service => new ScheduledService
            {
                Uuid = service.Uuid,
                ServiceStartTime = service.StartTime
            }).ToList();


            OperationResult<DateTime, GenericError> result = await systemFacade.BlockTimeRange(services, range, dto.ClientUuid);
            if (!result.IsSuccessful)
            {
                if (result.Errors != null && result.Errors.Any())
                {
                    return httpResponseService.Conflict(result.Errors, ApiVersionEnum.V1, result.Code.ToString());
                }
                return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());

            }
            return httpResponseService.OkResponse(result.Result, ApiVersionEnum.V1);
        }

        [HttpDelete("appointment/range/unblock")]
        [AllowAnonymous]
        public IActionResult UnblockTimeRange([FromBody] UnblockTimeRangeDTO dto)
        {
            OperationResult<bool, GenericError> result = systemFacade.UnblockTimeRange(dto.ClientUuid);
            if (!result.IsSuccessful)
            {
                return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());
            }
            return httpResponseService.OkResponse(result.Result, ApiVersionEnum.V1);
        }



        [HttpPut("availabilityTimeSlot")]
        [AllowAnonymous]
        public async Task<IActionResult> EditAvailabilityTimeSlot([FromBody] UpdateAvailabilityTimeSlotDTO dto)
        {
            bool isUpdated = false;
            try
            {
                AvailabilityTimeSlot availabilityTimeSlot = new()
                {
                    Uuid = dto.Uuid,
                    Date = dto.Date,
                    EndTime = dto.EndTime,
                    StartTime = dto.StartTime
                };

                OperationResult<bool, GenericError> result = await systemFacade.EditAvailabilityTimeSlot(availabilityTimeSlot);
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

        [HttpDelete("availabilityTimeSlot")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteAvailabilityTimeSlot([FromBody] DeleteAvailabilityTimeSlotDTO dto)
        {
            bool isStatusChanged = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.DeleteAvailabilityTimeSlotAsync(dto.Uuid);
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
            List<ServiceOfferDTO> assistantServiceDTO = [];
            try
            {
                var assistantsAvailable = await systemFacade.GetAvailableServicesClientAsync(getByDateDTO.Date);

                assistantServiceDTO = assistantsAvailable.Select(a => new ServiceOfferDTO
                {
                    AssistantUuid = a.Assistant!.Uuid!.Value,
                    AssistantName = a.Assistant!.Name,
                    ServiceUuid = a.Uuid,
                    ServiceName = a.Service!.Name,
                    ServicePrice = a.Service!.Price,
                    ServiceMinutes = a.Service!.Minutes,
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
                var availabilityTimeSlots = await systemFacade.GetAllAvailabilityTimeSlotsAsync(rangeDTO.StartDate, rangeDTO.EndDate);

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


        [HttpPatch("appointment/serviceOffer/disable")]
        [AllowAnonymous]
        public async Task<IActionResult> DisableServiceOffer([FromBody] DisableServiceOfferDTO dto)
        {
            bool isDisabled = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.DisableServiceOfferAsync(dto.Uuid);
                if (result.IsSuccessful)
                {
                    isDisabled = result.Result;
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
            return httpResponseService.OkResponse(isDisabled, ApiVersionEnum.V1);
        }

        [HttpPatch("appointment/serviceOffer/enable")]
        [AllowAnonymous]
        public async Task<IActionResult> EnableServiceOffer([FromBody] EnableServiceOfferDTO dto)
        {
            bool isEnabled = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.EnableServiceOfferAsync(dto.Uuid);
                if (result.IsSuccessful)
                {
                    isEnabled = result.Result;
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
            return httpResponseService.OkResponse(isEnabled, ApiVersionEnum.V1);
        }


        [HttpGet("appointment/")]
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
                    Assistants = app.ScheduledServices!.Select(se => new AsisstantOfferDTO
                    {
                        AssistantName = se.ServiceOffer!.Assistant!.Name,
                        AssistantUuid = se.ServiceOffer!.Assistant!.Uuid!.Value,
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
                List<Appointment> result = await systemFacade.GetAllAppoinmentsAsync(dto.StartDate, dto.EndDate);
                appointments = result.Select(app => new AppointmentDTO
                {
                    Uuid = app.Uuid,
                    StartTime = app.StartTime,
                    EndTime = app.EndTime,
                    Date = app.Date,
                    TotalCost = app.TotalCost,
                    Status = app.Status.ToString(),
                    CreatedAt = app.CreatedAt!.Value,
                    Client = new ClientDetailsDTO
                    {
                        Name = app.Client!.Name,
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


        [HttpPost("appointment/asClient")]
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
                    ScheduledServices = [],
                    Uuid = Guid.CreateVersion7()
                };
                appointment.StartTime = dto.SelectedServices.Min(service => service.StartTime);
                foreach (var serviceOfferUuid in dto.SelectedServices)
                {
                    var selectedService = new ScheduledService
                    {
                        Uuid = serviceOfferUuid.Uuid,
                        ServiceStartTime = serviceOfferUuid.StartTime
                    };
                    appointment.ScheduledServices!.Add(selectedService);
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

        [HttpPost("appointment/asStaff")]
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
                    ScheduledServices = [],
                    Uuid = Guid.CreateVersion7()
                };
                appointment.StartTime = dto.SelectedServices.Min(service => service.StartTime);
                foreach (var serviceOfferUuid in dto.SelectedServices)
                {
                    var selectedService = new ScheduledService
                    {
                        Uuid = serviceOfferUuid.Uuid,
                        ServiceStartTime = serviceOfferUuid.StartTime
                    };
                    appointment.ScheduledServices!.Add(selectedService);
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

        [HttpGet("appointment/conflict")]
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
                    AssistantName = a.ServiceOffer!.Assistant!.Name,
                    AssistantUuid = a.ServiceOffer!.Assistant!.Uuid!.Value
                }).ToList();
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1); ;
            }
            return httpResponseService.OkResponse(conflictingServiceOfferDTOs, ApiVersionEnum.V1);
        }

        [HttpPatch("appointment/confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmAppointment([FromBody] ConfirmAppointmentDTO dto)
        {
            bool isConfirmed = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.ConfirmAppointment(dto.Uuid);
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

        [HttpPatch("appointment/finalize")]
        [AllowAnonymous]
        public async Task<IActionResult> FinalizeAppointment([FromBody] FinalizeAppointmentDTO dto)
        {
            bool isConfirmed = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.FinalizeAppointmentAsync(dto.Uuid);
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

        [HttpPatch("appointment/cancel/asClient")]
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

        [HttpPatch("appointment/cancel/asStaff")]
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
