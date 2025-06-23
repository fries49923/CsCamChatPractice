namespace CsCamChatPractice.Tool
{
    public interface IDataSerializer
    {
        byte[] SerializeToByteAry<T>(T obj) where T : class;

        T DeserializeFromByteAry<T>(byte[] byteAry) where T : class;

        string SerializeToString<T>(T obj) where T : class;

        T DeserializeFromString<T>(string str) where T : class;
    }
}
