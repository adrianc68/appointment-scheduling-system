namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types
{
    public class DateTimeRange
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public DateTimeRange(DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
        }

        public DateTimeRange()
        {
        }
    }
}