
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model.Types;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;

public partial class Service
{
    public string? Description { get; set; }
    public int? Minutes { get; set; }
    public string? Name { get; set; }
    public double? Price { get; set; }
    public int? Id { get; set; }
    public Guid? Uuid { get; set; }
    public DateTime? CreatedAt { get; set; }
    public ServiceStatusType? Status { get; set; }

    public virtual IEnumerable<AssistantService>? AssistantServices { get; set; } 

}
