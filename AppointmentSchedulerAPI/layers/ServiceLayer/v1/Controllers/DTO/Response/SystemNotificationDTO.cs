using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class SystemNotificationDTO : NotificationDTO
    {
        public required SystemNotificationCodeType Code { get; set;}
        public required SystemNotificationSeverityCodeType Severity { get; set;}

    }
}