using System.ComponentModel.DataAnnotations;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Validation;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class CreateAssistantDTO
    {
        [Required(ErrorMessage = ValidationCodeType.VALIDATION_EMAIL_FIELD_REQUIRED)]
        [EmailAddress(ErrorMessage = ValidationCodeType.VALIDATION_EMAIL_INVALID_FORMAT)]
        [StringLength(100, ErrorMessage = ValidationCodeType.VALIDATION_EMAIL_MAX_LIMIT_VIOLATION)]
        [MinLength(3, ErrorMessage = ValidationCodeType.VALIDATION_EMAIL_MIN_LIMIT_VIOLATION)]
        public required string Email { get; set; }

        [Required(ErrorMessage = ValidationCodeType.VALIDATION_PASSWORD_FIELD_REQUIRED)]
        [StringLength(50, ErrorMessage = ValidationCodeType.VALIDATION_EMAIL_MAX_LIMIT_VIOLATION)]
        [MinLength(8, ErrorMessage = ValidationCodeType.VALIDATION_PASSWORD_MIN_LIMIT_VIOLATION)]
        [StrongPassword]
        public required string Password { get; set; }

        [Required(ErrorMessage = ValidationCodeType.VALIDATION_USERNAME_FIELD_REQUIRED)]
        [StringLength(50, ErrorMessage = ValidationCodeType.VALIDATION_USERNAME_MAX_LIMIT_VIOLATION)]
        [MinLength(3, ErrorMessage = ValidationCodeType.VALIDATION_USERNAME_MIN_LIMIT_VIOLATION)]
        [AllowSpecialCharacters(allowedCharacters: @"._")]
        public required string Username { get; set; }

        [Required(ErrorMessage = ValidationCodeType.VALIDATION_PHONE_NUMBER_FIELD_REQUIRED)]
        [PhoneNumber]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = ValidationCodeType.VALIDATION_NAME_FIELD_REQUIRED)]
        [StringLength(100, ErrorMessage = ValidationCodeType.VALIDATION_NAME_MAX_LIMIT_VIOLATION)]
        [MinLength(3, ErrorMessage = ValidationCodeType.VALIDATION_NAME_MIN_LIMIT_VIOLATION)]
        [AllowSpecialCharacters(allowedCharacters: " .")]
        public required string Name { get; set; }

    }
}