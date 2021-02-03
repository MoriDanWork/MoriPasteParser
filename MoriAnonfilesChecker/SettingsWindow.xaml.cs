using Microsoft.WindowsAPICodePack.Dialogs;
using MoriAnonfilesChecker.ProgramLogic;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;

namespace MoriAnonfilesChecker
{
    public partial class SettingsWindow : Window
    {
        MainWindow mainWindow;
        public SettingsWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            LoadSettings();
            this.mainWindow = mainWindow;
            this.VersionTextBlock.Text = SettingsLogic.CurrentVersion;
        }


        private void LoadSettings()
        {

        }

        private void SaveSettings()
        {

        }

        private void TitleGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
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
            SaveSettings();
            mainWindow.IsEnabled = true;
            mainWindow.Activate();
        }

        private void ChangeLogButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new ChangeLog(new HttpClient().GetStringAsync("https://raw.githubusercontent.com/MoriDanWork/MoriLogs/main/MoriProxyChecker").Result, this).Show();
                this.IsEnabled = false;
            }
            catch (Exception)
            {
            }
        }

        private void TelegramButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start tg://resolve?domain=MoriProxyChecker") { CreateNoWindow = true });
        }

        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = SettingsLogic.CurrentPath;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SettingsLogic.LoadPath = dialog.FileName;
            }
        }

        private void DefaultFolderButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsLogic.LoadPath = SettingsLogic.CurrentPath + "\\Files";
        }
    }
}
