using System;
using System.Collections.Generic;
using System.Linq;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessInterfaces;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.BusinessComponents
{
    public class ClientMgr : IClientMgt
    {
        public bool ChangeClientStatusType(int idClient, ClientStatusType status)
        {
            throw new NotImplementedException();
        }

        public Client GetClientDetails(int idClient)
        {
            throw new NotImplementedException();
        }

        public ClientStatusType GetClientStatusType(int idClient)
        {
            throw new NotImplementedException();
        }

        public bool IsClientAvailable(int idClient)
        {
            throw new NotImplementedException();
        }

        public bool RegisterClient(Client client)
        {
            throw new NotImplementedException();
        }
    }
}