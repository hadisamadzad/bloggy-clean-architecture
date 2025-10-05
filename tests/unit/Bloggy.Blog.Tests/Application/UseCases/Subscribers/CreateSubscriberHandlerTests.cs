using System;
using System.Threading;
using System.Threading.Tasks;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Subscribers;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Utilities.OperationResult;
using NSubstitute;
using Xunit;

namespace Bloggy.Blog.Tests.Application.Operations.Subscribers;

public class CreateSubscriberOperationTests
{
    private readonly IRepositoryManager _repository;
    private readonly CreateSubscriberOperation _operation;

    public CreateSubscriberOperationTests()
    {
        _repository = Substitute.For<IRepositoryManager>();
        _operation = new CreateSubscriberOperation(_repository);
    }

    [Fact]
    public async Task ExecuteAsync_WhenValidationFails_ShouldReturnInvalid()
    {
        // Arrange
        var request = new CreateSubscriberCommand("invalidemail");

        // Act
        var response = await _operation.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.False(response.Succeeded);
        Assert.Equal(OperationStatus.Invalid, response.Status);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailIsNew_ShouldCreateNewSubscriber()
    {
        // Arrange
        var request = new CreateSubscriberCommand("test@example.com");
        var existingSubscriber = (SubscriberEntity)null!;
        _repository.Subscribers.GetByEmailAsync(request.Email).Returns(existingSubscriber);

        // Act
        var response = await _operation.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.True(response.Succeeded);
        Assert.NotNull(response.Value);
        await _repository.Subscribers.Received(1).InsertAsync(Arg.Any<SubscriberEntity>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailExists_ShouldUpdateSubscriber()
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
        var response = await _operation.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.True(response.Succeeded);
        Assert.Equal(existingSubscriber.Id, response.Value);
        await _repository.Subscribers.Received(1).UpdateAsync(existingSubscriber);
    }
}