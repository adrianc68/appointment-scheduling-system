using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Validation
{
    public class AllowSpecialCharactersAttribute : ValidationAttribute
    {
        private readonly string _allowedCharacters;

        public AllowSpecialCharactersAttribute(string allowedCharacters = @".-_")
        {
            _allowedCharacters = Regex.Escape(allowedCharacters);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string str)
            {
                string pattern = $@"^[a-zA-Z0-9{_allowedCharacters}]*$";
                if (!Regex.IsMatch(str, pattern))
                {
                    return new ValidationResult($"Only letters, numbers, spaces, and the following characters are allowed: {_allowedCharacters}");
                }
            }
            return ValidationResult.Success;
        }
    }
}
