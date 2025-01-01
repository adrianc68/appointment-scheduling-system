using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IScheduleAppointmentClientSelf
    {
        bool ConfirmAppointment(int idAppointment);
        Appointment GetAppointmentDetails(int idAppointment);
        List<Appointment> GetAppoinments(DateTime startDate, DateTime endDate);
        Task<List<AssistantService>> GetAvailableServicesClientAsync(DateOnly date);
        Task<OperationResult<Guid, GenericError>> ScheduleAppointmentAsClientAsync(Appointment appointment);
    }
}