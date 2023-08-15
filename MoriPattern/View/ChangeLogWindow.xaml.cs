using MoriPattern.Data;
using System.Windows;
using System.Windows.Input;

namespace MoriPattern
{
    /// <summary>
    /// Логика взаимодействия для ChangeLog.xaml
    /// </summary>
    public partial class ChangeLog : Window
    {
        public SettingsWindow SettingsWindow { get; }

        public ChangeLog(SettingsWindow window)
        {
            Owner = window;
            InitializeComponent();
            ChangeLogBox.Text = string.Join("", GlobalData.Instance.ProgramInfo.ChangeLog);
            SettingsWindow = window;
            DataContext = GlobalData.Instance;
        }

        private void ChangeLogWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SettingsWindow.IsEnabled = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }
    }

}
