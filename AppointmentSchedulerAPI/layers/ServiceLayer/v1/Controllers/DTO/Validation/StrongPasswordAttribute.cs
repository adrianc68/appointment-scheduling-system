using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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
                    return new ValidationResult(ErrorMessage ?? "Password must include at least one uppercase letter, one lowercase letter, one number, and one special character.");
                }
            }

            return new ValidationResult("Password must be a string.");
        }
    }
}
