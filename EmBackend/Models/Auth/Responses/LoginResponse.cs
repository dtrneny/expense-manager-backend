namespace EmBackend.Models.Auth.Responses;

public record LoginResponse (
    string AccessToken,
    string RefreshToken
);
