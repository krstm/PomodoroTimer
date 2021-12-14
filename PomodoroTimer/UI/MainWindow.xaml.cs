using Microsoft.Toolkit.Uwp.Notifications;
using PomodoroTimer.Business;
using PomodoroTimer.Entities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PomodoroTimer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
        }

        public static System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        private enum Durum
        {
            Pomodoro,
            KisaMola,
            UzunMola,
            Bos
        }

        public static bool IsTheHistoryWindowOpen = false;
        public static bool IsTheSettingsWindowOpen = false;
        public static bool GecmisKapatilamaz = false;
        public static int Saniye { get; set; }
        public static int PomodoroSayisi { get; set; }
        public static int KisaMolaSayisi { get; set; }
        public static int UzunMolaSayisi { get; set; }
        public static int PomodoroSuresi { get; set; }
        public static int KisaMolaSuresi { get; set; }
        public static int UzunMolaSuresi { get; set; }
        public static int ToplamPomodoroDakikasi { get; set; }

        private static Durum durum;

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Saniye <= 0)
            {
                dispatcherTimer.Stop();
                ButonlariKilitle(true);
                IptalButonuGorunurlugu(false);

                Kaydet();
                btnStop.Visibility = Visibility.Hidden;
                BildirimCal();

                durum = Durum.Bos;
                BasliklariKalinlastir(durum);
                TaskbarItemInfo.ProgressValue = 1;
            }
            else
            {
                Saniye--;
                lblSure.Content = DakikaString(Saniye) + ":" + SaniyeString(Saniye);

                switch (durum)
                {
                    case Durum.Pomodoro:
                        TaskbarItemInfo.ProgressValue = ((float)((((float)PomodoroSuresi * 60) -
                            (float)Saniye) / ((float)PomodoroSuresi * 60)));
                        break;

                    case Durum.KisaMola:
                        TaskbarItemInfo.ProgressValue = ((float)((((float)KisaMolaSuresi * 60) -
                            (float)Saniye) / ((float)KisaMolaSuresi * 60)));
                        break;

                    case Durum.UzunMola:
                        TaskbarItemInfo.ProgressValue = ((float)((((float)UzunMolaSuresi * 60) -
                            (float)Saniye) / ((float)UzunMolaSuresi * 60)));
                        break;

                    case Durum.Bos:
                        break;

                    default:
                        break;
                }
            }
        }

        private void Kaydet()
        {
            History history = new History();
            HistoryManager historyManager = new HistoryManager();

            if (durum == Durum.Pomodoro)
            {
                ToplamPomodoroDakikasi += PomodoroSuresi;
                UpdateContent();
            }

            history.Tarih = DateTime.Now.ToString("dd/MM/yyyy");
            history.PomodoroSayisi = PomodoroSayisi;
            history.KisaMolaSayisi = KisaMolaSayisi;
            history.UzunMolaSayisi = UzunMolaSayisi;
            history.ToplamPomodoroDakikasi = ToplamPomodoroDakikasi;

            historyManager.SaveHistory(history);

            if (IsTheHistoryWindowOpen)
            {
                GecmisKapatilamaz = true;
                MessageBox.Show("Geçmiş veritabanına yeni kayıt yapıldı. Hata olmaması için Geçmiş penceresini kaydetmeden kapatmanız gerekmektedir");
            }
        }

        private static void YenidenBaslat()
        {
            var currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(currentExecutablePath);
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VeritabanlariniKontrolEt();

            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += dispatcherTimer_Tick;

            SettingManager settingManager = new SettingManager();
            HistoryManager historyManager = new HistoryManager();

            try
            {
                KisaMolaSuresi = settingManager.GetSettings().KisaMolaSuresi;
                UzunMolaSuresi = settingManager.GetSettings().UzunMolaSuresi;
                PomodoroSuresi = settingManager.GetSettings().PomodoroSuresi;

                Application.Current.MainWindow.Height = settingManager.GetSettings().Height;
                Application.Current.MainWindow.Width = settingManager.GetSettings().Width;
            }
            catch (Exception)
            {
                AyarlardaHata();
            }

            try
            {
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

                    historyManager.SaveHistory(history);
                }
            }
            catch (Exception)
            {
                GecmisteHata();
            }

            UpdateContent();
        }

        public static void GecmisteHata()
        {
            MessageBoxResult result = MessageBox.Show("Geçmiş veritabanında hata oluştu. Veritabanı sıfırdan oluşturulsun mu?", "Geçmiş Veritabanında Hata", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                System.IO.File.WriteAllText("PomodoroTimerDb.txt", DateTime.Now.ToString("dd/MM/yyyy") + ",0,0,0,0");
                YenidenBaslat();
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        public static void AyarlardaHata()
        {
            MessageBoxResult result = MessageBox.Show("Ayarlar veritabanında hata oluştu. Veritabanı sıfırdan oluşturulsun mu?", "Ayarlar Veritabanında Hata", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                System.IO.File.WriteAllText("PomodoroTimerSettings.txt", "25,5,15,500,350");
                YenidenBaslat();
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void VeritabanlariniKontrolEt()
        {
            if (!File.Exists("PomodoroTimerDb.txt"))
            {
                System.IO.File.WriteAllText("PomodoroTimerDb.txt", DateTime.Now.ToString("dd/MM/yyyy") + ",0,0,0,0");
                MessageBox.Show("Geçmiş veritabanı bulunamadı. Sıfırdan oluşturuldu.");
            }
            if (!File.Exists("PomodoroTimerSettings.txt"))
            {
                System.IO.File.WriteAllText("PomodoroTimerSettings.txt", "25,5,15,500,350");
                MessageBox.Show("Ayarlar veritabanı bulunamadı. Sıfırdan oluşturuldu.");
            }
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
            BasliklariKalinlastir(durum);

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

        public void UpdateContent()
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
            EkranBoyutunuKaydet();

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
            MessageBoxResult result = MessageBox.Show("İptal etmek istediğinize emin misiniz?", "İptal", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (Saniye > 0)
                {
                    dispatcherTimer.Stop();
                    lblSure.Content = "00:00";
                    Saniye = 0;
                    btnStop.Content = "Dur";

                    ButonlariKilitle(true);
                    IptalButonuGorunurlugu(false);
                    TaskbarItemInfo.ProgressValue = 0;

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
                    durum = Durum.Bos;
                    BasliklariKalinlastir(durum);
                }
            }
        }

        private void btnAyarlar_Click(object sender, RoutedEventArgs e)
        {
            if (!IsTheSettingsWindowOpen)
            {
                IsTheSettingsWindowOpen = true;

                SettingsWindow settingsWindow = new SettingsWindow();
                settingsWindow.Show();
            }
            else
            {
                MessageBox.Show("Ayarlar penceresi zaten açık");
            }
        }

        private void btnGecmis_Click(object sender, RoutedEventArgs e)
        {
            if (!IsTheHistoryWindowOpen)
            {
                IsTheHistoryWindowOpen = true;

                HistoryWindow historyWindow = new HistoryWindow();
                historyWindow.Show();
            }
            else
            {
                MessageBox.Show("Geçmiş penceresi zaten açık");
            }
        }

        private void EkranBoyutunuKaydet()
        {
            if (Application.Current.MainWindow.Height >= 500 &&
                Application.Current.MainWindow.Width >= 350)
            {
                Setting settings = new Setting();
                SettingManager settingManager = new SettingManager();
                settings = settingManager.GetSettings();
                settings.Height = (int)Application.Current.MainWindow.Height;
                settings.Width = (int)Application.Current.MainWindow.Width;

                settingManager.SaveSettings(settings);
            }
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
            new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddText(DurumStr(durum) + " bitti")
                .Show();
        }

        private string DurumStr(Durum _durum)
        {
            switch (_durum)
            {
                case Durum.Pomodoro:
                    return "Pomodoro";
                    break;

                case Durum.KisaMola:
                    return "Kısa Mola";
                    break;

                case Durum.UzunMola:
                    return "Uzun Mola";
                    break;

                default:
                    return "Hata";
                    break;
            }
        }

        private void BasliklariKalinlastir(Durum _durum)
        {
            switch (_durum)
            {
                case Durum.Pomodoro:
                    btnPomodoro.FontWeight = FontWeights.Bold;
                    btnShortBreak.FontWeight = FontWeights.ExtraLight;
                    btnLongBreak.FontWeight = FontWeights.ExtraLight;
                    btnStop.Visibility = Visibility.Visible;
                    break;

                case Durum.KisaMola:
                    btnShortBreak.FontWeight = FontWeights.Bold;
                    btnLongBreak.FontWeight = FontWeights.ExtraLight;
                    btnPomodoro.FontWeight = FontWeights.ExtraLight;
                    btnStop.Visibility = Visibility.Visible;
                    break;

                case Durum.UzunMola:
                    btnLongBreak.FontWeight = FontWeights.Bold;
                    btnShortBreak.FontWeight = FontWeights.ExtraLight;
                    btnPomodoro.FontWeight = FontWeights.ExtraLight;
                    btnStop.Visibility = Visibility.Visible;
                    break;

                default:
                    btnPomodoro.FontWeight = FontWeights.Normal;
                    btnLongBreak.FontWeight = FontWeights.Normal;
                    btnShortBreak.FontWeight = FontWeights.Normal;
                    btnStop.Visibility = Visibility.Hidden;
                    break;
            }
        }
    }
}