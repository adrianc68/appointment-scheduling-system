using System.ComponentModel.DataAnnotations;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.ValidationAttributes
{
    public class ValidGuidAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is Guid guid && guid == Guid.Empty)
            {
                return new ValidationResult("Uuid cannot be an empty GUID.");
            }
            return ValidationResult.Success!;
        }
    }
}
