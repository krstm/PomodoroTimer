using PomodoroTimer.Business;
using PomodoroTimer.Entities;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace PomodoroTimer
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btnKaydet_Click(object sender, RoutedEventArgs e)
        {
            if (Int32.TryParse(tbxKisaMolaSuresi.Text, out int value1) &&
            Int32.TryParse(tbxPomodoroSuresi.Text, out int value2) &&
            Int32.TryParse(tbxUzunMolaSuresi.Text, out int value3))
            {
                if (tbxKisaMolaSuresi.Text != string.Empty &&
                    tbxPomodoroSuresi.Text != string.Empty &&
                    tbxUzunMolaSuresi.Text != string.Empty)
                {
                    Setting settings = new Setting();
                    SettingManager settingManager = new SettingManager();
                    settings = settingManager.GetSettings();

                    settings.Height = (int)Application.Current.MainWindow.Height;
                    settings.Width = (int)Application.Current.MainWindow.Width;

                    settings.PomodoroSuresi = Convert.ToInt32(tbxPomodoroSuresi.Text);
                    settings.KisaMolaSuresi = Convert.ToInt32(tbxKisaMolaSuresi.Text);
                    settings.UzunMolaSuresi = Convert.ToInt32(tbxUzunMolaSuresi.Text);

                    settingManager.SaveSettings(settings);

                    MainWindow.PomodoroSuresi = settings.PomodoroSuresi;
                    MainWindow.KisaMolaSuresi = settings.KisaMolaSuresi;
                    MainWindow.UzunMolaSuresi = settings.UzunMolaSuresi;

                    MainWindow.IsTheSettingsWindowOpen = false;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Alanlar boş olamaz");
                }
            }
            else
            {
                MessageBox.Show("Alanlara sayı girmelisiniz");
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SettingManager settingManager = new SettingManager();

            tbxPomodoroSuresi.Text = settingManager.GetSettings().PomodoroSuresi.ToString();
            tbxKisaMolaSuresi.Text = settingManager.GetSettings().KisaMolaSuresi.ToString();
            tbxUzunMolaSuresi.Text = settingManager.GetSettings().UzunMolaSuresi.ToString();
        }

        private void btnIptal_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.IsTheSettingsWindowOpen = false;
            this.Close();
        }
    }
}