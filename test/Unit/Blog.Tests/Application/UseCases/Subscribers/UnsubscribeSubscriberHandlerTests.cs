using System;
using System.Threading;
using System.Threading.Tasks;
using Blog.Application.Interfaces;
using Blog.Application.Types.Entities;
using Blog.Application.UseCases.Subscribers;
using Common.Utilities.OperationResult;
using NSubstitute;
using Xunit;

namespace Blog.Tests.Application.UseCases.Subscribers;

public class UnsubscribeSubscriberHandlerTests
{
    private readonly IRepositoryManager _repository;
    private readonly UnsubscribeSubscriberHandler _handler;

    public UnsubscribeSubscriberHandlerTests()
    {
        _repository = Substitute.For<IRepositoryManager>();
        _handler = new UnsubscribeSubscriberHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenEmailIsInvalid_ShouldReturnInvalid()
    {
        // Arrange
        var request = new UnsubscribeSubscriberCommand("invalidemail");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
    }

    [Fact]
    public async Task Handle_WhenSubscriberNotFound_ShouldReturnIgnored()
    {
        // Arrange
        var request = new UnsubscribeSubscriberCommand("test@example.com");
        _repository.Subscribers.GetByEmailAsync(request.Email).Returns((SubscriberEntity)null!);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(OperationStatus.Ignored, result.Status);
    }

    [Fact]
    public async Task Handle_WhenSubscriberFound_ShouldUpdateSubscriberToInactive()
    {
        // Arrange
        var request = new UnsubscribeSubscriberCommand("test@example.com");
        var existingSubscriber = new SubscriberEntity
        {
            Id = string.Empty,
            Email = request.Email,
            IsActive = true,
            UpdatedAt = DateTime.UtcNow
        };

        _repository.Subscribers.GetByEmailAsync(request.Email).Returns(existingSubscriber);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(OperationStatus.Completed, result.Status);
        Assert.False(existingSubscriber.IsActive);
        await _repository.Subscribers.Received(1).UpdateAsync(existingSubscriber);
    }
}