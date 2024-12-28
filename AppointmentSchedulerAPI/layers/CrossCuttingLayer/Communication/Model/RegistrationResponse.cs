namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model
{
    public class RegistrationResponse<T>
    {
        public bool IsSuccessful { get; set; }
        public MessageCodeType Code { get; set; }
        public T? Data { get; set; }
    }
}