namespace EmBackend.Payloads.Auth;

public record LoginResponse (
    string AccessToken,
    string RefreshToken
);
