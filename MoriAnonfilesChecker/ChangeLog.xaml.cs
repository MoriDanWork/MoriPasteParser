using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MoriAnonfilesChecker
{
    /// <summary>
    /// Логика взаимодействия для ChangeLog.xaml
    /// </summary>
    public partial class ChangeLog : Window
    {
        public SettingsWindow SettingsWindow { get; }
        public ChangeLog(string log, SettingsWindow window)
        {
            this.Owner = window;
            InitializeComponent();
            ChangeLogBox.Text = log;
            this.SettingsWindow = window;
        }

        private void ChangeLogWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SettingsWindow.IsEnabled = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TitleGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
