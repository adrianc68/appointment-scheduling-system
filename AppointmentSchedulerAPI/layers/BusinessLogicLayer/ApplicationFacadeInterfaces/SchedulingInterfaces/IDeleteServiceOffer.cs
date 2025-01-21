using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IDeleteServiceOffer
    {
        Task<OperationResult<bool, GenericError>> DeleteServiceOfferAsync(Guid serviceOfferUuid);
    }
}