using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.AssistantInterfaces
{
    public interface IEditAssistant
    {
        bool EditAssistant(Assistant assistant);
    }
}