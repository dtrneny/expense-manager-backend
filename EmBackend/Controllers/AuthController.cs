using EmBackend.DTOs.Auth;
using EmBackend.Entities;
using EmBackend.Helpers;
using EmBackend.Repositories;
using EmBackend.Repositories.Auth;
using EmBackend.Services;
using EmBackend.Services.HashService;
using EmBackend.Utilities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EmBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly IRepository<User> _userRepository;
    private readonly AuthRepository _authRepository;
    private readonly IHashService _hashService;
    private readonly FilterBuilder<User> _userFilterBuilder = new();
    private readonly FilterBuilder<RefreshToken> _tokenFilterBuilder = new();
    
    public AuthController(IRepository<User> userRepository, AuthRepository authRepository, IHashService hashService)
    {
        _userRepository = userRepository;
        _authRepository = authRepository;
        _hashService = hashService;
    }
    
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest data)
    {
        var filter = _userFilterBuilder.BuildFilterDefinition(builder =>
            builder.Eq(user => user.Email, data.Email)
        );

        if (filter == null) { return Ok("Not so ok."); }
        
        var user = await _userRepository.GetOne(filter);
        
        if (user == null) { return Ok("Not so ok."); }

        var passwordVerified = _hashService.Verify(data.Password, user.Password);

        if (!passwordVerified)
        {
            return Unauthorized();
        }

        var tokens = await _authRepository.GetJwtTokens(user);

        if (!tokens.HasValue) { return Ok("Not so ok."); }
        
        var response = new LoginResponse(tokens.Value.accessToken, tokens.Value.refreshToken);
        
        return Ok(response);
    }
    
    [HttpPost]
    [Route("refresh-access")]
    public async Task<ActionResult<RefreshAccessResponse>> RefreshAccess(RefreshAccessRequest data)
    {
        var filter = _tokenFilterBuilder.BuildFilterDefinition(builder =>
            builder.Eq(token => token.Token, data.RefreshToken)
        );

        if (filter == null) { return Ok("Not so ok."); }

        var accessToken = await _authRepository.RefreshAccessToken(filter);
        
        if (accessToken == null) { return Forbid(); }

        var response = new RefreshAccessResponse(accessToken);
        
        return Ok(response);
    }
    
    [HttpPost]
    [Route("logout")]
    public async Task<ActionResult> Logout()
    {
        var userId = _authRepository.JwtService.GetUserIdFromClaimsPrincipal(HttpContext.User);

        if (userId == null) { return Forbid(); }
        
        var filter = _tokenFilterBuilder.BuildFilterDefinition(builder =>
            builder.Eq(token => token.UserId, userId)
        );

        if (filter == null) { return Ok("Not so ok."); }

        var deleteResult = await _authRepository.DeleteRefreshToken(filter);

        if (deleteResult == null) { return NotFound(); }
        
        return Ok("Logged out");
    }
}