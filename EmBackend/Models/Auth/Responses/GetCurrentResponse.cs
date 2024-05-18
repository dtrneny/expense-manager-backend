using EmBackend.Models.Users;

namespace EmBackend.Models.Auth.Responses;

public record GetCurrentResponse (
    UserDto User    
);
