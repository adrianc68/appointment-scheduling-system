namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Authentication
{
    public interface IAuthenticationService<TCredentials, TResult>
    {
        TResult? Authenticate(TCredentials credentials);
        TResult? RefreshCredentials(TCredentials credentials);
        bool ValidateCredentials(TCredentials credentials);
    }
}
