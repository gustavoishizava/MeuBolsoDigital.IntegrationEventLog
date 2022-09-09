namespace MeuBolsoDigital.IntegrationEventLog.UnitTests
{
    public class IntegrationEventLogEntryTests
    {
        [Theory]
        [InlineData(null)]
        public void Create_InvalidEventTypeName_ThrowsArgumentNullException(string eventTypeName)
        {
            // Act
            var exception = Assert.Throws<ArgumentException>(() => new IntegrationEventLogEntry(eventTypeName, Guid.NewGuid().ToString()));

            // Assert
            Assert.Equal("eventTypeName", exception.ParamName);
            Assert.Equal("eventTypeName cannot be null or empty. (Parameter 'eventTypeName')", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        public void Create_InvalidContext_ThrowsArgumentNullException(string content)
        {
            // Act
            var exception = Assert.Throws<ArgumentException>(() => new IntegrationEventLogEntry(Guid.NewGuid().ToString(), content));

            // Assert
            Assert.Equal("content", exception.ParamName);
            Assert.Equal("content cannot be null or empty. (Parameter 'content')", exception.Message);
        }

        [Fact]
        public void Create_ReturnSuccess()
        {
            // Arrange
            var eventTypeName = Guid.NewGuid().ToString();
            var content = Guid.NewGuid().ToString();

            // Act
            var integrationEventLogEntry = new IntegrationEventLogEntry(eventTypeName, content);

            // Assert
            Assert.NotEqual(Guid.Empty, integrationEventLogEntry.Id);
            Assert.NotEqual(DateTime.MinValue, integrationEventLogEntry.CreatedAt);
            Assert.Null(integrationEventLogEntry.UpdatedAt);
            Assert.Equal(eventTypeName, integrationEventLogEntry.EventTypeName);
            Assert.Equal(content, integrationEventLogEntry.Content);
            Assert.Equal(EventState.NotPublished, integrationEventLogEntry.State);
        }

        [Fact]
        public void SetStateToInProgress_ReturnCorrectState()
        {
            // Arrange
            var integrationEventLogEntry = new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            // Act
            integrationEventLogEntry.SetStateToInProgress();

            // Assert
            Assert.Equal(EventState.InProgress, integrationEventLogEntry.State);
            Assert.NotNull(integrationEventLogEntry.UpdatedAt);
        }

        [Fact]
        public void SetStateToPublished_ReturnCorrectState()
        {
            // Arrange
            var integrationEventLogEntry = new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            // Act
            integrationEventLogEntry.SetStateToPublished();

            // Assert
            Assert.Equal(EventState.Published, integrationEventLogEntry.State);
            Assert.NotNull(integrationEventLogEntry.UpdatedAt);
        }

        [Fact]
        public void SetStateToNotPublished_ReturnCorrectState()
        {
            // Arrange
            var integrationEventLogEntry = new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            // Act
            integrationEventLogEntry.SetStateToNotPublished();

            // Assert
            Assert.Equal(EventState.NotPublished, integrationEventLogEntry.State);
            Assert.NotNull(integrationEventLogEntry.UpdatedAt);
        }

        [Fact]
        public void SetStateToPublishedFailed_ReturnCorrectState()
        {
            // Arrange
            var integrationEventLogEntry = new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            // Act
            integrationEventLogEntry.SetStateToPublishedFailed();

            // Assert
            Assert.Equal(EventState.PublishedFailed, integrationEventLogEntry.State);
            Assert.NotNull(integrationEventLogEntry.UpdatedAt);
        }
    }
}