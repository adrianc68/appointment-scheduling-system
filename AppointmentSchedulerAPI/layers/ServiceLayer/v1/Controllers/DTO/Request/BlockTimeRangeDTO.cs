using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

public class BlockTimeRangeDTO
{
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    // $$$>> Remove it! It should be got it from Authentication service!
    public Guid AccountUuid { get; set;}
}