using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces
{
    public interface IEnableAssistant
    {
        Task<OperationResult<bool, GenericError>> EnableAssistantAsync(Guid uuidAssistant);
    }
}