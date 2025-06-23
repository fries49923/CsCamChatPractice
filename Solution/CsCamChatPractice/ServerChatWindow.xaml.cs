using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using CsCamChatPractice.Model;
using CsCamChatPractice.ViewModel;
using System.Windows;

namespace CsCamChatPractice
{
    /// <summary>
    /// ServerChatWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ServerChatWindow : Window
    {
        public ServerChatWindow()
        {
            InitializeComponent();

            DataContext = Ioc.Default.GetService<ServerChatViewModelBase>();

            WeakReferenceMessenger.Default.Register<ScrollToEndMessage>(
                this, (r, m) => ChatMsgScrollToEnd());
        }

        private void ChatMsgScrollToEnd()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var listBox = lbChatMsg;
                if (listBox.Items.Count > 0)
                {
                    var lastItem = listBox.Items[^1];
                    listBox.ScrollIntoView(lastItem);
                }
            });
        }
    }
}
