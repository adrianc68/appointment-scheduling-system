using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces
{
    public interface IAssignServiceToAssistant
    {
        Task<OperationResult<bool, GenericError>> AssignListServicesToAssistantAsync(Guid uuidAssistant, List<Guid> uuidServices);
    }
}