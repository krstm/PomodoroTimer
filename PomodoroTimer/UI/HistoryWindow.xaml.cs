using PomodoroTimer.Business;
using PomodoroTimer.Entities;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PomodoroTimer
{
    /// <summary>
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        public HistoryWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void dgPrimaryGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var desc = e.PropertyDescriptor as PropertyDescriptor;
            var att = desc.Attributes[typeof(ColumnNameAttribute)] as ColumnNameAttribute;
            if (att != null)
            {
                e.Column.Header = att.Name;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HistoryManager historyManager = new HistoryManager();
            dataGridTablo.ItemsSource = historyManager.GetHistories();
        }

        public static void HataMesaji(string mesaj)
        {
            MessageBox.Show(mesaj);
        }

        private void btnKaydet_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Saniye <= 0)
            {
                MessageBoxResult result = MessageBox.Show("Geçmişi kaydetmek için program yeniden başlayacak", "Geçmişi Kaydet", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    HistoryManager historyManager = new HistoryManager();
                    historyManager.SaveHistories(dataGridTablo);

                    if (HistoryManager.uygunluk)
                    {
                        MainWindow.IsTheHistoryWindowOpen = false;
                        YenidenBaslat();
                    }
                }
            }
            else
            {
                MessageBox.Show("Program çalışırken geçmiş değiştirilemez");
            }
        }

        private void YenidenBaslat()
        {
            var currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(currentExecutablePath);
            Application.Current.Shutdown();
        }

        private void btnKapat_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.GecmisKapatilamaz = false;
            MainWindow.IsTheHistoryWindowOpen = false;
            this.Close();
        }
    }
}