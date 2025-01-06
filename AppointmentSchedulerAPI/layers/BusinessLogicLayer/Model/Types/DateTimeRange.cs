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


        public override bool Equals(object? obj)
        {
            if (obj is DateTimeRange other)
            {
                return this.StartTime == other.StartTime && this.EndTime == other.EndTime && this.Date == other.Date;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StartTime, EndTime, Date);
        }
    }
}