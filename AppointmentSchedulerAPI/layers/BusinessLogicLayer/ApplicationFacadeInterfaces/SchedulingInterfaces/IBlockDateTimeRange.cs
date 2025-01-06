using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IBlockDateTimeRange
    {
      public OperationResult<DateTime, GenericError> BlockTimeRange(DateTimeRange range, Guid accountUuid);
       
    }
}