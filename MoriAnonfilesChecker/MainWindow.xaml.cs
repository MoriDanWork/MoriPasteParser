using MoriAnonfilesChecker.ProgramLogic;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MoriAnonfilesChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string CurrentPath = null;
        private bool Work = false;
        public MainWindow()
        {
            CurrentPath = Directory.GetCurrentDirectory();
            InitializeComponent();
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CollapseButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //MainLogic.Pages = Convert.ToInt32(PagesAmount.Text);
            //MainLogic.StartPage = Convert.ToInt32(StartPage.Text) * 1000;
            MainLogic.Extension = ExtensionBox.Text.Split(',');
            MainLogic.StartTesting(this);
        }


        private void StopButton_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
