using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.AssistantInterfaces
{
    public interface IEnableAssistant
    {
        bool EnableAssistant(int idAssistant);
    }
}