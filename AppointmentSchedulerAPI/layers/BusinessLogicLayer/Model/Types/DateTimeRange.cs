namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types
{
    public class DateTimeRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTimeRange(DateTime startDate, DateTime endDate)
        {
            // StartDate = startDate;
            // EndDate = endDate;
            StartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc).ToUniversalTime();
            EndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc).ToUniversalTime();
        }

        public DateTimeRange()
        {
        }


        public override bool Equals(object? obj)
        {
            if (obj is DateTimeRange other)
            {
                // Compara StartDate y EndDate
                return this.StartDate == other.StartDate && this.EndDate == other.EndDate;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EndDate, StartDate);
        }
    }
}