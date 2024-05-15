using EmBackend.Entities;
using EmBackend.Models.Users;
using Riok.Mapperly.Abstractions;

namespace EmBackend.Utilities.Mappers;

[Mapper]
public partial class UserMapper
{
    public partial UserDto MapUserToUserDto(User entity);
}
