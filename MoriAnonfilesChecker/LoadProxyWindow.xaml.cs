using Microsoft.Win32;
using MoriAnonfilesChecker.ProgramLogic;
using MoriImapProxy.Res;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static MoriAnonfilesChecker.ProgramLogic.ProxyParseLogic;

namespace MoriAnonfilesChecker
{
    public partial class LoadProxyWindow : Window
    {
        MainWindow mainwindow;
        ProxyParseLogic proxyParse;
        public LoadProxyWindow(MainWindow mainwindow)
        {
            InitializeComponent();
            this.mainwindow = mainwindow;
            LoadSettings();
        }

        private void LoadSettings()
        {
            FilePathBox.Text = SettingsLogic.ReadSetting("FilePath");
            UrlsPathBox.Text = SettingsLogic.ReadSetting("UrlsPath");
            ProxyTypeBox.Text = SettingsLogic.ReadSetting("ProxyType");
            ProxyFileCheckBox.IsChecked = false;
            ProxyURLsCheckBox.IsChecked = false;
        }

        private void SaveSettings()
        {
            SettingsLogic.UpdateSetting("FilePath", FilePathBox.Text);
            SettingsLogic.UpdateSetting("UrlsPath", UrlsPathBox.Text);
            SettingsLogic.UpdateSetting("ProxyType", ProxyTypeBox.Text);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            mainwindow.Activate();
        }

        private void TitleGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Filter = "Text Files (.txt) | *.txt"
            };
            if (openFile.ShowDialog() == true)
            {
                ProxyFileCheckBox.IsChecked = true;
                FilePathBox.Text = openFile.FileName;
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            bool urls = Convert.ToBoolean(ProxyURLsCheckBox.IsChecked);
            bool files = Convert.ToBoolean(ProxyFileCheckBox.IsChecked);

            if (files && String.IsNullOrEmpty(FilePathBox.Text))
            {
                MoriDanMessageBox.Show("Oops, something happened :(", "Files cannot be empty..", MessageBoxButton.OK);
            }
            else
            {

                if (urls && String.IsNullOrEmpty(UrlsPathBox.Text))
                {
                    MoriDanMessageBox.Show("Oops, something happened :(", "File path cannot be empty..", MessageBoxButton.OK);
                }
                else
                {

                    try
                    {
                        switch (ProxyParseLogic.ChooseProxyParse(urls, files))
                        {
                            case 1:
                                proxyParse = new ProxyParseLogic(FilePathBox.Text, ProxyParseLogic.SplitUrls(UrlsPathBox.Text), ProxyParseLogic.GetProxyType(ProxyTypeBox.SelectedIndex));
                                this.Close();
                                return;
                            case 2:
                                proxyParse = new ProxyParseLogic(ProxyParseLogic.SplitUrls(UrlsPathBox.Text), ProxyParseLogic.GetProxyType(ProxyTypeBox.SelectedIndex));
                                this.Close();
                                return;
                            case 3:
                                proxyParse = new ProxyParseLogic(FilePathBox.Text, ProxyParseLogic.GetProxyType(ProxyTypeBox.SelectedIndex));
                                this.Close();
                                return;
                            case 0:
                                MoriDanMessageBox.Show("Oops, something happened :(", "You must select anything, before loading..", MessageBoxButton.OK);
                                break;
                        }
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        MoriDanMessageBox.Show("Oops, something happened :(", "File not found..", MessageBoxButton.OK);
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {
                        MoriDanMessageBox.Show("Oops, something happened :(", "Folder not found..", MessageBoxButton.OK);
                    }
                    catch (System.Net.WebException)
                    {
                        MoriDanMessageBox.Show("Oops, something happened :(", "One of the links is not working :/", MessageBoxButton.OK);
                    }
                }
            }
        }


        private void FilePathBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void FilePathBox_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length != 0)
            {
                FilePathBox.Text = files[0];
                ProxyFileCheckBox.IsChecked = true;
            }
        }

        private void UrlsPathBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void UrlsPathBox_PreviewDrop(object sender, DragEventArgs e)
        {
            ProxyURLsCheckBox.IsChecked = true;
        }

        private void UrlsPathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProxyURLsCheckBox.IsChecked = true;
        }

        private void FilePathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProxyFileCheckBox.IsChecked = true;
        }

        private void LoadWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (proxyParse != null)
               SettingsLogic.Proxies = proxyParse.GetProxies();
            mainwindow.IsEnabled = true;
            SaveSettings();
        }
    }
}
