namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types
{
    public class DateTimeRange
    {
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public DateTimeRange(DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}