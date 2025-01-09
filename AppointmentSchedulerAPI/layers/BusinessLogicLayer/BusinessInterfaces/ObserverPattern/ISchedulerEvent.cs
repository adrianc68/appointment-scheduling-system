using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern
{
    public interface ISchedulerEvent
    {
        void NotifySuscribers<T>(SchedulerEvent<T> eventType);
        void Suscribe(ISchedulerObserver schedulerObserver);
        void Unsuscribe(ISchedulerObserver schedulerObserver);
    }
}