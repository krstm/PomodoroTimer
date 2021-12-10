using System;
using System.Collections.Generic;
using System.Text;

namespace PomodoroTimer.Entities
{
    class Setting
    {
        public int PomodoroSuresi { get; set; }
        public int KisaMolaSuresi { get; set; }
        public int UzunMolaSuresi { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public Setting()
        {
            this.PomodoroSuresi = 25;
            this.KisaMolaSuresi = 5;
            this.UzunMolaSuresi = 15;
            this.Height = 500;
            this.Width = 350;
        }
    }
}
