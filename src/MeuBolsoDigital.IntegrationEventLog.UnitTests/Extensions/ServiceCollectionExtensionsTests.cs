using MeuBolsoDigital.IntegrationEventLog.Extensions;
using MeuBolsoDigital.IntegrationEventLog.Repositories;
using MeuBolsoDigital.IntegrationEventLog.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MeuBolsoDigital.IntegrationEventLog.UnitTests.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        private class FakeRepository : IIntegrationEventLogRepository
        {
            public Task AddAsync(IntegrationEventLogEntry integrationEventLogEntry)
            {
                throw new NotImplementedException();
            }

            public Task<IntegrationEventLogEntry> FindNextToPublishAsync()
            {
                throw new NotImplementedException();
            }

            public Task ResetFailedAsync()
            {
                throw new NotImplementedException();
            }

            public Task ResetInProgressAsync()
            {
                throw new NotImplementedException();
            }

            public Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync()
            {
                throw new NotImplementedException();
            }

            public Task UpdateAsync(IntegrationEventLogEntry integrationEventLogEntry)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void AddIntegrationEventLog_ReturnSuccess()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddIntegrationEventLog<FakeRepository>();

            // Assert
            var service = services.FirstOrDefault(x => x.ServiceType == typeof(IIntegrationEventLogService));
            var repository = services.FirstOrDefault(x => x.ServiceType == typeof(IIntegrationEventLogRepository));
            Assert.NotNull(service);
            Assert.NotNull(repository);
        }
    }
}