using EmBackend.DTOs;

namespace EmBackend.Payloads.Users;

public record UsersResponse (
    List<UserDto> Users
);