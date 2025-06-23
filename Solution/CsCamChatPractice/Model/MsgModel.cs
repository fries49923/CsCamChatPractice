using CommunityToolkit.Mvvm.ComponentModel;
using CsCamChatPractice.Enum;

namespace CsCamChatPractice.Model
{
    public partial class MsgModel : ObservableObject
    {
        [ObservableProperty]
        protected ChatRole _role;

        [ObservableProperty]
        protected string _message = "";

        [ObservableProperty]
        protected DateTime _timestamp;
    }
}
