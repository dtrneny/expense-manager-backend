namespace EmBackend.DTOs.Auth;

public record LoginResponse (
    string AccessToken,
    string RefreshToken
);
