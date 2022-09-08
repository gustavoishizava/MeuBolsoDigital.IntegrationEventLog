namespace MeuBolsoDigital.IntegrationEventLog
{
    public interface IIntegrationEventLogService
    {
        Task CreateEventAsync<T>(T @event, string eventTypeName) where T : class;
        Task SetEventToPublishedAsync(IntegrationEventLogEntry integrationEventLogEntry);
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync();
    }
}