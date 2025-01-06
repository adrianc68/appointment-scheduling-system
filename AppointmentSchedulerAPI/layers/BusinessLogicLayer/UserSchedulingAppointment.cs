using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer
{
    public class UserSchedulingAppointment
    {
        public Guid AccountUuid { get; set; }
        public DateTimeRange Range { get; set; }

        public UserSchedulingAppointment(Guid accountUuid, DateTimeRange range)
        {
            AccountUuid = accountUuid;
            Range = range;
        }
    }
}