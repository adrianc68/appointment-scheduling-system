using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces
{
    public interface IGetAsssitantService
    {
        Task<OperationResult<List<ServiceOffer>, GenericError>> GetAllAssignedServicesAsync(Guid uuid);
    }
}