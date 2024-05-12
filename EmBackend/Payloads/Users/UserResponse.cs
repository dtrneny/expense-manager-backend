using EmBackend.DTOs;

namespace EmBackend.Payloads.Users;

public record UserResponse (
    UserDto User
);
