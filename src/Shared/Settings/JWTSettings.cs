﻿namespace Shared.Settings
{
    public record JWTSettings
    {
        public required string SecretKey { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public int ExpiryMinutes { get; set; } = 60;
    }
}