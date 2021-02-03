using MoriAnonfilesChecker.Object_Class;
using MoriAnonfilesChecker.ProgramLogic;
using MoriImapProxy.Res;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using static MoriAnonfilesChecker.GetLogic;
using static MoriAnonfilesChecker.ProgramLogic.ProxyParseLogic;
using static MoriAnonfilesChecker.ProgramLogic.SettingsLogic;

namespace MoriAnonfilesChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<MoriFile> Collection = new ObservableCollection<MoriFile>();
        public MainWindow()
        {
            InitializeComponent();
            this.Title = $"MoriAnonfiles v{SettingsLogic.CurrentVersion} | Coded by MoriDan";
            SetComboValues();
            FileInfoGrid.ItemsSource = Collection;
        }

        void SetComboValues()
        {
            var countries = new List<string>();
            foreach (var item in Enum.GetValues(typeof(Countries)).Cast<Countries>())
            {
                countries.Add(GetDisplayName(item));
            }
            this.CountryCombo.ItemsSource = countries;
            this.CountryCombo.SelectedValue = "Germany";
        }

        public string GetDisplayName(Enum enumValue)
        {
            return enumValue.GetType()?
                            .GetMember(enumValue.ToString())?
                            .First()?
                            .GetCustomAttribute<DisplayAttribute>()?
                            .Name;
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
            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = SettingsLogic.CurrentPath;
            process.Start();
            process.Close();
        }

        public void AddCollection(MoriFile moriFile)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
            {
                Collection.Add(moriFile);
                FileInfoGrid.Items.Refresh();
                DownloadLogic.AddQueue(moriFile);
            }));
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CollapseButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (Check()) return;
            SettingsLogic.Reload();
            this.StopButton.IsEnabled = true;
            this.StartButton.IsEnabled = false;
            this.LoadProxyButton.IsEnabled = false;
            this.SearchWord.IsReadOnly = true;
            this.ExtensionBox.IsReadOnly = true;
            this.CountryCombo.IsReadOnly = true;
            this.DeepAmount.IsReadOnly = true;
            this.MaxSizeAmount.IsReadOnly = true;
            GetLogic.country = GetCountry();
            MainLogic.mainWindow = this;
            DownloadLogic.mainWindow = this;
            MainLogic.StartTesting();
        }

        private bool Check()
        {
            if(SettingsLogic.ProxyCount == 0)
            {
                MoriDanMessageBox.Show("Oops, something happened :(", "Proxy list cannot be empty..", MessageBoxButton.OK);
                return true;
            }
            return false;
        }

        string GetCountry()
        {
            foreach (var item in Enum.GetValues(typeof(Countries)).Cast<Countries>())
            {
                if (GetDisplayName(item) == this.CountryCombo.Text)
                    return item.ToString();
            }
            return null;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsLogic.StopWorking();
            this.StopButton.IsEnabled = false;
            this.StopButton.Content = "Stopping";
        }

        public void Stopped()
        {
            if(SettingsLogic.ParseComplete && SettingsLogic.DownloadComplete)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    this.ProgressCount.Text = $"Completed..";
                    this.StopButton.Content = "Stop";
                    this.StartButton.IsEnabled = true;
                    this.StopButton.IsEnabled = false;
                    this.LoadProxyButton.IsEnabled = true;
                    this.SearchWord.IsReadOnly = false;
                    this.ExtensionBox.IsReadOnly = false;
                    this.CountryCombo.IsReadOnly = false;
                    this.DeepAmount.IsReadOnly = false;
                    this.MaxSizeAmount.IsReadOnly = false;
                }));
            }
        }

        private void LoadProxyButton_Click(object sender, RoutedEventArgs e)
        {
            new LoadProxyWindow(this) { Owner = this }.Show();
            this.IsEnabled = false;
        }

        public void ChangeStatus(string uid, string Status)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
            {
                foreach (MoriFile file in Collection)
                {
                    if (file.Uid == uid) file.Status = Status;
                }
            }));
        }

        public void FileCount()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
            {
                this.FilesCount.Content = Collection.Count;
            }));
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow(this) { Owner = this }.Show();
            this.IsEnabled = false;
        }
    }
}
