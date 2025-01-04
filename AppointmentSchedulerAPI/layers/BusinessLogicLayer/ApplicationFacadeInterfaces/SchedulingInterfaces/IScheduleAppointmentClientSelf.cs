using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IScheduleAppointmentClientSelf
    {
        Task<OperationResult<bool, GenericError>> ConfirmAppointment(Guid appointmentUuid);
        Task<OperationResult<Guid, GenericError>> ScheduleAppointmentAsClientAsync(Appointment appointment);
    }
}