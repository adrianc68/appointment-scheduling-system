using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.AccountInterfaces
{
    public interface IGetAccount
    {
        Task<OperationResult<AccountData, GenericError>> GetAccountData(Guid accountUuid);
    }
}