using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Validation
{
    public class NoSpecialCharactersAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string str && Regex.IsMatch(str, @"[^a-zA-Z0-9\s]"))
            {
                return new ValidationResult("Name must not contain special characters.");
            }
            return ValidationResult.Success;
        }
    }
}