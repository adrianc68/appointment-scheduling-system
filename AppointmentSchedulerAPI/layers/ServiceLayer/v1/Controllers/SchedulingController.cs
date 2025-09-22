using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AccountInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Authorization.Attributes;
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
        private readonly IAccountInterfaces acountSystemFacade;
        private readonly IHttpResponseService httpResponseService;

        public SchedulingController(ISchedulingInterfaces systemFacade, IAccountInterfaces accountSystemFacade, IHttpResponseService httpResponseService)
        {
            this.systemFacade = systemFacade;
            this.acountSystemFacade = accountSystemFacade;
            this.httpResponseService = httpResponseService;
        }


        [HttpGet("appointment/range")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR, RoleType.ASSISTANT, RoleType.CLIENT)]
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
                        StartDate = blockedService.StartDate,
                        EndDate = blockedService.EndDate
                    }
                }).ToList(),
                LockExpirationTime = slot.LockExpirationTime
            }).ToList();
            return httpResponseService.OkResponse(rangesBlocked, ApiVersionEnum.V1);
        }

        [HttpGet("appointment/client")]
        [Authorize]
        [AllowedRoles(RoleType.CLIENT)]
        public async Task<IActionResult> GetAppointmentOfClient([FromQuery] GetAllAppointmentsDTO dto)
        {
            var claims = ClaimsPOCO.GetUserClaims(User);
            List<AppointmentDetailsDTO> appointments = [];
            try
            {
                List<Appointment> result = await systemFacade.GetAppointmentsOfUserByUuidAndRange(dto.StartDate, dto.EndDate, claims.Uuid);
                appointments = result.Select(app => new AppointmentDetailsDTO
                {
                    Uuid = app.Uuid ?? Guid.Empty,
                    StartDate = app.StartDate,
                    EndDate = app.EndDate,
                    Status = app.Status,
                    TotalCost = app.TotalCost,
                    CreatedAt = app.CreatedAt ?? DateTime.MinValue,
                    Client = new ClientAppointmentDTO
                    {
                        Uuid = app.Client?.Uuid,
                        Name = app.Client?.Name,
                        PhoneNumber = app.Client?.PhoneNumber,
                        Email = app.Client?.Email,
                        Username = app.Client?.Username
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
                            StartDate = serviceSelected.ServiceStartDate!.Value,
                            EndDate = serviceSelected.ServiceEndDate!.Value,
                            Price = serviceSelected.ServicePrice,
                            Minutes = serviceSelected.ServicesMinutes,
                            Name = serviceSelected.ServiceName,
                            Uuid = serviceSelected.ServiceOffer.Uuid!.Value
                        }
                    }).ToList(),
                }).ToList();
            }

            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(appointments, ApiVersionEnum.V1);
        }

        [HttpGet("appointment/client/all")]
        [Authorize]
        [AllowedRoles(RoleType.CLIENT)]
        public async Task<IActionResult> GetAllAppointmentOfClient()
        {
            var claims = ClaimsPOCO.GetUserClaims(User);
            List<AppointmentDetailsDTO> appointments = [];
            try
            {
                List<Appointment> result = await systemFacade.GetAppointmentsOfUserByUuid(claims.Uuid);
                appointments = result.Select(app => new AppointmentDetailsDTO
                {
                    Uuid = app.Uuid ?? Guid.Empty,
                    StartDate = app.StartDate,
                    EndDate = app.EndDate,
                    Status = app.Status,
                    TotalCost = app.TotalCost,
                    CreatedAt = app.CreatedAt ?? DateTime.MinValue,
                    Client = new ClientAppointmentDTO
                    {
                        Uuid = app.Client?.Uuid,
                        Name = app.Client?.Name,
                        PhoneNumber = app.Client?.PhoneNumber,
                        Email = app.Client?.Email,
                        Username = app.Client?.Username
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
                            StartDate = serviceSelected.ServiceStartDate!.Value,
                            EndDate = serviceSelected.ServiceEndDate!.Value,
                            Price = serviceSelected.ServicePrice,
                            Minutes = serviceSelected.ServicesMinutes,
                            Name = serviceSelected.ServiceName,
                            Uuid = serviceSelected.ServiceOffer.Uuid!.Value
                        }
                    }).ToList(),
                }).ToList();
            }

            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(appointments, ApiVersionEnum.V1);
        }

        [HttpDelete("appointment/range/unblock")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR, RoleType.ASSISTANT, RoleType.CLIENT)]
        public IActionResult UnblockTimeRange()
        {
            var claims = ClaimsPOCO.GetUserClaims(User);
            OperationResult<bool, GenericError> result = systemFacade.UnblockTimeRange(claims.Uuid);
            if (!result.IsSuccessful)
            {
                return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());
            }
            return httpResponseService.OkResponse(result.Result, ApiVersionEnum.V1);
        }

        [HttpPut("availabilityTimeSlot")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> EditAvailabilityTimeSlot([FromBody] UpdateAvailabilityTimeSlotDTO dto)
        {
            bool isUpdated = false;
            try
            {
                AvailabilityTimeSlot availabilityTimeSlot = new()
                {
                    Uuid = dto.Uuid,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    UnavailableTimeSlots = dto.UnavailableTimeSlots?.Select(una => new UnavailableTimeSlot
                    {
                        StartDate = una.StartDate,
                        EndDate = una.EndDate,
                    }).ToList()
                };

                OperationResult<bool, GenericError> result = await systemFacade.EditAvailabilityTimeSlotAsync(availabilityTimeSlot);
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

        [HttpDelete("availabilityTimeSlot/delete")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> DeleteAvailabilityTimeSlot([FromQuery] DeleteAvailabilityTimeSlotDTO dto)
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


        [HttpPatch("availabilityTimeSlot/disable")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> DisableAvailabilityTimeSlot([FromBody] DisableAvailabilityTimeSlotDTO dto)
        {
            bool isStatusChanged = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.DisableAvailabilityTimeSlotAsync(dto.Uuid);
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

        [HttpPatch("availabilityTimeSlot/enable")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> EnableAvailabilityTimeSlot([FromBody] EnableAvailabilityTimeSlotDTO dto)
        {
            bool isStatusChanged = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.EnableAvailabilityTimeSlotAsync(dto.Uuid);
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
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> RegisterAvailabilityTimeSlot([FromBody] CreateAvailabilityTimeSlotDTO availabilityDTO)
        {
            Guid? guid;
            try
            {
                AvailabilityTimeSlot availabilityTimeSlot = new()
                {
                    EndDate = availabilityDTO.EndDate,
                    StartDate = availabilityDTO.StartDate,
                    Assistant = new Assistant
                    {
                        Uuid = availabilityDTO.AssistantUuid
                    },
                    UnavailableTimeSlots = availabilityDTO.UnavailableTimeSlots?.Select(e => new UnavailableTimeSlot
                    {
                        StartDate = e.StartDate,
                        EndDate = e.EndDate
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
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR, RoleType.ASSISTANT, RoleType.CLIENT)]
        public async Task<IActionResult> GetAllAvailableServicesByDate([FromQuery] AvailableServicesByDateDTO getByDateDTO)
        {
            List<ServiceAvailableDTO> assistantServiceDTO = [];
            try
            {
                var assistantsAvailable = await systemFacade.GetAvailableServicesClientAsync(getByDateDTO.Date);

                assistantServiceDTO = assistantsAvailable.Select(a => new ServiceAvailableDTO
                {
                    Uuid = a.Uuid!.Value,
                    Name = a.Service!.Name!,
                    Price = a.Service!.Price!.Value,
                    Minutes = a.Service!.Minutes!.Value,
                    Description = a.Service!.Description!,
                    Assistant = new AssistantServiceOfferDTO
                    {
                        Uuid = a.Assistant!.Uuid!.Value,
                        Name = a.Assistant!.Name,
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
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR, RoleType.ASSISTANT, RoleType.CLIENT)]
        public async Task<IActionResult> GetAllAvailabilityTimeSlot([FromQuery] DateOnlyRangeDTO rangeDTO)
        {
            List<AvailabilityTimeSlotDTO> availabilityTimeslotsDTO = [];
            try
            {
                var availabilityTimeSlots = await systemFacade.GetAllAvailabilityTimeSlotsAsync(rangeDTO.StartDate, rangeDTO.EndDate);

                availabilityTimeslotsDTO = availabilityTimeSlots.Select(a => new AvailabilityTimeSlotDTO
                {
                    Assistant = new AssistantAvailabilityTimeSlotDTO
                    {
                        Uuid = a.Assistant!.Uuid,
                        Name = a.Assistant.Name
                    },
                    UnavailableTimeSlots = a.UnavailableTimeSlots!.Select(una => new DTO.Response.UnavailableTimeSlotDTO
                    {
                        StartDate = una.StartDate,
                        EndDate = una.EndDate
                    }).ToList(),
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    Uuid = a.Uuid!.Value,
                    Status = a.Status
                }).ToList();
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1); ;
            }
            return httpResponseService.OkResponse(availabilityTimeslotsDTO, ApiVersionEnum.V1);
        }


        [HttpPatch("appointment/serviceOffer/disable")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
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
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
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

        [HttpPatch("appointment/serviceOffer/delete")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> DeleteServiceOffer([FromBody] DeleteServiceOfferDTO dto)
        {
            bool isEnabled = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.DeleteServiceOfferAsync(dto.Uuid);
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
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR, RoleType.ASSISTANT, RoleType.CLIENT)]
        public async Task<IActionResult> GetAppointmentsFromScheduler([FromQuery] GetAllAppointmentsDTO dto)
        {
            List<AppointmentDetailsDTO> appointments = [];
            try
            {
                List<Appointment> result = await systemFacade.GetAllAppoinmentsAsync(dto.StartDate, dto.EndDate);
                appointments = result.Select(app => new AppointmentDetailsDTO
                {
                    Uuid = app.Uuid ?? Guid.Empty,
                    StartDate = app.StartDate,
                    EndDate = app.EndDate,
                    Status = app.Status,
                    TotalCost = app.TotalCost,
                    CreatedAt = app.CreatedAt ?? DateTime.MinValue,
                    Client = new ClientAppointmentDTO
                    {
                        Uuid = app.Client?.Uuid,
                        Name = app.Client?.Name,
                        PhoneNumber = app.Client?.PhoneNumber,
                        Email = app.Client?.Email,
                        Username = app.Client?.Username
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
                            StartDate = serviceSelected.ServiceStartDate!.Value,
                            EndDate = serviceSelected.ServiceEndDate!.Value,
                            Price = serviceSelected.ServicePrice,
                            Minutes = serviceSelected.ServicesMinutes,
                            Name = serviceSelected.ServiceName,
                            Uuid = serviceSelected.ServiceOffer.Uuid!.Value
                        }
                    }).ToList(),
                }).ToList();
            }

            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(appointments, ApiVersionEnum.V1);
        }

        [HttpGet("appointment/details")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR, RoleType.ASSISTANT, RoleType.CLIENT)]
        public async Task<IActionResult> GetAppointmentsDetailsFromScheduler([FromQuery] GetAllAppointmentsDTO dto)
        {
            List<AppointmentDetailsDTO> appointments = [];
            try
            {
                List<Appointment> result = await systemFacade.GetScheduledOrConfirmedAppoinmentsAsync(dto.StartDate, dto.EndDate);
                appointments = result.Select(app => new AppointmentDetailsDTO
                {
                    Uuid = app.Uuid ?? Guid.Empty,
                    StartDate = app.StartDate,
                    EndDate = app.EndDate,
                    Status = app.Status,
                    TotalCost = app.TotalCost,
                    CreatedAt = app.CreatedAt ?? DateTime.MinValue,
                    Client = new ClientAppointmentDTO
                    {
                        Uuid = app.Client?.Uuid,
                        Name = app.Client?.Name,
                        PhoneNumber = app.Client?.PhoneNumber,
                        Email = app.Client?.Email,
                        Username = app.Client?.Username
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
                            StartDate = serviceSelected.ServiceStartDate!.Value,
                            EndDate = serviceSelected.ServiceEndDate!.Value,
                            Price = serviceSelected.ServicePrice,
                            Minutes = serviceSelected.ServicesMinutes,
                            Name = serviceSelected.ServiceName,
                            Uuid = serviceSelected.ServiceOffer.Uuid!.Value
                        }
                    }).ToList(),
                }).ToList();
            }

            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(appointments, ApiVersionEnum.V1);
        }


        [HttpGet("appointment/staffOnly")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR, RoleType.ASSISTANT)]
        public async Task<IActionResult> GetAllAppoinments([FromQuery] GetAllAppointmentsDTO dto)
        {
            List<AppointmentDetailsDTO> appointments = [];
            try
            {
                List<Appointment> result = await systemFacade.GetAllAppoinmentsAsync(dto.StartDate, dto.EndDate);
                appointments = result.Select(app => new AppointmentDetailsDTO
                {
                    Uuid = app.Uuid!.Value,
                    StartDate = app.StartDate,
                    EndDate = app.EndDate,
                    TotalCost = app.TotalCost,
                    Status = app.Status,
                    CreatedAt = app.CreatedAt!.Value,
                    Client = new ClientAppointmentDTO
                    {
                        Name = app.Client!.Name,
                        Uuid = app.Client.Uuid,
                        PhoneNumber = app.Client.PhoneNumber,
                        Email = app.Client.Email,
                    },

                }).ToList();
            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(appointments, ApiVersionEnum.V1);
        }

        [HttpGet("appointment/staffOnly/{uuid}")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR, RoleType.ASSISTANT)]
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
                            StartDate = app.StartDate,
                            EndDate = app.EndDate,
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
                                StartDate = serviceSelected.ServiceStartDate!.Value.Date,
                                EndDate = serviceSelected.ServiceEndDate!.Value.Date,
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
        [Authorize]
        [AllowedRoles(RoleType.CLIENT)]
        public async Task<IActionResult> ScheduleAppointmentAsClientAsync([FromBody] CreateAppointmentAsClientDTO dto)
        {
            Guid? guid;

            try
            {
                DateTime startDateTime = dto.SelectedServices
                    .Min(s => DateTime.SpecifyKind(dto.Date.ToDateTime(s.StartTime), DateTimeKind.Utc));

                var claims = ClaimsPOCO.GetUserClaims(User);
                Appointment appointment = new Appointment
                {
                    StartDate = startDateTime,
                    Client = new Client { Uuid = claims.Uuid },
                    ScheduledServices = new List<ScheduledService>(),
                    Uuid = Guid.CreateVersion7()
                };

                foreach (var serviceOffer in dto.SelectedServices)
                {
                    appointment.ScheduledServices!.Add(new ScheduledService
                    {
                        Uuid = serviceOffer.Uuid,
                        ServiceStartDate = DateTime.SpecifyKind(dto.Date.ToDateTime(serviceOffer.StartTime), DateTimeKind.Utc)
                    });
                }

                OperationResult<Guid, GenericError> result = await systemFacade.ScheduleAppointmentAsClientAsync(appointment);

                if (result.IsSuccessful)
                {
                    guid = result.Result;
                }
                else
                {
                    if (result.Errors != null && result.Errors.Any())
                        return httpResponseService.Conflict(result.Errors, ApiVersionEnum.V1, result.Code.ToString());

                    return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());
                }

            }
            catch (Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }

            return httpResponseService.OkResponse(guid, ApiVersionEnum.V1);
        }

        [HttpPost("appointment/asStaff")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR, RoleType.ASSISTANT)]
        public async Task<IActionResult> ScheduleAppointmentAsStaff([FromBody] CreateAppointmentAsStaffDTO dto)
        {
            Guid? guid;
            try
            {
                DateTime startDateTime = dto.SelectedServices
                    .Min(s => DateTime.SpecifyKind(dto.Date.ToDateTime(s.StartTime), DateTimeKind.Utc));

                Appointment appointment = new Appointment
                {
                    StartDate = startDateTime,
                    Client = new Client { Uuid = dto.ClientUuid },
                    ScheduledServices = new List<ScheduledService>(),
                    Uuid = Guid.CreateVersion7()
                };

                foreach (var serviceOffer in dto.SelectedServices)
                {
                    appointment.ScheduledServices!.Add(new ScheduledService
                    {
                        Uuid = serviceOffer.Uuid,
                        ServiceStartDate = DateTime.SpecifyKind(dto.Date.ToDateTime(serviceOffer.StartTime), DateTimeKind.Utc)
                    });
                }

                OperationResult<Guid, GenericError> result = await systemFacade.ScheduleAppointmentAsStaffAsync(appointment);

                if (result.IsSuccessful)
                {
                    guid = result.Result;
                }
                else
                {
                    if (result.Errors != null && result.Errors.Any())
                        return httpResponseService.Conflict(result.Errors, ApiVersionEnum.V1, result.Code.ToString());

                    return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());
                }

            }
            catch (Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }

            return httpResponseService.OkResponse(guid, ApiVersionEnum.V1);
        }


        [HttpPost("appointment/range/block")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR, RoleType.ASSISTANT, RoleType.CLIENT)]
        public async Task<IActionResult> BlockTimeRange([FromBody] BlockTimeRangeDTO dto)
        {
            var claims = ClaimsPOCO.GetUserClaims(User);
            if (claims.Role == RoleType.CLIENT && claims.Uuid != dto.ClientUuid)
            {
                return httpResponseService.Conflict(
                    new GenericError("Clients can only block their own uuids"),
                    ApiVersionEnum.V1,
                    MessageCodeType.AUTHENTICATION_UUID_VIOLATION.ToString()
                );
            }

            var earliestTime = dto.SelectedServices.Min(s => s.StartTime);
            var latestTime = dto.SelectedServices.Max(s => s.StartTime);

            // Construir DateTime en UTC directamente
            DateTimeRange range = new DateTimeRange
            {
                StartDate = DateTime.SpecifyKind(dto.Date.ToDateTime(earliestTime), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(dto.Date.ToDateTime(latestTime), DateTimeKind.Utc)
            };

            List<ScheduledService> services = dto.SelectedServices.Select(service => new ScheduledService
            {
                Uuid = service.Uuid,
                ServiceStartDate = DateTime.SpecifyKind(
                    dto.Date.ToDateTime(service.StartTime),
                    DateTimeKind.Utc
                )
            }).ToList();

            OperationResult<DateTime, GenericError> result =
                await systemFacade.BlockTimeRangeAsync(services, range, dto.ClientUuid);

            if (!result.IsSuccessful)
            {
                if (result.Errors != null && result.Errors.Any())
                    return httpResponseService.Conflict(result.Errors, ApiVersionEnum.V1, result.Code.ToString());

                return httpResponseService.Conflict(result.Error, ApiVersionEnum.V1, result.Code.ToString());
            }

            return httpResponseService.OkResponse(result.Result, ApiVersionEnum.V1);
        }


        [HttpGet("appointment/conflict")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR, RoleType.ASSISTANT, RoleType.CLIENT)]
        public async Task<IActionResult> GetConflictingServiceAppointmentFromRange([FromQuery] DateTimeRangeDTO dto)
        {
            List<ConflictingServiceOfferDTO> conflictingServiceOfferDTOs = [];
            try
            {
                DateTimeRange range = new DateTimeRange(dto.StartDate!.Value, dto.EndDate!.Value);
                var conflictingServiceOffers = await systemFacade.GetConflictingServicesByDateTimeRangeAsync(range);
                conflictingServiceOfferDTOs = conflictingServiceOffers.Select(a => new ConflictingServiceOfferDTO
                {
                    ScheduledService = new ConflictingServiceOfferDataDTO
                    {
                        Uuid = a.Uuid!.Value,
                        Name = a.ServiceName
                    },
                    Assistant = new AssistantConflictingAppointmentDTO
                    {
                        Name = a.ServiceOffer!.Assistant!.Name,
                        Uuid = a.ServiceOffer!.Assistant!.Uuid!.Value
                    },
                    TimeRange = new ConflictingAppointmentTimeRangeDTO
                    {
                        StartDate = a.ServiceStartDate!.Value,
                        EndDate = a.ServiceEndDate!.Value
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
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
        public async Task<IActionResult> ConfirmAppointment([FromBody] ConfirmAppointmentDTO dto)
        {
            bool isConfirmed = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.ConfirmAppointmentAsync(dto.Uuid);
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
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR)]
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
        [Authorize]
        [AllowedRoles(RoleType.CLIENT)]
        public async Task<IActionResult> CancelAppointment([FromBody] CancelAppointmentAsClientDTO dto)
        {
            bool isConfirmed = false;
            try
            {
                var claims = ClaimsPOCO.GetUserClaims(User);
                OperationResult<bool, GenericError> result = await systemFacade.CancelAppointmentClientSelfAsync(dto.Uuid, claims.Uuid);
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
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR, RoleType.ASSISTANT)]
        public async Task<IActionResult> CancelAppointmentAsStaff([FromBody] CancelAppointmentAsStaffDTO dto)
        {
            bool isConfirmed = false;
            try
            {
                OperationResult<bool, GenericError> result = await systemFacade.CancelAppointmentStaffAssistedAsync(dto.Uuid);
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
