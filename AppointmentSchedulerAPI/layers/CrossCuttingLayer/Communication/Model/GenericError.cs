namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model
{
    public class GenericError
    {
       public string Message { get; set; }
        public Dictionary<string, object>? AdditionalData { get; set; }
        
        public GenericError(string message, Dictionary<string, object>? additionalData = null)
        {
            Message = message;
            AdditionalData = additionalData;
        }

        public void AddData(string key, object value)
        {
            AdditionalData ??= [];
            AdditionalData[key] = value;
        }
    }
}