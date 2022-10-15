namespace MeuBolsoDigital.IntegrationEventLog.Services
{
    public interface IIntegrationEventLogService
    {
        Task CreateEventAsync<T>(T @event, string eventTypeName) where T : class;
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync();
        Task<int> ProcessEventsAsync(Func<IntegrationEventLogEntry, Task<bool>> execute, CancellationToken cancellationToken);
    }
}