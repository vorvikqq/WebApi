﻿namespace WebApi.Application.Settings
{
    public class JwtSettings
    {
        public string SigningKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
