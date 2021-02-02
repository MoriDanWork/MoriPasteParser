using MoriAnonfilesChecker.Object_Class;
using MoriAnonfilesChecker.ProgramLogic;
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

namespace MoriAnonfilesChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string CurrentPath = null;
        public List<Proxy> proxies;
        public ObservableCollection<MoriFile> Collection { get; set; }
        public List<MoriFile> Collection2 { get; set; }

        MoriFile test;
        public MainWindow()
        {
            CurrentPath = Directory.GetCurrentDirectory();
            Collection = new ObservableCollection<MoriFile>();
            InitializeComponent();
            SetComboValues();
            test = new MoriFile() { DownloadURL = "https://cdn-117.anonfiles.com/g4q6If07n2/1c5af2e2-1612216348/Good.txt", Url = "https://123.com", Extension = ".exe", SizeReadable = "1", Status = "Pending", Name = "test", Size = 1000, DownloadRetry = 0, Uid = "1" };
            FileInfoGrid.ItemsSource = Collection;
            Debug.WriteLine("Start");
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
            process.StartInfo.FileName = CurrentPath;
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

        public void TakeProxies(List<Proxy> proxieslist)
        {
            this.proxies = proxieslist;
            this.ProxyCount.Content = proxies.Count;
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
            MainLogic.Pages = Convert.ToInt32(PagesAmount.Text);
            this.StopButton.IsEnabled = true;
            this.StartButton.IsEnabled = false;
            this.LoadProxyButton.IsEnabled = false;
            this.ProgressCount.Text = $"Working [{this.ProgressBlock.Value}/{MainLogic.Pages}]";
            this.ProgressBlock.Maximum = MainLogic.Pages;
            MainLogic.Request = SearchWord.Text.Replace(" ", "%20");
            GetLogic.country = GetCountry();
            MainLogic.Extension = ExtensionBox.Text.Split(';');
            MainLogic.mainWindow = this;
            MainLogic.MaxSize = Convert.ToInt32(MaxSizeAmount.Text) * 1000000;
            DownloadLogic.mainWindow = this;
            MainLogic.StartTesting();
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

        public void AddError()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
            {
                this.ErrorsCount.Content = Convert.ToInt32(ErrorsCount.Content) + 1;
            }));
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            MainLogic.ThreadsCount = 0;
            MainLogic.StopParse = true;
            DownloadLogic.ThreadsCount = 0;
            this.StopButton.IsEnabled = false;
            this.StopButton.Content = "Stopping";
        }

        public void Stopped()
        {
            if(MainLogic.AllParsed && DownloadLogic.AllDownloaded)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    this.ProgressCount.Text = $"Completed..";
                    this.StopButton.Content = "Stop";
                    this.StartButton.IsEnabled = true;
                    this.StopButton.IsEnabled = false;
                    this.LoadProxyButton.IsEnabled = true;
                }));
            }
        }

        private void ThreadsAmount_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ThreadsAmount_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TimeoutAmount_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TimeoutAmount_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void LoadProxyButton_Click(object sender, RoutedEventArgs e)
        {
            new LoadProxyWindow(this) { Owner = this }.Show();
            this.IsEnabled = false;
        }

        public void AddProgress()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
            {
                this.ProgressBlock.Value += 1;
                this.ProgressCount.Text = $"Working [{this.ProgressBlock.Value}/{this.ProgressBlock.Maximum}]";
            }));
        }

        public void BadProxy()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
            {
                this.ProxyCount.Content = Convert.ToInt32(ProxyCount.Content) - 1;
            }));
        }

        public void AddFilterCount()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
            {
                this.FilterCount.Content = Convert.ToInt32(FilterCount.Content) + 1;
            }));
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

        public void AddLinksCount()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
            {
                this.LinksCount.Content = Convert.ToInt32(LinksCount.Content) + 1;
            }));
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
