﻿namespace WebApi.Application.DTOs.Account
{
    public class UserResponse
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
