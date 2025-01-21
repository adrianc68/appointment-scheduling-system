
namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.NotificationInterfaces
{
    public interface IMarkNotificationAsRead
    {
        Task<bool> MarkNotificationAsReadAsync(Guid uuid, Guid accountUuid);
    }
}