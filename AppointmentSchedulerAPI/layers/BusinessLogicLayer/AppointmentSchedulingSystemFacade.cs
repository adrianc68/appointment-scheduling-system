using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer
{
    public class AppointmentSchedulingSystemFacade(IServiceMgt serviceMgr, IAssistantMgt assistantMgr, IClientMgt clientMgr, ISchedulerMgt schedulerMgr)
    {
        private readonly IServiceMgt serviceMgr = serviceMgr;
        private readonly ISchedulerMgt schedulerMgr = schedulerMgr;
        private readonly IAssistantMgt assistantMgr = assistantMgr;
        private readonly IClientMgt clientMgr = clientMgr;
    }



}