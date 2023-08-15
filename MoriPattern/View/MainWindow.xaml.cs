using Microsoft.Win32;
using MoriPattern.Controller;
using MoriPattern.Data;
using MoriPattern.Res;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace MoriPattern
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            GeneralMethods.LoadSettings();
            InitializeComponent();
            DataContext = GlobalData.Instance;
            //FirstStart();
        }

        public async void FirstStart()
        {
            var (isVersionMatch, flexInfo) = await ProgramInfoController.GetProgramInfo();

            if (isVersionMatch)
            {
                MoriDanMessageBox.Show("New version is available!", $"A new version {flexInfo.LastVersion} is available, please download the update from the link:\n", $"{flexInfo.NewVersionUrl}");
            }

            await ProgramInfoController.IsFirstTime();
        }

        public void TitleGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void FolderButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = Directory.GetCurrentDirectory()
            });
            MenuPopup.IsOpen = false;
        }



        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CollapseButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        public async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await StartController.Start();
            }
            catch (RankException ex)
            {
                MoriDanMessageBox.Show("Oops, something happened :(", ex.Message, MessageBoxButton.OK);
            }

        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.StopWork = true;
        }

        private void LoadProxyButton_Click(object sender, RoutedEventArgs e)
        {
            new LoadProxyWindow(this) { Owner = this }.Show();
            this.IsEnabled = false;
        }


        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow(this) { Owner = this }.Show();
            this.IsEnabled = false;
            MenuPopup.IsOpen = false;
        }

        private void OpenPopupMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuPopup.IsOpen = !MenuPopup.IsOpen;
        }

        private void TelegramButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start https://t.me/MoriDanWork") { CreateNoWindow = true });
        }

        private void TextBoxThreads_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            GlobalData.Instance.ThreadsCount = Math.Min(500, Math.Max(1, GlobalData.Instance.ThreadsCount));

        }

        private void TextBoxThreads_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void TextBoxTimeout_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void TextBoxTimeout_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            GlobalData.Instance.Timeout = Math.Min(999, Math.Max(1, GlobalData.Instance.Timeout));
        }

        private void MoriProgramWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GeneralMethods.SaveSettings();
        }

        private void CheckBoxUseProxy_Checked(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.CurrentThreads++;
        }

        private async void ButtonLoadSource_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new()
            {
                Filter = "Text Files (.txt) | *.txt"
            };
            if (openFile.ShowDialog() == true) await SourceController.LoadSourceAsync(openFile.FileName);
        }
    }
}
