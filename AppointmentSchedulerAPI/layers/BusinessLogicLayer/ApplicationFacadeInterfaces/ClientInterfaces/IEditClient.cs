using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.ApplicationFacadeInterfaces.ClientInterfaces
{
    public interface IEditClient
    {
        bool EditClient(Client client);
    }
}