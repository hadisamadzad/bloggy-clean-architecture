using System.Threading;
using System.Threading.Tasks;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Tags;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Utilities.OperationResult;
using NSubstitute;
using Xunit;

namespace Bloggy.Blog.Tests.Application.Operations.Tags;

public class CreateTagOperationTests
{
    private readonly IRepositoryManager _repository;
    private readonly CreateTagOperation _operation;

    public CreateTagOperationTests()
    {
        _repository = Substitute.For<IRepositoryManager>();
        _operation = new CreateTagOperation(_repository);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTagValidationFails_ShouldReturnInvalid()
    {
        // Arrange
        var command = new CreateTagCommand("", "slug"); // Invalid name

        // Act
        var result = await _operation.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
    }

    [Fact]
    public async Task ExecuteAsync_WhenDuplicateTagSlug_ShouldReturnUnprocessable()
    {
        // Arrange
        var command = new CreateTagCommand("Valid Name", "duplicate-slug");
        _repository.Tags.GetBySlugAsync(command.Slug)
            .Returns(new TagEntity { Id = string.Empty, Name = string.Empty });

        // Act
        var result = await _operation.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Failed, result.Status);
    }

    [Fact]
    public async Task ExecuteAsync_WhenValidRequest_ShouldCreateTag()
    {
        // Arrange
        var command = new CreateTagCommand("Valid Name", "valid-slug");
        _repository.Tags.GetBySlugAsync(command.Slug).Returns((TagEntity)null!); // No duplicate slug

        // Act
        var result = await _operation.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.IsType<string>(result.Value);
        await _repository.Tags.Received(1).InsertAsync(Arg.Any<TagEntity>());
    }
}