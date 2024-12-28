namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Model
{
    public class JwtUserCredentials
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}