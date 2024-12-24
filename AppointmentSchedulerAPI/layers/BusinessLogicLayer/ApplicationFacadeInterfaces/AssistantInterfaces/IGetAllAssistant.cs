using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces
{
    public interface IGetAllAsssitant
    {
        Task<List<Assistant>> GetAllAssistantAsync();
    }
}