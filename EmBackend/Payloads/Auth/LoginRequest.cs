namespace EmBackend.Payloads.Auth;

public record LoginRequest (
    string Email,
    string Password
);
