using Asp.Versioning;
using EmBackend.Entities;
using EmBackend.Mappers;
using EmBackend.Models.Auth.Requests;
using EmBackend.Models.Auth.Responses;
using EmBackend.Models.Helpers;
using EmBackend.Repositories;
using EmBackend.Repositories.Interfaces;
using EmBackend.Services.Interfaces;
using EmBackend.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmBackend.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class AuthController: ControllerBase
{
    private readonly IRepository<User> _userRepository;
    private readonly AuthRepository _authRepository;
    private readonly IHashService _hashService;
    private readonly EntityMapper _entityMapper;
    
    public AuthController(
        IRepository<User> userRepository,
        AuthRepository authRepository,
        IHashService hashService,
        EntityMapper entityMapper    
    )
    {
        _userRepository = userRepository;
        _authRepository = authRepository;
        _hashService = hashService;
        _entityMapper = entityMapper;
    }
    
    [AllowAnonymous]
    [MapToApiVersion(1)]
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest data)
    {
        var filter = MongoDbDefinitionBuilder.BuildFilterDefinition<User>(builder =>
            builder.Eq(user => user.Email, data.Email)
        );
        if (filter == null) { return BadRequest("The provided data could not be utilized for filter."); }
        
        var user = await _userRepository.GetOne(filter);
        if (user == null) { return NotFound("User could not be found."); }

        var passwordVerified = _hashService.Verify(data.Password, user.Password);
        if (!passwordVerified) { return Unauthorized("The user could not be authorized."); }

        var tokens = await _authRepository.GetJwtTokens(user);
        if (!tokens.HasValue) { return Problem("The user could not be logged in."); }
        
        return Ok(new LoginResponse(tokens.Value.accessToken, tokens.Value.refreshToken));
    }
    
    [AllowAnonymous]
    [MapToApiVersion(1)]
    [HttpPost]
    [Route("refresh-access")]
    public async Task<ActionResult<RefreshAccessResponse>> RefreshAccess(RefreshAccessRequest data)
    {
        var filter = MongoDbDefinitionBuilder.BuildFilterDefinition<RefreshToken>(builder =>
            builder.Eq(token => token.Token, data.RefreshToken)
        );
        if (filter == null) { return BadRequest(new MessageResponse("The provided data could not be utilized for filter.")); }

        var accessToken = await _authRepository.RefreshAccessToken(filter);
        if (accessToken == null) { return StatusCode(403, new MessageResponse("The provided token could not be used for refresh of access token.")); }
        
        return Ok(new RefreshAccessResponse(accessToken));
    }
    
    [Authorize]
    [MapToApiVersion(1)]
    [HttpGet]
    [Route("current")]
    public async Task<ActionResult<GetCurrentResponse>> GetCurrent()
    {
        var userId = _authRepository.JwtService.GetUserIdFromClaimsPrincipal(HttpContext.User);
        if (userId == null) { return Unauthorized(); }
        
        var filter = MongoDbDefinitionBuilder.BuildFilterDefinition<User>(builder =>
            builder.Eq(user => user.Id, userId)
        );
        if (filter == null) { return BadRequest("The provided data could not be utilized for filter."); }

        var user = await _userRepository.GetOne(filter);
        if (user == null) { return NotFound("Current user was not found."); }

        var userDto = _entityMapper.UserMapper.MapUserToUserDto(user);
        if (userDto == null) { return StatusCode(500); }
        
        return Ok(new GetCurrentResponse(userDto));
    }
    
    [Authorize]
    [MapToApiVersion(1)]
    [HttpDelete]
    [Route("logout")]
    public async Task<ActionResult<MessageResponse>> Logout()
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null) { return Unauthorized("The token could not be authorized."); }
        
        var filter = MongoDbDefinitionBuilder.BuildFilterDefinition<RefreshToken>(builder =>
            builder.Eq(refToken => refToken.AccessToken, token)
        );
        if (filter == null) { return BadRequest("The provided data could not be utilized for filter."); }

        var deleteResult = await _authRepository.DeleteRefreshToken(filter);
        if (deleteResult == null) { return Problem("The user could not be logged out."); }
        
        return Ok(new MessageResponse("The user has been be logged out."));
    }
}