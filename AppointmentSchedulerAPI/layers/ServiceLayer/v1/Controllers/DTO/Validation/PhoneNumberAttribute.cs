using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Validation
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string phoneNumber && !Regex.IsMatch(phoneNumber, @"^\+?\d{10,15}$"))
            {
                return new ValidationResult("Phone number must be valid and include 10-15 digits.");
            }
            return ValidationResult.Success;
        }
    }
}
