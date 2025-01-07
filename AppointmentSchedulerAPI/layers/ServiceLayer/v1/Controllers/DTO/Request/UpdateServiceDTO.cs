using System.ComponentModel.DataAnnotations;
using AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Validation;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class UpdateServiceDTO
    {
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        [AllowSpecialCharacters(allowedCharacters: @" .-_:;,\n\r")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Minutes are required.")]
        [Range(1, 1440, ErrorMessage = "Minutes must be between 1 and 1440.")]
        public required int Minutes { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [AllowSpecialCharacters(allowedCharacters: @" ,.;")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public required double Price { get; set; }

        [Required(ErrorMessage = "Uuid is required.")]
        public required Guid Uuid { get; set; }

    }
}