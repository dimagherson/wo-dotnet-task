using System;
using System.IO;
using Newtonsoft.Json;

namespace Movies.Repository
{
    public interface IParser
    {
        public T Parse<T>(Stream stream);
    }

    public class JsonParser : IParser
    {
        public T Parse<T>(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            var serializer = JsonSerializer.Create();
            var data = serializer.Deserialize<T>(jsonTextReader);

            return data;
        }
    }
}
