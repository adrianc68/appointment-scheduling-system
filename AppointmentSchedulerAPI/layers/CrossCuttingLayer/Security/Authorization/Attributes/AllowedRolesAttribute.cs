using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Authorization.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AllowedRolesAttribute : Attribute
    {
        public RoleType[] Roles { get; }

        public AllowedRolesAttribute(params RoleType[] roles)
        {
            Roles = roles;
        }
    }
}