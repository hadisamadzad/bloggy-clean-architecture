﻿using Blog.Application.Constants.Errors;
using Blog.Application.Interfaces;
using Blog.Application.Types.Entities;
using Blog.Application.Types.Models.Articles;
using Common.Utilities.OperationResult;
using MediatR;

namespace Blog.Application.UseCases.Articles;

// Handler
internal class UpdateArticleStatusHandler(IRepositoryManager unitOfWork) :
    IRequestHandler<UpdateArticleStatusCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateArticleStatusCommand request, CancellationToken cancel)
    {
        var entity = await unitOfWork.Articles.GetArticleByIdAsync(request.ArticleId);
        if (entity is null)
            return OperationResult.Failure(OperationStatus.Unprocessable, Errors.ArticleNotFound);

        // Update timestamps
        if (request.Status == ArticleStatus.Archived)
            entity.ArchivedAt = DateTime.UtcNow;

        if (request.Status == ArticleStatus.Published && entity.Status == ArticleStatus.Draft)
            entity.PublishedAt = DateTime.UtcNow;

        entity.Status = request.Status;
        entity.UpdatedAt = DateTime.UtcNow;

        _ = await unitOfWork.Articles.UpdateAsync(entity);

        return OperationResult.Success(entity.MapToModel());
    }
}

// Model
public record UpdateArticleStatusCommand(
    string ArticleId,
    ArticleStatus Status
) : IRequest<OperationResult>;