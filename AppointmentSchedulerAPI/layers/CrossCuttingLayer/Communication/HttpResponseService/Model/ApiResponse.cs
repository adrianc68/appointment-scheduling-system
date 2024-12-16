namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService.Model
{
    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public string Version { get; set; }

        public ApiResponse(int status, string message, T data, string version)
        {
            Status = status;
            Message = message;
            Data = data;
            Version = version;
        }
    }
}