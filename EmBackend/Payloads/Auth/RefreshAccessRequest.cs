namespace EmBackend.Payloads.Auth;

public record RefreshAccessRequest (
    string RefreshToken
);
