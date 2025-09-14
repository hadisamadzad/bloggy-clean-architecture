using Bloggy.Identity.Application.Operations.Auth;
using Bloggy.Identity.Application.Operations.PasswordReset;
using Bloggy.Identity.Application.Operations.Users;

namespace Bloggy.Identity.Application.Interfaces;

public interface IOperationService
{
    // Auth
    CheckUsernameOperation CheckUsername { get; }
    GetUserProfileOperation GetUserProfile { get; }
    GetOwnershipStatusOperation GetOwnershipStatus { get; }
    LoginOperation Login { get; }
    RegisterOperation Register { get; }
    GetNewAccessTokenOperation GetNewAccessToken { get; }

    // Users
    CreateUserOperation CreateUser { get; }
    GetUserByIdOperation GetUserById { get; }
    UpdateUserOperation UpdateUser { get; }
    UpdateUserPasswordOperation UpdateUserPassword { get; }
    UpdateUserStateOperation UpdateUserState { get; }

    // Password Reset
    SendPasswordResetEmailOperation SendPasswordResetEmail { get; }
    GetPasswordResetInfoOperation GetPasswordResetInfo { get; }
    ResetPasswordOperation ResetPassword { get; }
}