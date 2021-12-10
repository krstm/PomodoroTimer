using Microsoft.Win32;
using PomodoroTimer.Business;
using PomodoroTimer.Entities;
using System;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace PomodoroTimer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        private enum Durum
        {
            Pomodoro,
            KisaMola,
            UzunMola
        }

        private static bool IsTheHistoryWindowOpen = false;
        private static bool IsTheSettingsWindowOpen = false;
        private static int Saniye { get; set; }
        private static int Dakika { get; set; }
        private static int PomodoroSayisi { get; set; }
        private static int KisaMolaSayisi { get; set; }
        private static int UzunMolaSayisi { get; set; }
        private static int PomodoroSuresi { get; set; }
        private static int KisaMolaSuresi { get; set; }
        private static int UzunMolaSuresi { get; set; }
        private static int ToplamPomodoroDakikasi { get; set; }

        private static Durum durum;

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Saniye <= 0)
            {
                dispatcherTimer.Stop();
                ButonlariKilitle(true);
                IptalButonuGorunurlugu(false);
                BildirimCal();
            }
            else
            {
                Saniye--;
                lblSure.Content = DakikaString(Saniye) + ":" + SaniyeString(Saniye);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += dispatcherTimer_Tick;

            SettingManager settingManager = new SettingManager();
            HistoryManager historyManager = new HistoryManager();

            KisaMolaSuresi = settingManager.GetSettings().KisaMolaSuresi;
            UzunMolaSuresi = settingManager.GetSettings().UzunMolaSuresi;
            PomodoroSuresi = settingManager.GetSettings().PomodoroSuresi;
            Application.Current.MainWindow.Height = settingManager.GetSettings().Height;
            Application.Current.MainWindow.Width = settingManager.GetSettings().Width;

            if (historyManager.GetHistories().Last().Tarih == DateTime.Now.ToString("dd/MM/yyyy"))
            {
                PomodoroSayisi = historyManager.GetHistories().Last().PomodoroSayisi;
                KisaMolaSayisi = historyManager.GetHistories().Last().KisaMolaSayisi;
                UzunMolaSayisi = historyManager.GetHistories().Last().UzunMolaSayisi;
                ToplamPomodoroDakikasi = historyManager.GetHistories().Last().ToplamPomodoroDakikasi;
            }
            else
            {
                History history = new History();

                history.PomodoroSayisi = 0;
                history.KisaMolaSayisi = 0;
                history.UzunMolaSayisi = 0;
                history.ToplamPomodoroDakikasi = 0;
                history.Tarih = DateTime.Now.ToString("dd/MM/yyyy");

                historyManager.SaveHistories(history);
            }

            Saniye = 60 * PomodoroSuresi;
            lblSure.Content = DakikaString(Saniye) + ":" + SaniyeString(Saniye);

            UpdateContent();
        }

        private string DakikaString(int saniye)
        {
            if ((saniye / 60) > 9)
            {
                return (saniye / 60).ToString();
            }
            else
            {
                return "0" + (saniye / 60).ToString();
            }
        }

        private string SaniyeString(int saniye)
        {
            if ((saniye % 60) > 9)
            {
                return (saniye % 60).ToString();
            }
            else
            {
                return "0" + (saniye % 60).ToString();
            }
        }

        private void GeriSayim(int saniye, Durum _durum)
        {
            ButonlariKilitle(false);
            IptalButonuGorunurlugu(true);
            Saniye = saniye;
            durum = _durum;

            switch (durum)
            {
                case Durum.Pomodoro:
                    PomodoroSayisi++;
                    UpdateContent();
                    break;

                case Durum.KisaMola:
                    KisaMolaSayisi++;
                    UpdateContent();
                    break;

                case Durum.UzunMola:
                    UzunMolaSayisi++;
                    UpdateContent();
                    break;
            }

            dispatcherTimer.Start();
        }

        private void UpdateContent()
        {
            btnPomodoro.Content = "Pomodoro (" + PomodoroSayisi + ")";
            btnShortBreak.Content = "Kısa Mola (" + KisaMolaSayisi + ")";
            btnLongBreak.Content = "Uzun Mola (" + UzunMolaSayisi + ")";
            lblToplamPomodoro.Content = "T.P.D. : " + ToplamPomodoroDakikasi;
        }

        private void btnPomodoro_Click(object sender, RoutedEventArgs e)
        {
            GeriSayim(PomodoroSuresi * 60, Durum.Pomodoro);
        }

        private void btnShortBreak_Click(object sender, RoutedEventArgs e)
        {
            GeriSayim(KisaMolaSuresi * 60, Durum.KisaMola);
        }

        private void btnLongBreak_Click(object sender, RoutedEventArgs e)
        {
            GeriSayim(UzunMolaSuresi * 60, Durum.UzunMola);
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (Saniye > 0)
            {
                if (dispatcherTimer.IsEnabled)
                {
                    dispatcherTimer.Stop();
                    btnStop.Content = "Devam Et";
                }
                else
                {
                    dispatcherTimer.Start();
                    btnStop.Content = "Dur";
                }
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (Saniye > 0)
            {
                MessageBoxResult result = MessageBox.Show("Çıkmak istediğinize emin misiniz?", "Çıkış", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (Saniye > 0)
            {
                dispatcherTimer.Stop();
                lblSure.Content = "00:00";
                Saniye = 0;
                btnStop.Content = "Dur";
                ButonlariKilitle(true);
                IptalButonuGorunurlugu(false);

                switch (durum)
                {
                    case Durum.Pomodoro:
                        PomodoroSayisi--;
                        UpdateContent();
                        break;

                    case Durum.KisaMola:
                        KisaMolaSayisi--;
                        UpdateContent();
                        break;

                    case Durum.UzunMola:
                        UzunMolaSayisi--;
                        UpdateContent();
                        break;
                }
            }
        }

        private void btnAyarlar_Click(object sender, RoutedEventArgs e)
        {
            if (!IsTheSettingsWindowOpen)
            {
                IsTheSettingsWindowOpen = true;
                if (Saniye < 1)
                {
                    SettingsWindow settingsWindow = new SettingsWindow();
                    settingsWindow.Show();
                }
                else
                {
                    MessageBox.Show("Ayarları açmak için " + durum.ToString() + " işlemini iptal etmeniz ya da bitirmeniz gerekmektedir.");
                }
            }
        }

        private void btnGecmis_Click(object sender, RoutedEventArgs e)
        {
            if (!IsTheHistoryWindowOpen)
            {
                IsTheHistoryWindowOpen = true;
                if (Saniye < 1)
                {
                    HistoryWindow historyWindow = new HistoryWindow();
                    historyWindow.Show();
                }
                else
                {
                    MessageBox.Show("Geçmişi açmak için " + durum.ToString() + " işlemini iptal etmeniz ya da bitirmeniz gerekmektedir.");
                }
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Setting settings = new Setting();
            //SettingManager settingManager = new SettingManager();
            //settings = settingManager.GetSettings();
            //settings.Height = (int)Application.Current.MainWindow.Height;
            //settings.Width = (int)Application.Current.MainWindow.Width;

            //settingManager.SaveSettings(settings);
        }

        private void ButonlariKilitle(bool durum)
        {
            btnShortBreak.IsEnabled = durum;
            btnPomodoro.IsEnabled = durum;
            btnLongBreak.IsEnabled = durum;
        }

        private void IptalButonuGorunurlugu(bool gorunurluk)
        {
            if (gorunurluk)
            {
                btnCancel.Visibility = Visibility.Visible;
            }
            else
            {
                btnCancel.Visibility = Visibility.Hidden;
            }
        }

        private void BildirimCal()
        {
            bool found = false;
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"AppEvents\Schemes\Apps\.Default\Notification.Default\.Current"))
                {
                    if (key != null)
                    {
                        Object o = key.GetValue(null);
                        if (o != null)
                        {
                            SoundPlayer theSound = new SoundPlayer((String)o);
                            theSound.Play();
                            found = true;
                        }
                    }
                }
            }
            catch
            { }
            if (!found)
                SystemSounds.Beep.Play();
        }
    }
}