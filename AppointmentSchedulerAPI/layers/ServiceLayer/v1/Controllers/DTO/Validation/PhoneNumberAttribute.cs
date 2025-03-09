using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Validation
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string phoneNumber && !Regex.IsMatch(phoneNumber, @"^\+?\d{10,15}$"))
            {
                return new ValidationResult(ValidationCodeType.VALIDATION_PHONE_NUMBER_INVALID_FORMAT);
            }
            return ValidationResult.Success;
        }
    }
}
