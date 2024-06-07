using System.IO.Compression;
using System.Text.Json;

namespace EmBackend.Utilities;

public static class FileUtility
{
    public static byte[] GetFileBytesFromObject<T>(T obj)
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
    
    public static byte[] CreateZipFromByteArrays(Dictionary<string, byte[]> namedByteArrays)
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var kp in namedByteArrays)
            {
                var zipEntry = archive.CreateEntry(kp.Key, CompressionLevel.Fastest);
                using var zipStream = zipEntry.Open();
                zipStream.Write(kp.Value, 0, kp.Value.Length);
            }
        }
        
        return memoryStream.ToArray();
    }
}