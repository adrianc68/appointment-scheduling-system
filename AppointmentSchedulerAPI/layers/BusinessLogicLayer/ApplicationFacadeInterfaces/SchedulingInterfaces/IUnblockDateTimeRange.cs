using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IUnblockDateTimeRange
    {
      public OperationResult<bool, GenericError> UnblockTimeRange(Guid clientUuid);
    }
}