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

public class DeleteSubscriberOperationTests
{
    private readonly IRepositoryManager _repository;
    private readonly DeleteSubscriberOperation _operation;

    public DeleteSubscriberOperationTests()
    {
        _repository = Substitute.For<IRepositoryManager>();
        _operation = new DeleteSubscriberOperation(_repository);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailIsInvalid_ShouldReturnInvalid()
    {
        // Arrange
        var request = new DeleteSubscriberCommand("invalidemail");

        // Act
        var result = await _operation.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSubscriberNotFound_ShouldReturnNoOperation()
    {
        // Arrange
        var request = new DeleteSubscriberCommand("test@example.com");
        _repository.Subscribers.GetByEmailAsync(request.Email).Returns((SubscriberEntity)null!);

        // Act
        var result = await _operation.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(OperationStatus.NoOperation, result.Status);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSubscriberFound_ShouldUpdateSubscriberToInactive()
    {
        // Arrange
        var request = new DeleteSubscriberCommand("test@example.com");
        var existingSubscriber = new SubscriberEntity
        {
            Id = string.Empty,
            Email = request.Email,
            IsActive = true,
            UpdatedAt = DateTime.UtcNow
        };

        _repository.Subscribers.GetByEmailAsync(request.Email).Returns(existingSubscriber);

        // Act
        var result = await _operation.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(OperationStatus.Completed, result.Status);
        Assert.False(existingSubscriber.IsActive);
        await _repository.Subscribers.Received(1).UpdateAsync(existingSubscriber);
    }
}