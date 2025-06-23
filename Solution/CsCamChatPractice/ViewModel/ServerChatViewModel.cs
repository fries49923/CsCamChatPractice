using CsCamChatPractice.Config;
using CsCamChatPractice.Enum;
using CsCamChatPractice.Model;
using CsCamChatPractice.Tool;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace CsCamChatPractice.ViewModel
{
    public partial class ServerChatViewModel : ServerChatViewModelBase
    {
        protected const int tcpPort = NetworkPorts.ServerTcpPort;
        protected const int udpPort = NetworkPorts.ServerUdpPort;
        protected const int clientUdpPort = NetworkPorts.ClientUdpPort;

        protected TcpListener? tcpListener;
        protected TcpClient? tcpClient;
        protected IPEndPoint? iPEndPoint;
        protected UdpClient? udpClient;
        protected CancellationTokenSource receiveMsgCts;

        public ServerChatViewModel()
            : base()
        {
            tcpListener = null;
            udpClient = null;

            receiveMsgCts = new();
        }

        protected override void WindowLoaded()
        {
            // 視窗啟動後開始監聽
            StartListen();
        }

        protected override void WindowClosing()
        {
            StopListen();
        }

        protected override void StartListen()
        {
            TcpListenerStart();
            UdpListenStart();
        }

        protected override void StopListen()
        {
            IsConnect = false;
            receiveMsgCts.Cancel();
            tcpClient?.Close();
            tcpListener?.Stop();
            udpClient?.Close();
        }

        protected void TcpListenerStart()
        {
            tcpListener = new TcpListener(IPAddress.Any, tcpPort);
            tcpListener.Start();

            _ = Task.Run(HandleTcpClientReceive);
        }

        protected void UdpListenStart()
        {
            udpClient = new UdpClient(udpPort);
            _ = Task.Run(HandleUdpReceive);
        }

        protected void HandleTcpClientReceive()
        {
            if (tcpListener is null)
            {
                ShowErrMsgBox("Tcp Listener is null");
                return;
            }

            while (receiveMsgCts.IsCancellationRequested is false)
            {
                try
                {
                    tcpClient = tcpListener.AcceptTcpClient();
                    if (tcpClient.Client.RemoteEndPoint is not IPEndPoint remoteEP)
                    {
                        ShowErrMsgBox("RemoteEP abnormal");
                        continue;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        IsConnect = true;
                    });

                    iPEndPoint = remoteEP;

                    var stream = tcpClient.GetStream();
                    var jsonSer = new JsonDataSerializer();
                    while (receiveMsgCts.IsCancellationRequested is false)
                    {
                        // HACK: 計算 buffer 所需大小
                        var buffer = new byte[1920 * 1080 * 3];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);

                        // 客戶端斷線時跳出迴圈
                        if (bytesRead == 0) break;

                        try
                        {
                            var validData = buffer.Take(bytesRead).ToArray();

                            // json通訊協議格式
                            var result = jsonSer.DeserializeFromByteAry<ImageContentModel>(validData);
                            var imgByte = result.Data;

                            // 轉成圖片並顯示
                            var img = Cv2.ImDecode(imgByte, ImreadModes.Color);

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                WebcamImgSource = img.ToBitmapSource();
                            });
                        }
                        catch (Exception)
                        { }

                        // HACK: 閃爍問題，可能要有Reponse的機制?
                        //byte[] response = Encoding.UTF8.GetBytes("Hello Client!");
                        //stream.Write(response, 0, response.Length);
                    }

                    IsConnect = false;
                    tcpClient?.Close();
                    tcpClient = null;
                    iPEndPoint = null;
                }
                catch (SocketException ex)
                {
                    // tcpListener.AcceptTcpClient()，當 tcpListener.Stop 時會拋出 SocketException
                    Console.WriteLine($"SocketException: {ex.ErrorCode}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"IOException: {ex.Message}");
                }
                catch (Exception)
                { }
            }
        }

        protected void HandleUdpReceive()
        {
            if (udpClient is null)
            {
                ShowErrMsgBox("Server Udp Client is null");
                return;
            }

            try
            {
                var remoteEP = new IPEndPoint(IPAddress.Any, 0);
                var jsonSer = new JsonDataSerializer();
                while (receiveMsgCts.IsCancellationRequested is false)
                {
                    byte[] buffer = udpClient.Receive(ref remoteEP);

                    try
                    {
                        // jons通訊協議
                        var result = jsonSer.DeserializeFromByteAry<TextContentModel>(buffer);
                        var message = result.Message;

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ChatMessages.Add(
                                new()
                                {
                                    Role = ChatRole.Other,
                                    Message = message,
                                    Timestamp = DateTime.Now,
                                });
                        });

                        ScrollToEndTest();
                    }
                    catch (Exception)
                    { }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"SocketException: {ex.ErrorCode}");
            }
            catch (Exception)
            { }
        }

        protected override async Task SendMessage()
        {
            if (udpClient is null)
            {
                ShowErrMsgBox("Server Udp Client is null");
                return;
            }

            if (iPEndPoint is null)
            {
                ShowErrMsgBox("Server iPEndPoint is null");
                return;
            }

            try
            {
                var serverEP = new IPEndPoint(iPEndPoint.Address, clientUdpPort);

                // jons通訊協議
                var textContent =
                    new TextContentModel()
                    {
                        Message = ChatMsg
                    };

                var jsonSer = new JsonDataSerializer();
                var data = jsonSer.SerializeToByteAry(textContent);

                // HACK: timeout機制
                await udpClient.SendAsync(data, data.Length, serverEP);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ChatMessages.Add(
                        new()
                        {
                            Role = ChatRole.You,
                            Message = ChatMsg,
                            Timestamp = DateTime.Now,
                        });
                });

                ChatMsg = "";
                ScrollToEndTest();
            }
            catch (Exception)
            { }
        }
    }
}
