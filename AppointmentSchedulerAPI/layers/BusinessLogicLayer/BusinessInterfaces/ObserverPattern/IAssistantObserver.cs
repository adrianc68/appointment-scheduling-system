using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern
{
    public interface IAssistantObserver : IAccountObserver<AssistantEvent>
    {
        void UpdateOnAssistantChanged(AssistantEvent assistantEvent);
    }
}