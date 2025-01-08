using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern
{
    public interface IAssistantEvent
    {
        void NotifySuscribers(AssistantEvent eventType);
        void Suscribe(IAssistantObserver assistantObserver);
        void Unsuscribe(IAssistantObserver assistantObserver);
    }
}