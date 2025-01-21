
namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces.ObserverPattern
{
    public interface IAccountEvent<TEventType>
    {
        void NotifySubscribers(TEventType eventType);

        void Subscribe(IAccountObserver<TEventType> observer);

        void Unsubscribe(IAccountObserver<TEventType> observer);
    }
}
