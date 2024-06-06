
using System.Text.Json;
namespace EmBackend.Services;

public class FileService
{
    public byte[] GetFileBytesFromObject<T>(T obj)
    {
        var jsonContent = JsonSerializer.Serialize(
            obj,
            new JsonSerializerOptions
            {
                WriteIndented = true
            }
        );
        
        return System.Text.Encoding.UTF8.GetBytes(jsonContent);
    }
    
}