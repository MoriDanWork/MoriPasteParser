using Microsoft.Win32;
using MoriPattern.Controller;
using MoriPattern.Data;
using MoriPattern.Res;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MoriPattern
{
    public partial class LoadProxyWindow : Window
    {
        MainWindow mainwindow;
        public LoadProxyWindow(MainWindow mainwindow)
        {
            InitializeComponent();
            this.mainwindow = mainwindow;
            DataContext = GlobalData.Instance;

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            mainwindow.Activate();
            GeneralMethods.SaveSettings();
        }

        private void TitleGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new()
            {
                Filter = "Text Files (.txt) | *.txt"
            };
            if (openFile.ShowDialog() == true)
            {
                ProxyFileCheckBox.IsChecked = true;
                GlobalData.Instance.FileProxyPath = openFile.FileName;
            }
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            bool urls = ProxyURLsCheckBox.IsChecked ?? false;
            bool files = ProxyFileCheckBox.IsChecked ?? false;
            bool clipboard = ProxyFromClipBoardCheckBox.IsChecked ?? false;

            if (!(urls || files || clipboard))
            {
                MoriDanMessageBox.Show("Oops, something happened :(", "You must select anything, before loading..", MessageBoxButton.OK);
            }
            else
            {
                ParseMode mode = ProxyController.ChooseProxyParse(urls, files, clipboard);
                await ProxyController.ParseProxyAsync(mode, GlobalData.Instance.ProxyType, GlobalData.Instance.FileProxyPath, GeneralMethods.SplitUrls(GlobalData.Instance.FileUrlsPath), GlobalData.Instance.FileUrlsPath, GeneralMethods.SplitProxies(ProxyFromClipBoardBox.Text));
                GlobalData.Instance.ProxyList = ProxyController.GetProxies();

                if (StartAfterLoadCheckBox.IsChecked == true)
                {
                    mainwindow.StartButton_Click(sender, e);
                }

                Close();
            }
        }


        private void UrlFilePathBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void UrlFilePathBox_Drop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files?.Length > 0)
            {
                GlobalData.Instance.FileUrlsPath = files[0];
                ProxyURLsCheckBox.IsChecked = true;
            }
        }

        private void FilePathBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void FilePathBox_Drop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files?.Length > 0)
            {
                GlobalData.Instance.FileProxyPath = files[0];
                ProxyFileCheckBox.IsChecked = true;
            }
        }

        private void UrlsPathBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void UrlsPathBox_PreviewDrop(object sender, DragEventArgs e)
        {
            ProxyURLsCheckBox.IsChecked = !string.IsNullOrWhiteSpace(GlobalData.Instance.FileUrlsPath) || !string.IsNullOrWhiteSpace(GlobalData.Instance.Urls);
        }



        private void LoadWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainwindow.IsEnabled = true;
            mainwindow.Activate();
        }

        private void OpenUrlsFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new()
            {
                Filter = "Text Files (.txt) | *.txt"
            };
            if (openFile.ShowDialog() == true)
            {
                ProxyURLsCheckBox.IsChecked = true;
                GlobalData.Instance.FileUrlsPath = openFile.FileName;
            }
        }

        private void LoadFromClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                ProxyFromClipBoardCheckBox.IsChecked = true;
                ProxyFromClipBoardBox.Text += $"{Clipboard.GetText()}\n";
            }
        }

        private void ProxyFromClipBoardBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProxyFromClipBoardCheckBox.IsChecked = !string.IsNullOrWhiteSpace(ProxyFromClipBoardBox.Text);
        }

        private void ClearClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            ProxyFromClipBoardBox.Text = string.Empty;
        }

        private void FilePathBox_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            ProxyFileCheckBox.IsChecked = !string.IsNullOrWhiteSpace(GlobalData.Instance.FileProxyPath);
        }

        private void UrlsPathBox_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            ProxyURLsCheckBox.IsChecked = !string.IsNullOrWhiteSpace(GlobalData.Instance.FileUrlsPath) || !string.IsNullOrWhiteSpace(GlobalData.Instance.Urls);
        }
    }
}
