using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.SchedulingInterfaces
{
    public interface IBlockDateTimeRange
    {
      Task<OperationResult<DateTime, GenericError>> BlockTimeRange(List<ScheduledService> services, DateTimeRange range, Guid clientUuid);
    }
}