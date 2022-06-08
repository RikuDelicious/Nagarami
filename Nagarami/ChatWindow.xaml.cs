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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TwitchIRC;
using MessageProcessing;

namespace Nagarami
{
    /// <summary>
    /// ChatWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ChatWindow : Window, INotifyPropertyChanged
    {
        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly FrameworkElement _targetControl;
        private readonly Window _targetWindow;

        // ChatArea
        private SolidColorBrush _chatAreaBackground = new SolidColorBrush();
        public SolidColorBrush ChatAreaBackground
        {
            get => _chatAreaBackground;
            set
            {
                if (value != _chatAreaBackground)
                {
                    _chatAreaBackground = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private double _chatAreaWidth = 200;
        public double ChatAreaWidth {
            get => _chatAreaWidth;
            set
            {
                if (value != _chatAreaWidth)
                {
                    _chatAreaWidth = value;
                    NotifyPropertyChanged();
                }
                
            }
        }
        private double _chatAreaHeight = 100;
        public double ChatAreaHeight
        {
            get => _chatAreaHeight;
            set
            {
                if (value != _chatAreaHeight)
                {
                    _chatAreaHeight = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _autoScroll = false;
        private ObservableCollection<TextBlock> _comments = new ObservableCollection<TextBlock>();
        private int _maxComments = 100;

        public ChatWindow(FrameworkElement targetControl, Window targetWindow)
        {
            InitializeComponent();
            _targetControl = targetControl;
            _targetWindow = targetWindow;

            this.Loaded += ChatWindow_Loaded;

            commentList.ItemsSource = _comments;
        }

        private void ChatWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Owner = _targetWindow;
            this.Topmost = _targetWindow.Topmost;
            _autoScroll = true;
        }

        public void Reposition()
        {
            Point gridPoint = _targetControl.PointToScreen(new Point(0, 0));
            this.Width = _targetControl.ActualWidth;
            this.Height = _targetControl.ActualHeight;
            this.Top = gridPoint.Y;
            this.Left = gridPoint.X;
        }

        private void ChatArea_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset == chatArea.ScrollableHeight)
            {
                _autoScroll = true;
            }
            else
            {
                _autoScroll = false;
            }
        }

        private void AddComment(MessageInfo messageInfo)
        {
            Run message = new Run(messageInfo.Parameters["chat-message"]);
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.Add(message);
            textBlock.TextWrapping = TextWrapping.Wrap;

            _comments.Add(textBlock);

            if (_autoScroll)
            {
                chatArea.ScrollToBottom();
            }

            if (_comments.Count > _maxComments)
            {
                _comments.RemoveAt(0);
            }
        }

        public void ConnectToChat()
        {
            ChatClient.PRIVMSGReceived += (messageInfo) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    AddComment(messageInfo);
                });
                
            };
            ChatClient.ConnectToChat(((MainWindow)_targetWindow).CurrentChannel);
        }
    }
}
