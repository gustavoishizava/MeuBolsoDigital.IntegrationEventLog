using System.Text.Json;
using MeuBolsoDigital.IntegrationEventLog.Repositories;

namespace MeuBolsoDigital.IntegrationEventLog.Services
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly IIntegrationEventLogRepository _repository;

        public IntegrationEventLogService(IIntegrationEventLogRepository repository)
        {
            _repository = repository;
        }

        public async Task CreateEventAsync<T>(T @event, string eventTypeName) where T : class
        {
            ArgumentNullException.ThrowIfNull(@event, nameof(@event));

            var content = JsonSerializer.Serialize(@event, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var integrationEventLogEntry = new IntegrationEventLogEntry(eventTypeName, content);
            await _repository.AddAsync(integrationEventLogEntry);
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync()
        {
            return await _repository.RetrieveEventLogsPendingToPublishAsync();
        }

        public async Task SetEventToPublishedAsync(IntegrationEventLogEntry integrationEventLogEntry)
        {
            ArgumentNullException.ThrowIfNull(integrationEventLogEntry, nameof(integrationEventLogEntry));

            integrationEventLogEntry.SetStateToPublished();
            await _repository.UpdateAsync(integrationEventLogEntry);
        }
    }
}