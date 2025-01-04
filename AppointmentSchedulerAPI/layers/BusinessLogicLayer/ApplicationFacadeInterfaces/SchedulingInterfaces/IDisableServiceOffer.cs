using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IDisableServiceOffer
    {
        Task<OperationResult<bool, GenericError>> DisableServiceOfferAsync(Guid serviceOfferUuid);
    }
}