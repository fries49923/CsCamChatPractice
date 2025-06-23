using System.Text.Json;

namespace CsCamChatPractice.Tool
{
    public class JsonDataSerializer : IDataSerializer
    {
        private readonly JsonSerializerOptions jsonOptions;

        public JsonDataSerializer()
        {
            jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
        }

        public byte[] SerializeToByteAry<T>(T obj)
            where T : class
        {
            try
            {
                return JsonSerializer.SerializeToUtf8Bytes(obj);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T DeserializeFromByteAry<T>(byte[] byteAry)
            where T : class
        {
            try
            {
                var obj = JsonSerializer.Deserialize<T>(byteAry);

                if (obj is not T result)
                {
                    throw new Exception("Deserialize fail");
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string SerializeToString<T>(T obj)
            where T : class
        {
            try
            {
                return JsonSerializer.Serialize(obj, jsonOptions);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T DeserializeFromString<T>(string str)
            where T : class
        {
            try
            {
                var obj = JsonSerializer.Deserialize<T>(str);

                if (obj is not T result)
                {
                    throw new Exception("Deserialize fail");
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
