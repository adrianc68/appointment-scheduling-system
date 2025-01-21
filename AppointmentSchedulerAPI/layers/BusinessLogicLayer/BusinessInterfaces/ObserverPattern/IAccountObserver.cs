namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern
{
    public interface IAccountObserver<TEventType>
    {
        public void UpdateOnAccountChanged(TEventType eventType);
    }
}
