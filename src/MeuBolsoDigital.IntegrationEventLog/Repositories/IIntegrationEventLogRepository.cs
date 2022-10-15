namespace MeuBolsoDigital.IntegrationEventLog.Repositories
{
    public interface IIntegrationEventLogRepository
    {
        Task AddAsync(IntegrationEventLogEntry integrationEventLogEntry);
        Task UpdateAsync(IntegrationEventLogEntry integrationEventLogEntry);
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync();
        Task<IntegrationEventLogEntry> FindNextToPublishAsync();
        Task ResetFailedAsync();
        Task ResetInProgressAsync();
    }
}