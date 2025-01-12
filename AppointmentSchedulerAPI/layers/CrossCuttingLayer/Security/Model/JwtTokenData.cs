using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Model
{
    public class JwtTokenData
    {
        public required string Uuid { get; set; }
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Role { get; set; }

    }
}