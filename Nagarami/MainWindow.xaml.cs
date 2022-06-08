using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace Nagarami
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string CurrentChannel { get; private set; } = string.Empty;
        private ChatWindow _chatWindow;

        public MainWindow()
        {
            InitializeComponent();

            _chatWindow = new ChatWindow(webViewWrapper, this);
        }

        private void ChannelSelectButton_Click(object sender, RoutedEventArgs e)
        {
            webView.Source = new Uri($"https://player.twitch.tv/?channel={channelTextBox.Text}&enableExtensions=true&muted=false&parent=twitch.tv&player=popout&quality=auto&volume=1");
            CurrentChannel = channelTextBox.Text;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        private void ShowChat_Checked(object sender, RoutedEventArgs e)
        {
            _chatWindow.Show();
        }

        private void ShowChat_Unchecked(object sender, RoutedEventArgs e)
        {
            _chatWindow.Hide();
        }

        private void ChatBackgroundColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (chatBackgroundColorPicker.SelectedColor is not null)
            {
                _chatWindow.ChatAreaBackground.Color = (Color)chatBackgroundColorPicker.SelectedColor;
            }
        }

        private void ChatAreaCanvasLeft_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Canvas.SetLeft(_chatWindow.chatArea, chatAreaCanvasLeft.Value);
        }

        private void ChatAreaCanvasRight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _chatWindow.chatArea.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
            Canvas.SetRight(_chatWindow.chatArea, chatAreaCanvasRight.Value);
        }

        private void ChatAreaCanvasTop_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Canvas.SetTop(_chatWindow.chatArea, chatAreaCanvasTop.Value);
        }

        private void ChatAreaCanvasBottom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _chatWindow.chatArea.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
            Canvas.SetBottom(_chatWindow.chatArea, chatAreaCanvasBottom.Value);
        }


        private void ChatAreaWidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _chatWindow.ChatAreaWidth = chatAreaWidthSlider.Value;
        }

        private void ChatAreaHeightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _chatWindow.ChatAreaHeight = chatAreaHeightSlider.Value;
        }

        private void WebViewWrapper_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                webViewWrapper.Height = e.NewSize.Width * 9 / 16;
            }

            _chatWindow.Reposition();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            _chatWindow.Reposition();
        }

        private void SettingsExpander_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _chatWindow.Reposition();
        }

        private void ChatConnectButton_Click(object sender, RoutedEventArgs e)
        {
            _chatWindow.ConnectToChat();
        }
    }
}
