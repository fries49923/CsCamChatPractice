using CsCamChatPractice.Config;
using CsCamChatPractice.Enum;
using CsCamChatPractice.Model;
using CsCamChatPractice.Tool;
using CsCamChatPractice.Util;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using static CsCamChatPractice.Util.CancellationTokenSourceUtil;
using static CsCamChatPractice.Util.StringExtensions;

namespace CsCamChatPractice.ViewModel
{
    public partial class ClientChatViewModel : ClientChatViewModelBase
    {
        protected const int udpPort = NetworkPorts.ClientUdpPort;
        protected const int serverTcpPort = NetworkPorts.ServerTcpPort;
        protected const int serverUdpPort = NetworkPorts.ServerUdpPort;

        protected TcpClient? tcpClient;
        protected UdpClient? udpClient;
        protected CancellationTokenSource? receiveMsgCts;
        protected CancellationTokenSource? sendImgCts;

        public ClientChatViewModel()
            : base()
        {
            tcpClient = null;
            udpClient = null;
            receiveMsgCts = null;
            sendImgCts = null;
        }

        protected override void WindowClosing()
        {
            _ = Disconnect();
        }

        protected override async Task ToggleConnect()
        {
            if (IsConnect is false)
            {
                await StartConnect();
            }
            else
            {
                await Disconnect();
            }
        }

        protected async Task StartConnect()
        {
            // IpAddress 格式檢查
            if (IpAddress.IsValidIPAddress() is false)
            {
                ShowErrMsgBox("Invalid IP address format.");
                return;
            }

            ResetToken(ref receiveMsgCts);

            try
            {
                await TcpConnectStart();
                UdpListenStart();

                IsConnect = true;
            }
            catch (Exception ex)
            {
                // 連線失敗
                ShowErrMsgBox(ex.Message);
                await Disconnect();
            }
        }

        protected async Task Disconnect()
        {
            receiveMsgCts?.Cancel();
            sendImgCts?.Cancel();

            tcpClient?.Close();
            tcpClient?.Dispose();
            tcpClient = null;

            udpClient?.Close();
            udpClient?.Dispose();
            udpClient = null;

            IsConnect = false;
            IsWebcamOn = false;

            await Task.CompletedTask;
        }

        protected async Task TcpConnectStart()
        {
            tcpClient = new TcpClient();

            var flag = await Task.Run(() =>
            {
                return tcpClient.ConnectAsync(IpAddress, serverTcpPort).Wait(3000);
            });

            if (flag is false)
            {
                throw new Exception("Failed to connect.");
            }
        }

        protected void UdpListenStart()
        {
            udpClient = new UdpClient(udpPort);
            _ = Task.Run(HandleUdpReceive);
        }

        protected void HandleUdpReceive()
        {
            if (udpClient is null)
            {
                ShowErrMsgBox("Udp Client is null");
                return;
            }

            if (receiveMsgCts is null)
            {
                ShowErrMsgBox("ReceiveMsgCts is null");
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
                ShowErrMsgBox("Udp Client is null");
                return;
            }

            try
            {
                var serverEP = new IPEndPoint(IPAddress.Parse(IpAddress), serverUdpPort);

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

        protected override async Task ToggleWebcam()
        {
            if (IsWebcamOn is false)
            {
                ResetToken(ref sendImgCts);
                _ = Task.Run(TcpClientSendImg);
            }
            else
            {
                sendImgCts?.Cancel();
            }

            await Task.CompletedTask;
        }

        protected async Task TcpClientSendImg()
        {
            try
            {
                if (tcpClient is null)
                {
                    throw new Exception("Tcp Client is null");
                }

                if (sendImgCts is null)
                {
                    throw new Exception("SendImgCts is null");
                }

                using var capture = new VideoCapture(0);
                if (capture.IsOpened() is false)
                {
                    throw new Exception("Unable to access the camera");
                }

                double frameWidth = capture.Get(VideoCaptureProperties.FrameWidth);
                double frameHeight = capture.Get(VideoCaptureProperties.FrameHeight);
                double fps = capture.Get(VideoCaptureProperties.Fps);
                double brightness = capture.Get(VideoCaptureProperties.Brightness);

                double width = capture.FrameWidth;
                double height = capture.FrameHeight;

                //capture.Set(VideoCaptureProperties.FrameWidth, 1920);
                //capture.Set(VideoCaptureProperties.FrameHeight, 1080);

                var frame = new Mat();
                var stream = tcpClient.GetStream();
                var jsonSer = new JsonDataSerializer();
                IsWebcamOn = true;
                int emptyCount = 0;
                while (sendImgCts.IsCancellationRequested is false)
                {
                    try
                    {
                        capture.Read(frame);

                        if (frame.Empty() is true)
                        {
                            emptyCount++;
                            if (emptyCount >= 100)
                            {
                                throw new Exception("The camera may be malfunctioning due to repeated failure to detect video frames.");
                            }

                            continue;
                        }

                        emptyCount = 0;
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    try
                    {
                        // 灰階處理
                        if (UseGrayFilter is true)
                        {
                            Cv2.CvtColor(frame, frame, ColorConversionCodes.BGR2GRAY);
                        }

                        var imgByte = frame.ToBytes();

                        // jons通訊協議
                        var imgContent =
                            new ImageContentModel()
                            {
                                Data = imgByte,
                                Channels = 3
                            };

                        var data = jsonSer.SerializeToByteAry(imgContent);
                        await stream.WriteAsync(data);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            WebcamImgSource = frame.ToBitmapSource();
                        });
                    }
                    catch (Exception)
                    { }

                    await Task.Delay(50);
                }
            }
            catch (Exception ex)
            {
                ShowErrMsgBox(ex.Message);
            }
            finally
            {
                IsWebcamOn = false;
            }
        }
    }
}
