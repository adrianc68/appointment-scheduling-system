using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AccountInterfaces
{
    public interface IAccountJwtLogin
    {
        Task<OperationResult<JwtTokenResult, GenericError>> LoginWithEmailOrUsernameOrPhoneNumberJwtToken(string account, string password);
        Task<OperationResult<JwtTokenResult, GenericError>> RefreshToken(string token);
        Task<OperationResult<AccountData, GenericError>> ValidateCredentials(string token);
    }
}