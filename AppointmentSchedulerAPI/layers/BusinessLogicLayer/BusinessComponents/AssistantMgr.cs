using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;


namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents
{
    public class AssistantMgr : IAssistantMgt
    {
        private readonly IAssistantRepository assistantRepository;

        public AssistantMgr(IAssistantRepository assistantRepository)
        {
            this.assistantRepository = assistantRepository;
        }

        public bool ChangeAssistantStatus(int idAssistant, AssistantStatusType status)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Assistant>> GetAllAssistantsAsync()
        {
            return (List<Assistant>)await assistantRepository.GetAllAssistantsAsync();
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

        public async Task<Guid?> RegisterAssistantAsync(Assistant assistant)
        {
            assistant.Uuid = Guid.CreateVersion7();
            bool isRegistered = await assistantRepository.RegisterAssistantAsync(assistant);
            if (!isRegistered)
            {
                return null;
            }
            return assistant.Uuid.Value;
        }

        public bool UpdateAssistant(int idAssistant, Assistant assistant)
        {
            throw new NotImplementedException();
        }
    }
}