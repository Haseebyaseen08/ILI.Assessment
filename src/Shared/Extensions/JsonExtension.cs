using System.Text.Json;

namespace Shared.Extensions
{
 
    public static class JsonExtension
    {
        public static object ToJson(this object obj) => JsonSerializer.Serialize(obj);
    }
    
}
