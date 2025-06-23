using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CsCamChatPractice.ViewModel
{
    public abstract partial class ClientChatViewModelBase : ChatViewModelBase
    {
        #region Binding Property

        // IP Address
        [ObservableProperty]
        protected string _ipAddress;

        // 是否開 Webcam
        [ObservableProperty]
        protected bool _isWebcamOn;

        // 是否用 Gray Filter
        [ObservableProperty]
        protected bool _useGrayFilter;

        #endregion

        public ClientChatViewModelBase()
            : base()
        {
            _ipAddress = "127.0.0.1";
            _isWebcamOn = false;
            _useGrayFilter = false;
        }

        [RelayCommand]
        protected abstract Task ToggleConnect();

        [RelayCommand]
        protected abstract Task ToggleWebcam();
    }
}
