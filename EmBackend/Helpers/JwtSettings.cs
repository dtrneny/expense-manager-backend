namespace EmBackend.Helpers;

public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int MinuteExpiration { get; set; } = 1;
    public string Key { get; set; } = string.Empty;
}