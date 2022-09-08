using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Moq.AutoMock;

namespace MeuBolsoDigital.IntegrationEventLog.UnitTests
{
    public class IntegrationEventLogServiceTests
    {
        public class FakeEvent
        {
            public int Id { get; set; }

            public FakeEvent()
            {
                Id = 1;
            }
        }

        private readonly AutoMocker _autoMocker;
        private readonly IIntegrationEventLogService _service;

        public IntegrationEventLogServiceTests()
        {
            _autoMocker = new AutoMocker();
            _service = _autoMocker.CreateInstance<IntegrationEventLogService>();
        }

        [Fact]
        public async Task Create_EventNull_ThrowsArgumentNullException()
        {
            // Arrange
            var repository = _autoMocker.GetMock<IIntegrationEventLogRepository>();

            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateEventAsync<FakeEvent>(null, "fakeEvent"));

            // Assert
            Assert.Equal("event", exception.ParamName);
            Assert.Equal("Value cannot be null. (Parameter 'event')", exception.Message);
            repository.Verify(x => x.AddAsync(It.IsAny<IntegrationEventLogEntry>()), Times.Never);
        }

        [Fact]
        public async Task CreateEvent_ReturnSuccess()
        {
            // Arrange
            var repository = _autoMocker.GetMock<IIntegrationEventLogRepository>();

            var fakeEvent = new FakeEvent();

            // Act
            await _service.CreateEventAsync<FakeEvent>(fakeEvent, Guid.NewGuid().ToString());

            // Assert
            repository.Verify(x => x.AddAsync(It.IsAny<IntegrationEventLogEntry>()), Times.Once);
        }

        [Fact]
        public async Task RetrieveEventLogPendingToPublish_ReturnSuccess()
        {
            // Arrange
            var repository = _autoMocker.GetMock<IIntegrationEventLogRepository>();
            repository.Setup(x => x.RetrieveEventLogsPendingToPublishAsync())
                .ReturnsAsync(new List<IntegrationEventLogEntry>()
                {
                    new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
                });

            // Act
            var result = await _service.RetrieveEventLogsPendingToPublishAsync();

            // Assert
            Assert.Single(result);
            repository.Verify(x => x.RetrieveEventLogsPendingToPublishAsync(), Times.Once);
        }

        [Fact]
        public async Task SetEventToPublished_IntegrationEventLogNull_ThrowsArgumentNullException()
        {
            // Arrange
            var repository = _autoMocker.GetMock<IIntegrationEventLogRepository>();

            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _service.SetEventToPublishedAsync(null));

            // Assert
            Assert.Equal("integrationEventLogEntry", exception.ParamName);
            Assert.Equal("Value cannot be null. (Parameter 'integrationEventLogEntry')", exception.Message);
            repository.Verify(x => x.UpdateAsync(It.IsAny<IntegrationEventLogEntry>()), Times.Never);
        }

        [Fact]
        public async Task SetEventToPublished_ReturnSuccess()
        {
            // Arrange
            var integrationEventLogEntry = new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var repository = _autoMocker.GetMock<IIntegrationEventLogRepository>();

            // Act
            await _service.SetEventToPublishedAsync(integrationEventLogEntry);

            // Assert
            repository.Verify(x => x.UpdateAsync(It.Is<IntegrationEventLogEntry>(x => x.State == EventStateEnum.Published)), Times.Once);
        }
    }
}