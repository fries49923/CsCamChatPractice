using CsCamChatPractice.Enum;

namespace CsCamChatPractice.ViewModel
{
    public partial class ClientChatTestViewModel : ClientChatViewModelBase
    {
        public ClientChatTestViewModel()
            : base()
        {
            #region Chat Message

            ChatMessages.Add(
                new()
                {
                    Role = ChatRole.You,
                    Message = "Hey! How's your day?",
                    Timestamp = DateTime.Now.AddMinutes(-10),
                });

            ChatMessages.Add(
                new()
                {
                    Role = ChatRole.Other,
                    Message = "Not too bad! How about you?",
                    Timestamp = DateTime.Now.AddMinutes(-9),
                });

            ChatMessages.Add(
                new()
                {
                    Role = ChatRole.You,
                    Message = "Busy all day, but finally getting some time to relax.",
                    Timestamp = DateTime.Now.AddMinutes(-8),
                });

            ChatMessages.Add(
                new()
                {
                    Role = ChatRole.Other,
                    Message = "That sounds exhausting! Want to play some games to unwind?",
                    Timestamp = DateTime.Now.AddMinutes(-7),
                });

            ChatMessages.Add(
                new()
                {
                    Role = ChatRole.Other,
                    Message = "I've been playing E Ring lately. Super challenging!",
                    Timestamp = DateTime.Now.AddMinutes(-6),
                });

            ChatMessages.Add(
                new()
                {
                    Role = ChatRole.You,
                    Message = "Really? I've heard it's tough but fun. I want to try it!",
                    Timestamp = DateTime.Now.AddMinutes(-5),
                });

            ChatMessages.Add(
                new()
                {
                    Role = ChatRole.Other,
                    Message = "Oh, for sure. The bosses are insane, but super rewarding once you beat them.",
                    Timestamp = DateTime.Now.AddMinutes(-4),
                });

            ChatMessages.Add(
                new()
                {
                    Role = ChatRole.Other,
                    Message = "We could team up and help each other get through some fights!",
                    Timestamp = DateTime.Now.AddMinutes(-3),
                });

            ChatMessages.Add(
                new()
                {
                    Role = ChatRole.You,
                    Message = "That would be awesome! Alright, I'll go set up. See you in a bit!",
                    Timestamp = DateTime.Now.AddMinutes(-2),
                });

            ChatMessages.Add(
                new()
                {
                    Role = ChatRole.Other,
                    Message = "Cool! See you soon!",
                    Timestamp = DateTime.Now.AddMinutes(-1),
                });

            #endregion
        }

        protected override async Task ToggleConnect()
        {
            IsConnect = !IsConnect;
            await Task.CompletedTask;
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

        protected override async Task ToggleWebcam()
        {
            IsWebcamOn = !IsWebcamOn;
            await Task.CompletedTask;
        }
    }
}
