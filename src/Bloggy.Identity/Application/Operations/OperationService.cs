using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Operations.Auth;
using Bloggy.Identity.Application.Operations.PasswordReset;
using Bloggy.Identity.Application.Operations.Users;
using Bloggy.Identity.Application.Types.Models.Auth;
using Bloggy.Identity.Application.Types.Models.Users;

namespace Bloggy.Identity.Application.Operations;

#pragma warning disable S107 // Avoid excessive complexity
public class OperationService(
    IOperation<CheckUsernameCommand, bool> checkUsername,
    IOperation<GetUserProfileCommand, UserModel> getUserProfile,
    IOperation<GetOwnershipStatusCommand, bool> getOwnershipStatus,
    IOperation<LoginCommand, LoginResult> login,
    IOperation<RegisterCommand, RegisterResult> register,
    IOperation<GetNewAccessTokenCommand, TokenResult> getNewAccessToken,
    IOperation<CreateUserCommand, string> createUser,
    IOperation<GetUserByIdCommand, UserModel> getUserById,
    IOperation<UpdateUserCommand, NoResult> updateUser,
    IOperation<UpdateUserPasswordCommand, NoResult> updateUserPassword,
    IOperation<UpdateUserStateCommand, NoResult> updateUserState,
    IOperation<SendPasswordResetEmailCommand, NoResult> sendPasswordResetEmail,
    IOperation<GetPasswordResetInfoCommand, string> getPasswordResetInfo,
    IOperation<ResetPasswordCommand, NoResult> resetPassword
    ) : IOperationService
#pragma warning restore S107
{
    // Auth
    public CheckUsernameOperation CheckUsername { get; } =
        checkUsername as CheckUsernameOperation;
    public GetUserProfileOperation GetUserProfile { get; } =
        getUserProfile as GetUserProfileOperation;
    public GetOwnershipStatusOperation GetOwnershipStatus { get; } =
        getOwnershipStatus as GetOwnershipStatusOperation;
    public LoginOperation Login { get; } =
        login as LoginOperation;
    public RegisterOperation Register { get; } =
        register as RegisterOperation;
    public GetNewAccessTokenOperation GetNewAccessToken { get; } =
        getNewAccessToken as GetNewAccessTokenOperation;

    // Users
    public CreateUserOperation CreateUser { get; } =
        createUser as CreateUserOperation;
    public GetUserByIdOperation GetUserById { get; } =
        getUserById as GetUserByIdOperation;
    public UpdateUserOperation UpdateUser { get; } =
        updateUser as UpdateUserOperation;
    public UpdateUserPasswordOperation UpdateUserPassword { get; } =
        updateUserPassword as UpdateUserPasswordOperation;
    public UpdateUserStateOperation UpdateUserState { get; } =
        updateUserState as UpdateUserStateOperation;

    // Password Reset
    public SendPasswordResetEmailOperation SendPasswordResetEmail { get; } =
        sendPasswordResetEmail as SendPasswordResetEmailOperation;
    public GetPasswordResetInfoOperation GetPasswordResetInfo { get; } =
        getPasswordResetInfo as GetPasswordResetInfoOperation;
    public ResetPasswordOperation ResetPassword { get; } =
        resetPassword as ResetPasswordOperation;
}