﻿using Common.Helpers;

namespace Identity.Application.Helpers;

public static class StampGenerator
{
    public static string CreateSecurityStamp(int length)
    {
        return RandomGenerator
            .GenerateString(length, AllowedCharacters.AlphanumericCapital)
            .ToUpper();
    }
}