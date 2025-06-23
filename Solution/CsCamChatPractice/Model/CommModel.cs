using CsCamChatPractice.Enum;

namespace CsCamChatPractice.Model
{
    public abstract class CommModelBase
    {
        public ContentType Type { get; set; }
    }

    public class TextContentModel : CommModelBase
    {
        public string Message
        { get; set; } = "";

        public TextContentModel()
        {
            Type = ContentType.Text;
        }
    }

    public class ImageContentModel : CommModelBase
    {
        public byte[] Data
        { get; set; } = [];

        public byte Channels
        { get; set; } = 3;

        public ImageContentModel()
        {
            Type = ContentType.Image;
        }
    }
}
