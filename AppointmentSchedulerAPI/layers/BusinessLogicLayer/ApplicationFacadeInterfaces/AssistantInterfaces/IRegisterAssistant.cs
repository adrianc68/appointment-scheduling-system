using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces
{
    public interface IRegisterAssistant
    {
        Task<RegistrationResponse<Guid>> RegisterAssistant(Assistant assistant);
    }
}