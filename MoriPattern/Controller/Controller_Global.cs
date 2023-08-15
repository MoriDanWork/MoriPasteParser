using System;
using System.Collections.Generic;
using System.Configuration;

namespace MoriPattern.Controller
{
    public static class Global
    {
        private static readonly Random _random = new();

        public static List<T> Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
            return list;
        }

        public static T ReadSetting<T>(string key, T defaultValue = default)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string value = appSettings[key];
                if (!string.IsNullOrEmpty(value))
                    return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
            catch (FormatException) { }
            return defaultValue;
        }

        public static void SetSetting(string key, object value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                if (value != null)
                {
                    if (settings[key] == null)
                        settings.Add(key, value.ToString());
                    else
                        settings[key].Value = value.ToString();
                }
                else
                {
                    settings.Add(key, "");
                }

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }

}
