namespace EmBackend.DTOs;

public record UserDto (
    string Id,
    string Firstname,
    string Lastname,
    string Email
);