namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AssistantInterfaces
{
    public interface IAssignServiceToAssistant
    {
        Task<bool> AssignServicesToAssistant(Guid assistantUuid, List<Guid?> servicesUuid);
    }
}