using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern
{
    public interface IClientObserver : IAccountObserver<ClientEvent>
    {
        void UpdateOnClientChanged(ClientEvent clientEvent);
    }
}