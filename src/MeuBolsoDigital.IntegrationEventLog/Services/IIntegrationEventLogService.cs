namespace MeuBolsoDigital.IntegrationEventLog.Services
{
    public interface IIntegrationEventLogService
    {
        Task CreateEventAsync<T>(T @event, string eventTypeName) where T : class;
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync();
        Task<int> ProcessEventsAsync(CancellationToken cancellationToken, Func<IntegrationEventLogEntry, Task<bool>> execute);
    }
}