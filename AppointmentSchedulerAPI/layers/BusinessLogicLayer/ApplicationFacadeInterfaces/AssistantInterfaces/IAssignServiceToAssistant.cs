using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces
{
    public interface IAssignServiceToAssistant
    {
        Task<OperationResult<bool, GenericError>> AssignServicesToAssistant(Guid assistantUuid, List<Guid?> servicesUuid);
    }
}