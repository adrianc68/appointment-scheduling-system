using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Helper;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.Model;
using AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSchedulerAPI.layers.DataLayer.DatabaseComponents.RepositoryComponents
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly IDbContextFactory<Model.AppointmentDbContext> context;

        public ServiceRepository(IDbContextFactory<Model.AppointmentDbContext> context)
        {
            this.context = context;
        }

        public async Task<bool> AddServiceAsync(BusinessLogicLayer.Model.Service service)
        {
            bool isRegistered = false;
            using var dbContext = context.CreateDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
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

                dbContext.Services.Add(serviceEntity);
                await dbContext.SaveChangesAsync();
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
            using var dbContext = context.CreateDbContext();
            var servicesdb = await dbContext.Services.ToListAsync();

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

        public async Task<BusinessLogicLayer.Model.Service?> GetServiceByIdAsync(int id)
        {
            using var dbContext = context.CreateDbContext();
            var serviceDB = await dbContext.Services
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();

            if (serviceDB == null) return null;
            return new BusinessLogicLayer.Model.Service
            {
                Description = serviceDB.Description,
                Name = serviceDB.Name,
                Minutes = serviceDB.Minutes,
                Price = serviceDB.Price,
                Uuid = serviceDB.Uuid,
                Status = (BusinessLogicLayer.Model.Types.ServiceStatusType)serviceDB.Status,
                CreatedAt = serviceDB.CreatedAt,
                Id = serviceDB.Id
            };
        }

        public async Task<BusinessLogicLayer.Model.Service?> GetServiceByUuidAsync(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();
            var service = await dbContext.Services
            .Where(serviceDB => serviceDB.Uuid == uuid)
            .Select(serviceDB => new BusinessLogicLayer.Model.Service
            {
                Description = serviceDB.Description,
                Name = serviceDB.Name,
                Minutes = serviceDB.Minutes,
                Price = serviceDB.Price,
                Uuid = serviceDB.Uuid,
                Status = (BusinessLogicLayer.Model.Types.ServiceStatusType?)serviceDB.Status,
                CreatedAt = serviceDB.CreatedAt,
                Id = serviceDB.Id
            })
            .FirstOrDefaultAsync();
            return service;
        }

        public async Task<int?> GetServiceIdByUuidAsync(Guid uuid)
        {
            using var dbContext = context.CreateDbContext();
            var serviceId = await dbContext.Services
                .Where(a => a.Uuid == uuid)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
            return serviceId;
        }

        public async Task<bool> IsServiceNameRegisteredAsync(string name)
        {
            using var dbContext = context.CreateDbContext();
            var serviceName = await dbContext.Services
                .Where(a => a.Name.ToLower() == name.ToLower())
                .Select(a => a.Name)
                .FirstOrDefaultAsync();
            return serviceName != null;
        }
    }
}