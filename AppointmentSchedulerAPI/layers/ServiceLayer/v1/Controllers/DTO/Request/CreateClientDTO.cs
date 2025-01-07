using System.ComponentModel.DataAnnotations;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Validation;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class CreateClientDTO
    {
        public required string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#]).{8,}$", ErrorMessage = "Password must include at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public required string Password { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        [NoSpecialCharacters]
        public required string Username { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name cannot exceed 100 characters.")]
        [NoSpecialCharacters]
        public required string Name { get; set; }
         [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Phone number must be valid and include 10-15 digits.")]
        public required string PhoneNumber { get; set; }
    }
}