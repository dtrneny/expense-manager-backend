namespace EmBackend.Models.Users.Requests;

public record UpdateUserRequest (
    string Id,
    string? Firstname,
    string? Lastname,
    string? Email,
    double? Balance
);
