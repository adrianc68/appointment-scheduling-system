using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IEnableServiceOffer
    {
        Task<OperationResult<bool, GenericError>> EnableServiceOfferAsync(Guid serviceOfferUuid);
    }
}