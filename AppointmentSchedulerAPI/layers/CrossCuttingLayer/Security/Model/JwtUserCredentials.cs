namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Model
{
    public class JwtUserCredentials
    {
        public required string Username { get; set; }
        public required string Role { get; set;}
        public required string Email { get; set;}
        public required string Uuid { get; set;}
    }
}