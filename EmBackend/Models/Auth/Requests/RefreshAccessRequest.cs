namespace EmBackend.Models.Auth.Requests;

public record RefreshAccessRequest (
    string RefreshToken
);
