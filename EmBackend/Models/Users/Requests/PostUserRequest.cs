namespace EmBackend.Models.Users.Requests;

public record PostUserRequest (
    string Firstname,
    string Lastname,
    string Email,
    string Password
);
