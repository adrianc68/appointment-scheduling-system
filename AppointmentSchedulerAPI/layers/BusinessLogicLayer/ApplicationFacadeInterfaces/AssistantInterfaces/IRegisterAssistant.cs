using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces
{
    public interface IRegisterAssistant
    {
        Task<OperationResult<Guid?>> RegisterAssistant(Assistant assistant);
    }
}