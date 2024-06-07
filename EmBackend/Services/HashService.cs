using EmBackend.Services.Interfaces;
using Scrypt;

namespace EmBackend.Services;

public class HashService: IHashService
{
    private readonly ScryptEncoder _encoder = new();
    
    public string? Hash(string value)
    {
        return _encoder.Encode(value);
    }

    public bool Verify(string value, string hashedValue)
    {
        return _encoder.Compare(value, hashedValue);
    }
}