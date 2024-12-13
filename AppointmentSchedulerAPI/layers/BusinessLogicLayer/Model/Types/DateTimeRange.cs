namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types
{
    public class DateTimeRange
    {
        public DateTime Date { get; set; }
        public Time StartTime { get; set; }
        public Time EndTime { get; set; }

        public DateTimeRange(DateTime date, Time startTime, Time endTime)
        {
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}