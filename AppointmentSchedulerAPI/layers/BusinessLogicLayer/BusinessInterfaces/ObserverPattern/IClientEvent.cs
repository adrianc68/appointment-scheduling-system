using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern
{
    public interface IClientEvent
    {
        void NotifySubscribers(ClientEvent eventType);
        void Suscribe(IClientObserver clientObserver);
        void Unsuscribe(IClientObserver clientObserver);
    }
}