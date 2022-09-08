using MeuBolsoDigital.IntegrationEventLog.Repositories;
using MeuBolsoDigital.IntegrationEventLog.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MeuBolsoDigital.IntegrationEventLog.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIntegrationEventLog<TRepository>(this IServiceCollection services) where TRepository : IIntegrationEventLogRepository
        {
            services.AddScoped<IIntegrationEventLogService, IntegrationEventLogService>()
                    .AddScoped(typeof(IIntegrationEventLogRepository), typeof(TRepository));

            return services;
        }
    }
}