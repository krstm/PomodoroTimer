using System;
using System.Collections.Generic;
using System.Text;

namespace PomodoroTimer.Entities
{
    class History
    {
        [ColumnName("Sıra")]
        public int Id { get; set; }
        [ColumnName("Tarih")]
        public string Tarih { get; set; }
        [ColumnName("Pomodoro Sayısı")]
        public int PomodoroSayisi { get; set; }
        [ColumnName("Kısa Mola Sayısı")]
        public int KisaMolaSayisi { get; set; }
        [ColumnName("Uzun Mola Sayısı")]
        public int UzunMolaSayisi { get; set; }
        [ColumnName("Toplam Pomodoro Dakikası")]
        public int ToplamPomodoroDakikasi { get; set; }

        public History()
        {
            this.Id = 1;
            this.Tarih = DateTime.Now.ToString("dd/MM/yyyy");
            this.PomodoroSayisi = 0;
            this.KisaMolaSayisi = 0;
            this.UzunMolaSayisi = 0;
            this.ToplamPomodoroDakikasi = 0;
        }
    }
}
