namespace CsCamChatPractice.ViewModel
{
    public abstract partial class ServerChatViewModelBase : ChatViewModelBase
    {
        public ServerChatViewModelBase()
            : base()
        {

        }

        protected abstract void StartListen();

        protected abstract void StopListen();
    }
}
