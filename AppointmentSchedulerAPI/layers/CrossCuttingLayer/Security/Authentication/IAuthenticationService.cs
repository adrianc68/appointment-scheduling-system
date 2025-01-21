namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Authentication
{
    public interface IAuthenticationService<TCredentials, TResult, TData>
    {
        TResult? Authenticate(TCredentials credentials);
        TResult? RefreshCredentials(TCredentials credentials);
        TData? ValidateCredentials(TResult credentials);
    }
}
