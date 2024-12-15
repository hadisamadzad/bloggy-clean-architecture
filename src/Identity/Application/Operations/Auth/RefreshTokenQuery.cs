using Common.Application.Infrastructure.Operations;
using MediatR;

namespace Identity.Application.Operations.Auth;

public record RefreshTokenQuery(string RefreshToken) : IRequest<OperationResult>;