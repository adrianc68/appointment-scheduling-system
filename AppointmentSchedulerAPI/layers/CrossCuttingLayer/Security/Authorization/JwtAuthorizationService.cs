using System.IdentityModel.Tokens.Jwt;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Model;

namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Authorization
{
    public class JwtAuthorizationService : IAuthorizationService<JwtUserCredentials>
    {
        public bool Authorize(JwtUserCredentials credentials, string action, string? resource = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetPermissions(JwtUserCredentials credentials)
        {
            throw new NotImplementedException();
        }
    }
}
