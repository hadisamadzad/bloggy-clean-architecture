using Bloggy.Core.Utilities;

namespace Bloggy.Identity.Application.Constants;

public static class Errors
{
    const string IdentityError = "Identity Error";

    // Common
    public static readonly ErrorModel InvalidId =
        new("IDCO-100", IdentityError, "Invalid ID.");

    public static readonly ErrorModel InvalidEmail =
        new("IDCO-101", IdentityError, "Invalid email address.");

    public static readonly ErrorModel WeakPassword =
        new("IDCO-102", IdentityError, "Password is weak.");

    public static readonly ErrorModel InvalidPassword =
        new("IDCO-103", IdentityError, "Password is not provided.");

    public static readonly ErrorModel InvalidFirstName =
        new("IDCO-104", IdentityError, "Invalid first name.");

    public static readonly ErrorModel InvalidLastName =
        new("IDCO-105", IdentityError, "Invalid last name.");

    public static readonly ErrorModel InvalidUserState =
        new("IDCO-106", IdentityError, "Invalid user state.");


    // Auth
    public static readonly ErrorModel InvalidCredentials =
        new("IDAU-101", IdentityError, "Invalid credentials.");

    public static readonly ErrorModel LockedUser =
        new("IDAU-102", IdentityError, "User is locked out due to multiple failed logins.");

    public static readonly ErrorModel OwnershipAlreadyDone =
        new("IDAU-103", IdentityError, "Registration is already done.");

    public static readonly ErrorModel DuplicateUsername =
        new("IDAU-104", IdentityError, "User is already registered.");

    public static readonly ErrorModel InsufficientAccessLevel =
        new("IDAU-105", IdentityError, "Access denied.");

    public static readonly ErrorModel InvalidToken =
        new("IDAU-105", IdentityError, "Invalid token.");
}