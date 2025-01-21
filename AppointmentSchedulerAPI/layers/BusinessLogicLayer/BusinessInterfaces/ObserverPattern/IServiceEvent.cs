using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern
{
    public interface IServiceEvent
    {
        void NotifySuscribers(ServiceEvent eventType);
        void Suscribe(IServiceObserver serviceObserver);
        void Unsuscribe(IServiceObserver serviceObserver);
    }
}