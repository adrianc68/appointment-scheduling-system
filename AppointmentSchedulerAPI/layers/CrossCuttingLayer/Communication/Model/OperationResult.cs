using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;

namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model
{
    public class OperationResult<T, TError>
    {
        public bool IsSuccessful { get; set; }
        public MessageCodeType Code { get; set; }
        public T? Result { get; set; }
        public TError? Error { get; set; }
        public List<TError>? Errors { get; set; } 

        private OperationResult(bool isSuccessful, MessageCodeType code, T? result = default, TError? error = default, List<TError>? errors = null)
        {
            IsSuccessful = isSuccessful;
            Code = code;
            Result = result;
            Error = error;
            Errors = errors;
        }

        public static OperationResult<T, TError> Success(T result, MessageCodeType code = MessageCodeType.OK)
        {
            return new OperationResult<T, TError>(true, code, result, default);
        }

        public static OperationResult<T, TError> Failure(List<TError> errors, MessageCodeType code = MessageCodeType.ERROR)
        {
            return new OperationResult<T, TError>(false, code, default, default, errors);
        }

        public static OperationResult<T, TError> Failure(TError error, MessageCodeType code = MessageCodeType.ERROR)
        {
            return new OperationResult<T, TError>(false, code, default, error);
        }

        public OperationResult() { }
    }
}