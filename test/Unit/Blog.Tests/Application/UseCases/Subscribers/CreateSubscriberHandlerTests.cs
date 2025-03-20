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

public class CreateSubscriberHandlerTests
{
    private readonly IRepositoryManager _repository;
    private readonly CreateSubscriberHandler _handler;

    public CreateSubscriberHandlerTests()
    {
        _repository = Substitute.For<IRepositoryManager>();
        _handler = new CreateSubscriberHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnInvalid()
    {
        // Arrange
        var request = new CreateSubscriberCommand("invalidemail");
        var validation = new CreateSubscriberValidator();
        var result = validation.Validate(request);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(response.Succeeded);
        Assert.Equal(OperationStatus.Invalid, response.Status);
    }

    [Fact]
    public async Task Handle_WhenEmailIsNew_ShouldCreateNewSubscriber()
    {
        // Arrange
        var request = new CreateSubscriberCommand("test@example.com");
        var existingSubscriber = (SubscriberEntity)null!;
        _repository.Subscribers.GetByEmailAsync(request.Email).Returns(existingSubscriber);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(response.Succeeded);
        Assert.NotNull(response.Value);
        await _repository.Subscribers.Received(1).InsertAsync(Arg.Any<SubscriberEntity>());
    }

    [Fact]
    public async Task Handle_WhenEmailExists_ShouldUpdateSubscriber()
    {
        // Arrange
        var request = new CreateSubscriberCommand("test@example.com");
        var existingSubscriber = new SubscriberEntity
        {
            Id = "subscriber-1",
            Email = request.Email,
            IsActive = true,
            UpdatedAt = DateTime.UtcNow
        };

        _repository.Subscribers.GetByEmailAsync(request.Email).Returns(existingSubscriber);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);
        var responseValue = response.Value as SubscriberEntity;

        // Assert
        Assert.True(response.Succeeded);
        Assert.Equal(existingSubscriber.Email, responseValue.Email);
        await _repository.Subscribers.Received(1).UpdateAsync(existingSubscriber);
    }
}