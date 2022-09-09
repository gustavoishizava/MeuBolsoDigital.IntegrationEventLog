namespace MeuBolsoDigital.IntegrationEventLog
{
    public class IntegrationEventLogEntry
    {
        public Guid Id { get; private init; }
        public DateTime CreatedAt { get; private init; }
        public DateTime? UpdatedAt { get; private set; }
        public string EventTypeName { get; private init; }
        public string Content { get; private init; }
        public EventState State { get; private set; }

        public IntegrationEventLogEntry(string eventTypeName, string content)
        {
            if (string.IsNullOrEmpty(eventTypeName))
                throw new ArgumentException($"{nameof(eventTypeName)} cannot be null or empty.", nameof(eventTypeName));

            if (string.IsNullOrEmpty(content))
                throw new ArgumentException($"{nameof(content)} cannot be null or empty.", nameof(content));

            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            UpdatedAt = null;
            EventTypeName = eventTypeName;
            Content = content;
            State = EventState.NotPublished;
        }

        protected IntegrationEventLogEntry()
        {
        }

        private void SetState(EventState state)
        {
            UpdatedAt = DateTime.Now;
            State = state;
        }

        public void SetStateToInProgress() => SetState(EventState.InProgress);

        public void SetStateToPublished() => SetState(EventState.Published);

        public void SetStateToNotPublished() => SetState(EventState.NotPublished);
    }
}