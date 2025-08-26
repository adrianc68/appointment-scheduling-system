using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces
{
    public interface IAssignServiceToAssistant
    {
        Task<OperationResult<List<Guid>, GenericError>> AssignListServicesToAssistantAsync(Guid assistantUuid, List<Guid> servicesUuids);
    }
}