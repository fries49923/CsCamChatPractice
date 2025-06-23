# About

Just a simple C# WPF project that implements a basic video and text communication system.  
It includes two windows: **Client** and **Server**. Users can exchange text messages, and the Client captures webcam video and sends it to the Server for real-time display.

# Features

- Uses the `OpenCvSharp4.Windows` package to capture webcam frames on the Client side.
- Two separate ports are used for data transmission:
  - **TCP**: For transmitting video data.
  - **UDP**: For transmitting text messages.
- Custom communication protocol used for both video and text data.
- The Client and Server are separate windows, both supporting basic text messaging.
- The Client captures webcam video and streams it to the Server for real-time display.
- 📌 *Audio/microphone transmission is not included.*

# Screenshots

- Client window
![Screenshot00](https://github.com/fries49923/CsCamChatPractice/blob/main/Screenshot/Screenshot00.png)

- Server window
![Screenshot01](https://github.com/fries49923/CsCamChatPractice/blob/main/Screenshot/Screenshot01.png)