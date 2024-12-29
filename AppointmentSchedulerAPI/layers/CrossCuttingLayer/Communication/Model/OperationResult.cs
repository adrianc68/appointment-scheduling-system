namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model
{
    public class OperationResult<T>
    {
        public bool IsSuccessful { get; set; }
        public MessageCodeType Code { get; set; }
        public T? Result { get; set; }

        public OperationResult(bool isSuccessful, MessageCodeType code, T? result = default)
        {
            IsSuccessful = isSuccessful;
            Code = code;
            Result = result;
        }

        public OperationResult() { }
    }
}