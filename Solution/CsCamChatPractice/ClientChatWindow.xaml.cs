using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using CsCamChatPractice.Model;
using CsCamChatPractice.ViewModel;
using System.Windows;

namespace CsCamChatPractice
{
    /// <summary>
    /// Interaction logic for ClientChatWindow.xaml
    /// </summary>
    public partial class ClientChatWindow : Window
    {
        public ClientChatWindow()
        {
            InitializeComponent();

            DataContext = Ioc.Default.GetService<ClientChatViewModelBase>();

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