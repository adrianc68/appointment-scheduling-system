namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Model
{
    public class JwtTokenResult
    {
        public string? Token { get; set; }
        public DateTime? Expiration { get; set; }
    }
}