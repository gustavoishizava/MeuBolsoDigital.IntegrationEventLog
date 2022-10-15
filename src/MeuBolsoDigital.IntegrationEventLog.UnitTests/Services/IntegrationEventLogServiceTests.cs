using MeuBolsoDigital.IntegrationEventLog.Repositories;
using MeuBolsoDigital.IntegrationEventLog.Services;
using Moq;
using Moq.AutoMock;

namespace MeuBolsoDigital.IntegrationEventLog.UnitTests.Services
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
            Assert.Equal("@event", exception.ParamName);
            Assert.Equal("Value cannot be null. (Parameter '@event')", exception.Message);
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
        public async Task ProcessEvents_NoHaveEvents_DoNothing()
        {
            // Arrange
            var repository = _autoMocker.GetMock<IIntegrationEventLogRepository>();
            repository.Setup(x => x.FindNextToPublishAsync()).ReturnsAsync((IntegrationEventLogEntry)null);

            // Act
            var result = await _service.ProcessEventsAsync(new CancellationToken(), (@event) =>
            {
                return Task.FromResult(true);
            });

            // Assert
            Assert.Equal(0, result);
            repository.Verify(x => x.FindNextToPublishAsync(), Times.Once);
            repository.Verify(x => x.UpdateAsync(It.IsAny<IntegrationEventLogEntry>()), Times.Never);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ProcessEvents_HaveOneEvent_ProcessAndUpdate(bool executionResult)
        {
            // Arrange
            var repository = _autoMocker.GetMock<IIntegrationEventLogRepository>();
            var @event = new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var state = executionResult ? EventState.Published : EventState.PublishedFailed;
            repository.SetupSequence(x => x.FindNextToPublishAsync())
                    .ReturnsAsync(@event)
                    .ReturnsAsync((IntegrationEventLogEntry)null);

            // Act
            var result = await _service.ProcessEventsAsync(new CancellationToken(), (@event) =>
            {
                return Task.FromResult(executionResult);
            });

            // Assert
            Assert.Equal(1, result);
            repository.Verify(x => x.FindNextToPublishAsync(), Times.Exactly(2));
            repository.Verify(x => x.UpdateAsync(It.Is<IntegrationEventLogEntry>(x => x.State == state)), Times.Once);
        }

        [Fact]
        public async Task ProcessEvents_HaveManyEvents_ProcessAndUpdate()
        {
            // Arrange
            var repository = _autoMocker.GetMock<IIntegrationEventLogRepository>();
            var state = EventState.Published;

            repository.SetupSequence(x => x.FindNextToPublishAsync())
                    .ReturnsAsync(new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                    .ReturnsAsync(new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                    .ReturnsAsync(new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                    .ReturnsAsync(new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                    .ReturnsAsync(new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                    .ReturnsAsync(new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                    .ReturnsAsync(new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                    .ReturnsAsync((IntegrationEventLogEntry)null);

            // Act
            var result = await _service.ProcessEventsAsync(new CancellationToken(), (@event) =>
            {
                return Task.FromResult(true);
            });

            // Assert
            Assert.Equal(7, result);
            repository.Verify(x => x.FindNextToPublishAsync(), Times.Exactly(8));
            repository.Verify(x => x.UpdateAsync(It.Is<IntegrationEventLogEntry>(x => x.State == state)), Times.Exactly(7));
        }

        [Fact]
        public async Task ProcessEvents_CancellationIsRequested_ProcessAndUpdateOneAndStop()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource(100);
            var repository = _autoMocker.GetMock<IIntegrationEventLogRepository>();
            var state = EventState.Published;

            repository.SetupSequence(x => x.FindNextToPublishAsync())
                    .ReturnsAsync(() =>
                    {
                        Task.Delay(200).Wait();
                        return new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                    })
                    .ReturnsAsync(new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                    .ReturnsAsync(new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                    .ReturnsAsync(new IntegrationEventLogEntry(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                    .ReturnsAsync((IntegrationEventLogEntry)null);

            // Act
            var result = await _service.ProcessEventsAsync(cancellationTokenSource.Token, (@event) =>
            {
                return Task.FromResult(true);
            });

            // Assert
            Assert.Equal(1, result);
            repository.Verify(x => x.FindNextToPublishAsync(), Times.Exactly(1));
            repository.Verify(x => x.UpdateAsync(It.Is<IntegrationEventLogEntry>(x => x.State == state)), Times.Exactly(1));
        }
    }
}