using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern
{
    public interface IAssistantObserver
    {
        void UpdateOnAssistantChanged(AssistantEvent assistantEvent);
    }
}