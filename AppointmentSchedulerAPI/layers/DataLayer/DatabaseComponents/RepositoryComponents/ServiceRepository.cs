using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryComponents
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly Model.AppointmentDbContext context;

        public ServiceRepository(Model.AppointmentDbContext context)
        {
            this.context = context;
        }


        public async Task<bool> AddServiceAsync(BusinessLogicLayer.Model.Service service)
        {
            bool isRegistered = false;
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var serviceEntity = new Service
                {
                    Description = service.Description,
                    Minutes = service.Minutes,
                    Name = service.Name,
                    Price = service.Price,
                    Uuid = service.Uuid,
                    Status = Model.Types.ServiceStatusType.ENABLED,
                };

                context.Services.Add(serviceEntity);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                isRegistered = true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            return isRegistered;
        }

        public async Task<IEnumerable<BusinessLogicLayer.Model.Service>> GetAllServicesAsync()
        {
            IEnumerable<BusinessLogicLayer.Model.Service> services = [];
            var servicesdb = await context.Services.ToListAsync();

            services = servicesdb
                .Select(a => new BusinessLogicLayer.Model.Service
                {
                    Description = a.Description,
                    Name = a.Name,
                    Minutes = a.Minutes,
                    Price = a.Price,
                    Uuid = a.Uuid,
                    Status = (BusinessLogicLayer.Model.Types.ServiceStatusType?)a.Status,
                    CreatedAt = a.CreatedAt,
                    Id = a.Id
                })
                .ToList();
            return services;
        }

        public async Task<BusinessLogicLayer.Model.Service?> GetServiceByUuidAsync(Guid uuid)
        {
           BusinessLogicLayer.Model.Service? service = null;

            var serviceDB = await context.Services
                .FirstOrDefaultAsync(a => a.Uuid == uuid);
            
            if(serviceDB != null )
            {
                service = new BusinessLogicLayer.Model.Service
                {
                    Description = serviceDB.Description,
                    Name = serviceDB.Name,
                    Minutes = serviceDB.Minutes,
                    Price = serviceDB.Price,
                    Uuid = serviceDB.Uuid,
                    Status = (BusinessLogicLayer.Model.Types.ServiceStatusType?)serviceDB.Status,
                    CreatedAt = serviceDB.CreatedAt,
                    Id = serviceDB.Id
                };
            }

           return service;
        }

        public async Task<int?> GetServiceIdByUuidAsync(Guid uuid)
        {
            var serviceId = await context.Services
                .Where(a => a.Uuid == uuid)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
            return serviceId;
        }
    }
}