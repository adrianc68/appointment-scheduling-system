using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.AssistantInterfaces
{
    public interface IRegisterAssistant
    {
        bool RegisterAssistant(Assistant assistant);
    }
}