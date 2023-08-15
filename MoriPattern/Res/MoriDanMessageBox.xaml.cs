using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;

namespace MoriPattern.Res
{
    public partial class MoriDanMessageBox : Window
    {
        public MoriDanMessageBox()
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
        }

        public MoriDanMessageBox(Window window)
        {
            Owner = window;
            InitializeComponent();
        }

        void AddButtons(MessageBoxButton buttons)
        {
            switch (buttons)
            {
                case MessageBoxButton.OK:
                    AddButton("OK", MessageBoxResult.OK);
                    break;
                case MessageBoxButton.OKCancel:
                    AddButton("OK", MessageBoxResult.OK);
                    AddButton("Cancel", MessageBoxResult.Cancel, isCancel: true);
                    break;
                case MessageBoxButton.YesNo:
                    AddButton("Yes", MessageBoxResult.Yes);
                    AddButton("No", MessageBoxResult.No);
                    break;
                case MessageBoxButton.YesNoCancel:
                    AddButton("Yes", MessageBoxResult.Yes);
                    AddButton("No", MessageBoxResult.No);
                    AddButton("Cancel", MessageBoxResult.Cancel, isCancel: true);
                    break;
                default:
                    throw new ArgumentException("Unknown button value", nameof(buttons));
            }
        }

        void AddButton(string text, MessageBoxResult result, bool isCancel = false)
        {
            var button = new Button { Style = FindResource("MoriButton") as Style, Width = 60, Height = 22, Content = text, IsCancel = isCancel };
            button.Click += (o, args) => { Result = result; DialogResult = true; };
            ButtonContainer.Children.Add(button);
        }

        MessageBoxResult Result = MessageBoxResult.None;

        public static MessageBoxResult Show(string caption, string message,
                                            MessageBoxButton buttons)
        {
            var dialog = InitializeDialog(caption, message);
            dialog.AddButtons(buttons);
            dialog.ShowDialog();
            return dialog.Result;
        }

        public static MessageBoxResult Show(string caption, string message)
        {
            var dialog = InitializeDialog(caption, message);
            dialog.AddButtons(MessageBoxButton.OK);
            dialog.ShowDialog();
            return dialog.Result;
        }

        public static MessageBoxResult Show(string caption, string message, Window window)
        {
            var dialog = InitializeDialog(caption, message, window);
            dialog.AddButtons(MessageBoxButton.OK);
            dialog.ShowDialog();
            return dialog.Result;
        }

        public static MessageBoxResult Show(string caption, string message, string url)
        {
            var dialog = InitializeDialog(caption, message);
            var hyperlink = new Hyperlink { NavigateUri = new Uri(url) };
            hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
            hyperlink.Inlines.Add(url);
            dialog.MessageContainer.Text = null;
            dialog.MessageContainer.Inlines.Add(message);
            dialog.MessageContainer.Inlines.Add(hyperlink);
            dialog.AddButtons(MessageBoxButton.OK);
            dialog.ShowDialog();
            return dialog.Result;
        }

        private static MoriDanMessageBox InitializeDialog(string caption, string message, Window window = null)
        {
            var dialog = window != null ? new MoriDanMessageBox(window) : new MoriDanMessageBox();
            dialog.MessageContainer.Text = message;
            dialog.Title = caption;
            dialog.Height += MeasureString(dialog, caption) - 50;
            return dialog;
        }

        private static void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Debug.WriteLine(e.Uri.AbsoluteUri);
            Process.Start(new ProcessStartInfo($"cmd", $"/c start {e.Uri.AbsoluteUri}") { CreateNoWindow = true });
            e.Handled = true;
        }

        private static int MeasureString(MoriDanMessageBox dialog, string candidate)
        {
            var msb = dialog.MessageContainer;
            var t = new TextBlock();
            const int columnWidth = 230;
            t.FontFamily = msb.FontFamily;
            t.FontSize = msb.FontSize;
            t.Text = msb.Text;
            t.Width = columnWidth;
            t.TextWrapping = TextWrapping.Wrap;
            t.Arrange(new Rect(0, 0, columnWidth, 1000));
            return (int)t.DesiredSize.Height;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
