using System.Threading;
using System.Threading.Tasks;
using Blog.Application.Interfaces;
using Blog.Application.Types.Entities;
using Blog.Application.UseCases.Tags;
using Common.Utilities.OperationResult;
using NSubstitute;
using Xunit;

namespace Blog.Tests.Application.UseCases.Tags;

public class CreateTagHandlerTests
{
    private readonly IRepositoryManager _repository;
    private readonly CreateTagHandler _handler;

    public CreateTagHandlerTests()
    {
        _repository = Substitute.For<IRepositoryManager>();
        _handler = new CreateTagHandler(_repository);
    }

    [Fact]
    public async Task TestHandle_WhenTagValidationFails_ShouldReturnInvalid()
    {
        // Arrange
        var command = new CreateTagCommand("", "slug"); // Invalid name

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
    }

    [Fact]
    public async Task TestHandle_WhenDuplicateTagSlug_ShouldReturnUnprocessable()
    {
        // Arrange
        var command = new CreateTagCommand("Valid Name", "duplicate-slug");
        _repository.Tags.GetBySlugAsync(command.Slug)
            .Returns(new TagEntity { Id = string.Empty, Name = string.Empty });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Failed, result.Status);
    }

    [Fact]
    public async Task TestHandle_WhenValidRequest_ShouldCreateTag()
    {
        // Arrange
        var command = new CreateTagCommand("Valid Name", "valid-slug");
        _repository.Tags.GetBySlugAsync(command.Slug).Returns((TagEntity)null!); // No duplicate slug

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.IsType<TagEntity>(result.Value);
        await _repository.Tags.Received(1).InsertAsync(Arg.Any<TagEntity>());
    }
}