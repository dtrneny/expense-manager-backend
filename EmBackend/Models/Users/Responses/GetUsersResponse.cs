namespace EmBackend.Models.Users.Responses;

public record GetUsersResponse(
    List<UserDto> Users
);
