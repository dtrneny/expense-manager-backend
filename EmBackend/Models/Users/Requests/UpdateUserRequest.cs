namespace EmBackend.Models.Users.Requests;

public record UpdateUserRequest (
    string? Firstname,
    string? Lastname,
    string? Email,
    double? Balance
);
