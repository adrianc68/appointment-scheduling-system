namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Authorization
{
    public interface IAuthorizationService<TCredentials>
    {
        bool Authorize(TCredentials credentials, string action, string? resource = null);
        IEnumerable<string> GetPermissions(TCredentials credentials);
    }
}
