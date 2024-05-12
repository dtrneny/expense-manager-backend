namespace EmBackend.Models.Users;

public record UserDto (
    string Id,
    string Firstname,
    string Lastname,
    string Email
);