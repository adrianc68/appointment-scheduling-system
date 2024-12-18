namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model
{
    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public string Version { get; set; }

        public ApiResponse(int status, string message, string version, T data)
        {
            Status = status;
            Message = message;
            Data = data;
            Version = version;
        }
        public ApiResponse(int status, string message, string version)
        {
            Status = status;
            Message = message;
            Version = version;
        }

    }
}