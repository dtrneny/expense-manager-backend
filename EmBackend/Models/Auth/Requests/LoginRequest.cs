namespace EmBackend.Models.Auth.Requests;

public record LoginRequest (
    string Email,
    string Password
);
