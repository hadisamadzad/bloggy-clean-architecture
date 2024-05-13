﻿using FluentValidation;
using Identity.Application.Constants.Errors;
using Identity.Application.Operations.Auth.Models;

namespace Identity.Application.Operations.Auth;

public class SendPasswordResetEmailValidator : AbstractValidator<SendPasswordResetEmailCommand>
{
    public SendPasswordResetEmailValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithState(_ => Errors.InvalidEmailError);
    }
}