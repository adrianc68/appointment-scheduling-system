using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryComponents
{
    public class AssistantRepository : IAssistantRepository
    {
        // private readonly Model.AppointmentDbContext context;
        // public AssistantRepository(Model.AppointmentDbContext context)
        // {
        //     this.context = context;
        // }

        public bool ChangeAssistantStatus(int idAssistant, AssistantStatusType status)
        {
            throw new NotImplementedException();
        }

        public AssistantStatusType GetAssistantStatus(int idAssistant)
        {
            throw new NotImplementedException();
        }

        public bool GetServicesAssignedToAssistant(int idAssistant)
        {
            throw new NotImplementedException();
        }

        public bool IsAssistantRegistered(Assistant assistant)
        {
            throw new NotImplementedException();
        }

        public bool RegisterAssistant(Assistant assistant)
        {

            try
            {

                // var userAccount = new Model.UserAccount
                // {
                //     Email = "test@hotmail.com",
                //     Password = "hellowolrd",
                //     Username = "Testing",
                // };

                // context.UserAccounts.Add(userAccount);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
            throw new NotImplementedException();
        }

        public bool UpdateAssistant(int idAssistant, Assistant assistant)
        {
            throw new NotImplementedException();
        }
    }
}