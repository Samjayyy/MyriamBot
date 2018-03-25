using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using MyriamBot.Conversation;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFMediaKit.DirectShow.Controls;

namespace MyriamBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        private AbstractConversation _conversationState;
        public MainWindow(IFaceApiHelper faceApiHelper)
        {
            InitializeComponent();
            FaceApiHelper = faceApiHelper;
            _conversationState = new StartConversation(this);
        }
        public IFaceApiHelper FaceApiHelper { get; private set; }

        public Person ActivePerson { get; set; }
        public string TakePicture()
        {
            var control = captureElement;
            var size = new Size(control.ActualWidth, control.ActualHeight);
            control.Measure(size);
            control.Arrange(new Rect(size));
            control.UpdateLayout();
            var bmp = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Default);
            bmp.Render(control);
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            var guid = Guid.NewGuid();
            var filename = Path.Combine(App.PIC_DIR, $@"sam-{guid}.jpg");
            using (Stream stm = File.Create(filename))
            {
                encoder.Save(stm);
            }
            return filename;
        }

        public void ReplyAsBot(string msg)
        {
            AppendLineToChatBox($"[Myriam] - {msg}");
        }

        /// <summary>
        /// Send our message.
        /// </summary>
        private async Task SendMessage()
        {
            //If we have tried to send a zero length string we just return
            if (string.IsNullOrWhiteSpace(messageText.Text) || messageText.IsReadOnly) return;
            messageText.IsReadOnly = true;
            //We write our own message to the chatBox
            AppendLineToChatBox($"[You] - " + messageText.Text);
            try
            {
                _conversationState = await _conversationState.HandleUserInput(messageText.Text);
            }
            catch (FaceAPIException ex) when ("RateLimitExceeded".Equals(ex.ErrorCode))
            {
                ReplyAsBot($"My brain is overloaded with your questions.");
                ReplyAsBot("Please hold on for a while because I can only do a limited amount of requests per minute..");
                Thread.Sleep(3000);
            }
            //We clear the text within the messageText box.
            messageText.Text = "";
            messageText.IsReadOnly = false;
        }

        /// <summary>
        /// Append the provided message to the chatBox text box.
        /// </summary>
        /// <param name="message"></param>
        private void AppendLineToChatBox(string message)
        {
            //To ensure we can successfully append to the text box from any thread
            //we need to wrap the append within an invoke action.
            chatBox.Dispatcher.BeginInvoke(new Action<string>((messageToAdd) =>
            {
                chatBox.AppendText(messageToAdd + "\n");
                chatBox.ScrollToEnd();
            }), new object[] { message });
        }

        private async void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            await SendMessage();
        }

        private async void MessageText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                await SendMessage();
            }
        }

        private void cbCameras_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            captureElement.VideoCaptureSource = (string)cbCameras.SelectedItem;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbCameras.ItemsSource = MultimediaUtil.VideoInputNames;
            if (MultimediaUtil.VideoInputNames.Length > 0)
            {
                cbCameras.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No Camera!");
            }
        }
    }
}
