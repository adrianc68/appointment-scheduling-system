using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern
{
    public interface ISchedulerObserver
    {
        void UpdateOnSchedulerEvent<T>(SchedulerEvent<T> schedulerEvent);
    }
}