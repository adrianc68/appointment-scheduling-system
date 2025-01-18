using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ServiceInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
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
    public class NotificationController : ControllerBase
    {
        private readonly INotificationInterfaces systemFacade;
        private readonly IHttpResponseService httpResponseService;

        public NotificationController(INotificationInterfaces systemFacade, IHttpResponseService httpResponseService)
        {
            this.systemFacade = systemFacade;
            this.httpResponseService = httpResponseService;
        }

        [HttpGet("unread")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR, RoleType.ASSISTANT, RoleType.CLIENT)]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            List<object> notificationDTOs = [];
            try
            {
                var claims = ClaimsPOCO.GetUserClaims(User);

                List<Notification> notifications = await systemFacade.GetUnreadNotificationsByAccountUuid(claims.Uuid);
                foreach (var notification in notifications)
                {

                    if (notification is AppointmentNotification appointmentNotification)
                    {
                        notificationDTOs.Add(new AppointmentNotificationDTO
                        {
                            // Status = appointmentNotification.Status!.Value,
                            Uuid = appointmentNotification.Uuid!.Value,
                            CreatedAt = appointmentNotification!.CreatedAt!.Value,
                            Message = appointmentNotification.Message!,
                            Type = appointmentNotification.Type,
                            Code = appointmentNotification!.Code,
                            Appointment = new AppointmentUuidDTO
                            {
                                Uuid = appointmentNotification.Appointment!.Uuid!.Value,
                            }
                        });
                    }
                    else if (notification is SystemNotification systemNotification)
                    {
                        notificationDTOs.Add(new SystemNotificationDTO
                        {
                            // Status = systemNotification.Status!.Value,
                            Uuid = systemNotification.Uuid!.Value,
                            CreatedAt = systemNotification!.CreatedAt!.Value,
                            Message = systemNotification.Message!,
                            Type = systemNotification.Type,
                            Code = systemNotification!.Code,
                            Severity = systemNotification.Severity!.Value
                        });
                    }
                    else if (notification is GeneralNotification generalNotification)
                    {
                        notificationDTOs.Add(new GeneralNotificationDTO
                        {
                            // Status = generalNotification.Status!.Value,
                            Uuid = generalNotification.Uuid!.Value,
                            CreatedAt = generalNotification!.CreatedAt!.Value,
                            Message = generalNotification.Message!,
                            Type = generalNotification.Type,
                            Code = generalNotification!.Code!,
                        });
                    }
                    else
                    {
                        throw new InvalidOperationException("Notification unknown.");
                    }
                }
                ;

            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(notificationDTOs, ApiVersionEnum.V1);
        }

        [HttpGet("all")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR, RoleType.ASSISTANT, RoleType.CLIENT)]
        public async Task<IActionResult> GetAllNotifications()
        {
            List<object> notificationDTOs = [];
            try
            {
                var claims = ClaimsPOCO.GetUserClaims(User);

                List<Notification> notifications = await systemFacade.GetNotificationsByAccountUuid(claims.Uuid);
                foreach (var notification in notifications)
                {

                    if (notification is AppointmentNotification appointmentNotification)
                    {
                        notificationDTOs.Add(new AppointmentNotificationDTO
                        {
                            // Status = appointmentNotification.Status!.Value,
                            Uuid = appointmentNotification.Uuid!.Value,
                            CreatedAt = appointmentNotification!.CreatedAt!.Value,
                            Message = appointmentNotification.Message!,
                            Type = appointmentNotification.Type,
                            Code = appointmentNotification!.Code,
                            Appointment = new AppointmentUuidDTO
                            {
                                Uuid = appointmentNotification.Appointment!.Uuid!.Value,
                            }
                        });
                    }
                    else if (notification is SystemNotification systemNotification)
                    {
                        notificationDTOs.Add(new SystemNotificationDTO
                        {
                            // Status = systemNotification.Status!.Value,
                            Uuid = systemNotification.Uuid!.Value,
                            CreatedAt = systemNotification!.CreatedAt!.Value,
                            Message = systemNotification.Message!,
                            Type = systemNotification.Type,
                            Code = systemNotification!.Code,
                            Severity = systemNotification.Severity!.Value
                        });
                    }
                    else if (notification is GeneralNotification generalNotification)
                    {
                        notificationDTOs.Add(new GeneralNotificationDTO
                        {
                            // Status = generalNotification.Status!.Value,
                            Uuid = generalNotification.Uuid!.Value,
                            CreatedAt = generalNotification!.CreatedAt!.Value,
                            Message = generalNotification.Message!,
                            Type = generalNotification.Type,
                            Code = generalNotification!.Code!,
                        });
                    }
                    else
                    {
                        throw new InvalidOperationException("Notification unknown.");
                    }
                }
                ;

            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(notificationDTOs, ApiVersionEnum.V1);
        }


        [HttpPost("read")]
        [Authorize]
        [AllowedRoles(RoleType.ADMINISTRATOR, RoleType.ASSISTANT, RoleType.CLIENT)]
        public async Task<IActionResult> ReadNotification([FromBody] NotificationUuidDTO dto)
        {
            bool isUpdated = false;
            try
            {
                var claims = ClaimsPOCO.GetUserClaims(User);

                isUpdated = await systemFacade.MarkNotificationAsReadAsync(dto.Uuid, claims.Uuid);

            }
            catch (System.Exception ex)
            {
                return httpResponseService.InternalServerErrorResponse(ex, ApiVersionEnum.V1);
            }
            return httpResponseService.OkResponse(isUpdated, ApiVersionEnum.V1);
        }

    }
}