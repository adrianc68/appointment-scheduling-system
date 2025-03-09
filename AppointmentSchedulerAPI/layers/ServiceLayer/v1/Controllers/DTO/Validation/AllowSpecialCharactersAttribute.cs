using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Validation
{
    public class AllowSpecialCharactersAttribute : ValidationAttribute
    {
        private readonly string _allowedCharacters;
        private readonly string _message;

        public AllowSpecialCharactersAttribute(string allowedCharacters = @".-_", string message = ValidationCodeType.VALIDATION_CHARACTERS_NOT_ALLOWED_VIOLATION)
        {
            _allowedCharacters = Regex.Escape(allowedCharacters);
            _message = message;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string str)
            {
                string pattern = $@"^[a-zA-Z0-9{_allowedCharacters}]*$";
                if (!Regex.IsMatch(str, pattern))
                {
                    return new ValidationResult(_message);
                }
            }
            return ValidationResult.Success;
        }
    }
}
