using PomodoroTimer.Entities;
using System;
using System.IO;

namespace PomodoroTimer.DataAccess
{
    internal class SettingDal
    {
        public static string SettingsDbFileName { get; } = "PomodoroTimerSettings.txt";
        private string[] stringArray;
        private string settingsText;

        public Setting ReadSettings()
        {
            if (File.Exists(SettingsDbFileName))
            {
                Setting setting = new Setting();

                settingsText = File.ReadAllText(SettingsDbFileName);

                stringArray = settingsText.Split(',');
                setting.PomodoroSuresi = Convert.ToInt32(stringArray[0]);
                setting.KisaMolaSuresi = Convert.ToInt32(stringArray[1]);
                setting.UzunMolaSuresi = Convert.ToInt32(stringArray[2]);
                setting.Height = Convert.ToInt32(stringArray[3]);
                setting.Width = Convert.ToInt32(stringArray[4]);

                return setting;
            }

            return new Setting();
        }

        public void WriteSettings(Setting setting)
        {
            if (File.Exists(SettingsDbFileName))
            {
                System.IO.File.WriteAllText(SettingsDbFileName,
                setting.PomodoroSuresi.ToString() + "," +
                setting.KisaMolaSuresi.ToString() + "," +
                setting.UzunMolaSuresi.ToString() + "," +
                setting.Height.ToString() + "," +
                setting.Width.ToString());
            }
        }
    }
}