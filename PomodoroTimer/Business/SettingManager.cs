using PomodoroTimer.DataAccess;
using PomodoroTimer.Entities;
using System;
using System.IO;

namespace PomodoroTimer.Business
{
    internal class SettingManager
    {
        public static string SettingsDatabaseFileName { get; } = "PomodoroTimerSettings.txt";
        private SettingDal settingDal = new SettingDal();

        internal Setting GetSettings()
        {
            if (File.Exists(SettingDal.SettingsDbFileName))
            {
                if (
                (Int32.TryParse(settingDal.ReadSettings().Height.ToString(), out int value1)) &&
                (Int32.TryParse(settingDal.ReadSettings().Width.ToString(), out int value2)) &&
                (Int32.TryParse(settingDal.ReadSettings().PomodoroSuresi.ToString(), out int value3)) &&
                (Int32.TryParse(settingDal.ReadSettings().KisaMolaSuresi.ToString(), out int value4)) &&
                (Int32.TryParse(settingDal.ReadSettings().UzunMolaSuresi.ToString(), out int value5)))
                {
                    return settingDal.ReadSettings();
                }
                else
                {
                    return new Setting();
                    MainWindow.AyarlardaHata();
                }
            }
            else
            {
                return new Setting();
                MainWindow.AyarlardaHata();
            }
        }

        public void SaveSettings(Setting setting)
        {
            if (File.Exists(SettingDal.SettingsDbFileName))
            {
                settingDal.WriteSettings(setting);
            }
            else
            {
                MainWindow.AyarlardaHata();
            }
        }
    }
}