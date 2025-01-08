using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern
{
    public interface IClientEvent
    {
        void NotifySuscribers(ClientEvent eventType);
        void Suscribe(IClientObserver clientObserver);
        void Unsuscribe(IClientObserver clientObserver);
    }
}