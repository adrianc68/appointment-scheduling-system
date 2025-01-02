using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces
{
    public interface IEditAssistant
    {
        Task<OperationResult<bool, GenericError>> EditAssistant(Assistant assistant);
    }
}