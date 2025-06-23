using CsCamChatPractice.Enum;

namespace CsCamChatPractice.ViewModel
{
    public partial class ServerChatTestViewModel : ServerChatViewModelBase
    {
        public ServerChatTestViewModel()
            : base()
        {
            IsConnect = true;
        }

        protected override void StartListen()
        {
            throw new NotImplementedException();
        }

        protected override void StopListen()
        {
            throw new NotImplementedException();
        }

        protected override async Task SendMessage()
        {
            ChatMessages.Add(
                new()
                {
                    Role = ChatRole.You,
                    Message = ChatMsg,
                    Timestamp = DateTime.Now,
                });

            ChatMsg = "";
            ScrollToEndTest();
            await Task.CompletedTask;
        }
    }
}
