using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces
{
    public interface IDisableAssistant
    {
        Task<OperationResult<bool,GenericError>> DisableAssistantAsync(Guid uuidAssistant);
    }
}