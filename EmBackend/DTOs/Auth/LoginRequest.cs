namespace EmBackend.DTOs.Auth;

public record LoginRequest (
    string Email,
    string Password
);
