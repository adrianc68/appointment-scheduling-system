using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces
{
    public interface IDeleteAssistant
    {
        Task<OperationResult<bool, GenericError>> DeleteAssistantAsync(Guid assistantUuid);
    }
}