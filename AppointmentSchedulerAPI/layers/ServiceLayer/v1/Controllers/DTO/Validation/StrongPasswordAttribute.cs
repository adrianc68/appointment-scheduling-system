using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Validation
{
    public class StrongPasswordAttribute : ValidationAttribute
    {
        private const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#]).{8,}$";

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string password)
            {
                if (Regex.IsMatch(password, PasswordPattern))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(ErrorMessage ?? ValidationCodeType.VALIDATION_PASSWORD_INVALID_FORMAT);
                }
            }

            return new ValidationResult(ValidationCodeType.VALIDATION_PASSWORD_INVALID_TYPE);
        }
    }
}
