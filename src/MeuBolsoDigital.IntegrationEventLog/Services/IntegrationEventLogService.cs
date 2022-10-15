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
            ArgumentNullException.ThrowIfNull(@event);

            var content = JsonSerializer.Serialize(@event, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var integrationEventLogEntry = new IntegrationEventLogEntry(eventTypeName, content);
            await _repository.AddAsync(integrationEventLogEntry);
        }

        public async Task<int> ProcessEventsAsync(Func<IntegrationEventLogEntry, Task<bool>> execute, CancellationToken cancellationToken)
        {
            var count = 0;
            var @event = await _repository.FindNextToPublishAsync();
            while (@event is not null)
            {
                count++;

                var result = await execute(@event);
                if (result)
                    @event.SetStateToPublished();
                else
                    @event.SetStateToPublishedFailed();

                await _repository.UpdateAsync(@event);

                if (cancellationToken.IsCancellationRequested)
                    break;

                @event = await _repository.FindNextToPublishAsync();
            }

            return count;
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync()
        {
            return await _repository.RetrieveEventLogsPendingToPublishAsync();
        }
    }
}