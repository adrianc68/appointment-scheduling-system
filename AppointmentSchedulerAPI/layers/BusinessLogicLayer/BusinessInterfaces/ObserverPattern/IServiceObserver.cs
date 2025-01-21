using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern
{
    public interface IServiceObserver
    {
        void UpdateOnServiceChanged(ServiceEvent serviceEvent);
    }
}