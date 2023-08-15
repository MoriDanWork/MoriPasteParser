using MoriPattern.Data;
using MoriPattern.Res;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace MoriPattern
{
    public partial class SettingsWindow : Window
    {
        MainWindow mainWindow;
        public SettingsWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            DataContext = GlobalData.Instance;
        }


        private void TitleGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }



        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start https://t.me/MoriDanWork") { CreateNoWindow = true });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GeneralMethods.SaveSettings();
            mainWindow.IsEnabled = true;
            mainWindow.Activate();
        }

        private void ChangeLogButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new ChangeLog(this).Show();
                this.IsEnabled = false;
            }
            catch { }
        }


        private void TextBoxProxyRetries_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void TextBoxBadRecheck_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void TextBoxBadRecheck_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            GlobalData.Instance.BadRecheck = Math.Min(10, Math.Max(0, GlobalData.Instance.BadRecheck));
        }

        private void TextBoxProxyRetries_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            GlobalData.Instance.ProxyRetries = Math.Min(10, Math.Max(0, GlobalData.Instance.ProxyRetries));
        }
    }
}
