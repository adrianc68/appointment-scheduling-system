using System.ComponentModel.DataAnnotations;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.ServiceLayer.v1.Controllers.DTO.Request
{
    public class LoginAccountAndPasswordDTO
    {
        [Required(ErrorMessage = ValidationCodeType.VALIDATION_ACCOUNT_FIELD_REQUIRED)]
        public required string Account { get; set; }

        [Required(ErrorMessage = ValidationCodeType.VALIDATION_PASSWORD_FIELD_REQUIRED)]
        public required string Password { get; set; }
    }
}