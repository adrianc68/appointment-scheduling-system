using AppointmentSchedulerAPI.layers.BusinessLogicLayer.ExternalComponents.NotificationMgr.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Response
{
    public class GeneralNotificationDTO : NotificationDTO
    {
        public required GeneralNotificationCodeType Code { get; set;}
    }
}