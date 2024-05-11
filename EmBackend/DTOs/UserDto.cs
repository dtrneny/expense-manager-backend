namespace EmBackend.DTOs;

public record class UserDto(
    string Id,
    string Firstname,
    string Lastname,
    string Email
);