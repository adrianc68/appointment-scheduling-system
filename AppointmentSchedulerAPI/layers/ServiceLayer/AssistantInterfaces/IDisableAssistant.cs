using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.AssistantInterfaces
{
    public interface IDisableAssistant
    {
        bool DisableAssistant(int dAssistant);
    }
}