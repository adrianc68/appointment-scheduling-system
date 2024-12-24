using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces
{
    public interface IRegisterAssistant
    {
        Task<Guid?> RegisterAssistant(Assistant assistant);
    }
}