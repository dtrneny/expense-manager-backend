namespace EmBackend.Services.Interfaces;

public interface IHashService
{
    public string? Hash(string value);
    public bool Verify(string value, string hashedValue);
}