using System.ComponentModel.DataAnnotations;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.TimeSlotLock.Model;
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
            OperationResult<List<BlockedTimeSlot>, GenericError> result = systemFacade.GetSchedulingBlockRanges(date);
            if (!result.IsSuccessful)
            {
                if (result.Errors != null && result.Errors.Any())
                {
                    return httpResponseService.Conflict(result.Errors, ApiVersionEnum.V1, result.Code.ToString());
                }
                return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());

            }
            List<BlockedServiceTimeSlotDTO> rangesBlocked = result.Result!.Select(slot => new BlockedServiceTimeSlotDTO
            {
                BlockedRange = slot.TotalServicesTimeRange,
                BlockedServices = slot.SelectedServices.Select(blockedService => new AssistantServiceBlockedTimeSlotDataDTO 
                {
                    Assistant = new AssistantBlockedTimeSlotDTO 
                    {
                        Uuid = blockedService.AssistantUuid,
                    },
                    Service = new ServiceBlockedTimeSlotDTO
                    {
                        Uuid = blockedService.ServiceUuid,
                        StartTime = blockedService.StartTime,
                        EndTime = blockedService.EndTime
                    }
                }).ToList(),
                LockExpirationTime = slot.LockExpirationTime
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
                StartTime = dto.SelectedServices.Min(a => a.StartTime)
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
                    StartTime = dto.StartTime,
                    UnavailableTimeSlots = dto.UnavailableTimeSlots?.Select(una => new UnavailableTimeSlot
                    {
                        StartTime = una.StartTime,
                        EndTime = una.EndTime,
                    }).ToList()
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
                    },
                    UnavailableTimeSlots = availabilityDTO.UnavailableTimeSlots?.Select(e => new UnavailableTimeSlot
                    {
                        StartTime = e.StartTime,
                        EndTime = e.EndTime
                    }).ToList()
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
            List<ServiceAvailableDTO> assistantServiceDTO = [];
            try
            {
                var assistantsAvailable = await systemFacade.GetAvailableServicesClientAsync(getByDateDTO.Date);

                assistantServiceDTO = assistantsAvailable.Select(a => new ServiceAvailableDTO
                {
                    Assistant = new AssistantServiceOfferDTO
                    {
                        Uuid = a.Assistant!.Uuid!.Value,
                        Name = a.Assistant!.Name,
                    },
                    Service = new ServiceOfferDTO
                    {
                        Uuid = a.Uuid,
                        Name = a.Service!.Name,
                        Price = a.Service!.Price,
                        Minutes = a.Service!.Minutes,
                        Description = a.Service!.Description
                    },

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
            List<AvailabilityTimeSlotRegisteredDTO> availabilityTimeslotsDTO = [];
            try
            {
                var availabilityTimeSlots = await systemFacade.GetAllAvailabilityTimeSlotsAsync(rangeDTO.StartDate, rangeDTO.EndDate);

                availabilityTimeslotsDTO = availabilityTimeSlots.Select(a => new AvailabilityTimeSlotRegisteredDTO
                {
                    Assistant = new AssistantAvailabilityTimeSlotDTO
                    {
                        Uuid = a.Assistant!.Uuid,
                        Name = a.Assistant.Name
                    },
                    AvailabilityTimeSlot = new AvailabilityTimeSlotDTO
                    {
                        Date = a.Date,
                        EndTime = a.EndTime,
                        StartTime = a.StartTime,
                        Uuid = a.Uuid,
                    }
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
            List<AppointmentDetailsDTO> appointments = [];
            try
            {
                List<Appointment> result = await systemFacade.GetScheduledOrConfirmedAppoinmentsAsync(dto.StartDate, dto.EndDate);
                appointments = result.Select(app => new AppointmentDetailsDTO
                {
                    Appointment = new AppointmentDTO
                    {
                        Uuid = app.Uuid,
                        StartTime = app.StartTime,
                        EndTime = app.EndTime,
                        Date = app.Date,
                        Status = app.Status.ToString(),
                        CreatedAt = app.CreatedAt!.Value,
                    },
                    Assistants = app.ScheduledServices!.Select(se => new AsisstantOfferDTO
                    {
                        Assistant = new AssistantOccupiedServiceOfferDTO
                        {
                            Name = se.ServiceOffer!.Assistant!.Name,
                            Uuid = se.ServiceOffer!.Assistant.Uuid!.Value,
                        },
                        OccupiedTimeRange = new ServiceOfferRangeDTO
                        {
                            StartTime = se.ServiceStartTime!.Value,
                            EndTime = se.ServiceEndTime!.Value
                        }
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
            List<AppointmentDetailsDTO> appointments = [];
            try
            {
                List<Appointment> result = await systemFacade.GetAllAppoinmentsAsync(dto.StartDate, dto.EndDate);
                appointments = result.Select(app => new AppointmentDetailsDTO
                {
                    Appointment = new AppointmentDTO
                    {
                        Uuid = app.Uuid,
                        StartTime = app.StartTime,
                        EndTime = app.EndTime,
                        Date = app.Date,
                        TotalCost = app.TotalCost,
                        Status = app.Status.ToString(),
                        CreatedAt = app.CreatedAt!.Value,
                    },
                    Client = new ClientAppointmentDTO
                    {
                        Name = app.Client!.Name,
                        Uuid = app.Client.Uuid,
                        PhoneNumber = app.Client.PhoneNumber,
                        Email = app.Client.Email,
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
            Appointment app;
            AppointmentFullDetailsDTO appointmentDto;
            try
            {
                OperationResult<Appointment, GenericError> result = await systemFacade.GetAppointmentDetailsAsync(uuid);
                if (result.IsSuccessful)
                {
                    app = result.Result!;
                    appointmentDto = new AppointmentFullDetailsDTO
                    {
                        Client = new ClientAppointmentDTO
                        {
                            Name = app.Client!.Name,
                            Uuid = app.Client.Uuid,
                            PhoneNumber = app.Client.PhoneNumber,
                            Email = app.Client.Email,
                        },
                        Appointment = new AppointmentDTO
                        {
                            Uuid = app.Uuid,
                            StartTime = app.StartTime,
                            EndTime = app.EndTime,
                            Date = app.Date,
                            TotalCost = app.TotalCost,
                            Status = app.Status.ToString(),
                            CreatedAt = app.CreatedAt!.Value,
                        },
                        ScheduledServices = app.ScheduledServices!.Select(serviceSelected => new AppointmentServiceDTO
                        {

                            Assistant = new AssistantScheduledServiceDTO
                            {
                                Name = serviceSelected.ServiceOffer!.Assistant!.Name,
                                Uuid = serviceSelected.ServiceOffer!.Assistant.Uuid,
                                PhoneNumber = serviceSelected.ServiceOffer!.Assistant.PhoneNumber,
                                Email = serviceSelected.ServiceOffer.Assistant!.Email
                            },
                            Service = new ScheduledServiceDTO
                            {
                                StartTime = serviceSelected.ServiceStartTime!.Value,
                                EndTime = serviceSelected.ServiceEndTime!.Value,
                                Price = serviceSelected.ServicePrice,
                                Minutes = serviceSelected.ServicesMinutes,
                                Name = serviceSelected.ServiceName,
                                Uuid = serviceSelected.ServiceOffer.Uuid!.Value
                            }
                        }).ToList()

                    };
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
            return httpResponseService.OkResponse(appointmentDto, ApiVersionEnum.V1);
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
                    ScheduledService = new ConflictingServiceOfferDataDTO
                    {
                        Uuid = a.Uuid!.Value
                    },
                    Assistant = new AssistantConflictingAppointmentDTO
                    {
                        Name = a.ServiceOffer!.Assistant!.Name,
                        Uuid = a.ServiceOffer!.Assistant!.Uuid!.Value
                    },
                    TimeRange = new ConflictingAppointmentTimeRangeDTO 
                    {
                        StartTime = a.ServiceStartTime!.Value,
                        EndTime = a.ServiceEndTime!.Value
                    }
 
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
