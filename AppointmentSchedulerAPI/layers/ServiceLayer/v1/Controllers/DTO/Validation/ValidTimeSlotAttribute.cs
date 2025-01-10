using System.ComponentModel.DataAnnotations;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Validation
{
    public class ValidTimeSlotAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not List<UnavailableTimeSlotDTO> timeSlots || timeSlots.Count == 0)
            {
                return ValidationResult.Success;
            }

            foreach (var timeSlot in timeSlots)
            {
                if (timeSlot.StartTime >= timeSlot.EndTime)
                {
                    return new ValidationResult("Each UnavailableTimeSlot must have StartTime earlier than EndTime.");
                }
            }

            var sortedTimeSlots = timeSlots.OrderBy(ts => ts.StartTime).ToList();
            for (int i = 0; i < sortedTimeSlots.Count - 1; i++)
            {
                if (sortedTimeSlots[i].EndTime > sortedTimeSlots[i + 1].StartTime)
                {
                    return new ValidationResult("UnavailableTimeSlots cannot have overlapping time ranges.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
