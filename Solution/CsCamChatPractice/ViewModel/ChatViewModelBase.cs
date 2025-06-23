using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CsCamChatPractice.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfUi = Wpf.Ui.Controls;

namespace CsCamChatPractice.ViewModel
{
    public abstract partial class ChatViewModelBase : ObservableObject
    {
        #region Binding Property

        // 是否連線
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
        protected bool _isConnect;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
        protected string _chatMsg;

        [ObservableProperty]
        private BitmapSource? _webcamImgSource;

        // 聊天訊息內容
        public ObservableCollection<MsgModel> ChatMessages
        { get; set; }

        #endregion

        public ChatViewModelBase()
        {
            _isConnect = false;
            _chatMsg = "";
            _webcamImgSource = null;

            ChatMessages = [];
        }

        [RelayCommand]
        protected virtual void WindowLoaded()
        {

        }

        [RelayCommand]
        protected virtual void WindowClosing()
        {

        }

        protected virtual bool CanSendMessage()
        {
            return IsConnect
                && string.IsNullOrWhiteSpace(ChatMsg) is false;
        }

        [RelayCommand(CanExecute = nameof(CanSendMessage))]
        protected abstract Task SendMessage();

        protected static void ScrollToEndTest()
        {
            WeakReferenceMessenger.Default.Send<ScrollToEndMessage>();
        }

        protected virtual void ShowErrMsgBox(string msg)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var msgBox = new WpfUi.MessageBox
                {
                    Title = "Error",
                    Content = msg,
                    CloseButtonText = "OK",
                };

                msgBox.ShowDialogAsync();
            });
        }
    }
}
