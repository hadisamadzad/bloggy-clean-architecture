using System.IdentityModel.Tokens.Jwt;

namespace Bloggy.Gateway.Core.DelegatingHandlers;

public class GlobalDelegatingHandler : DelegatingHandler
{
    private const string RequestedByHeaderKey = "RequestedBy";

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var bearer = request.Headers.Authorization?.Parameter;
        if (string.IsNullOrEmpty(bearer))
            return await base.SendAsync(request, cancellationToken);

        // Remove UserId from the request
        // NOTE This header key must not be existing in the original request
        request.Headers.Remove(RequestedByHeaderKey);

        // Read the token
        var jwtToken = new JwtSecurityTokenHandler().ReadToken(bearer) as JwtSecurityToken;

        // Add UserId to header retrieved from token
        request.Headers.Add(RequestedByHeaderKey, jwtToken.Subject);

        return await base.SendAsync(request, cancellationToken);
    }
}
