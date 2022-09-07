namespace MeuBolsoDigital.IntegrationEventLog
{
    public interface IIntegrationEventLogRepository
    {
        Task AddAsync(IntegrationEventLogEntry integrationEventLogEntry);
        Task UpdateAsync(IntegrationEventLogEntry integrationEventLogEntry);
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync();
    }
}