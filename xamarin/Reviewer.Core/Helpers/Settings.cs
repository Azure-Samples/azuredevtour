using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace PhotoTour.Core
{
    public class Settings
    {
        static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        const string SettingsKey = "settings_key";
        static readonly string SettingsDefault = string.Empty;

        const string BlockedUsersKey = "blocked_users_key";
        const string ReportedImagesKey = "reported_images_key";

        #endregion

        public static string GeneralSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(SettingsKey, value);
            }
        }

        public static List<string> BlockedUsers
        {
            get
            {
                // will be csv
                var rawValue = AppSettings.GetValueOrDefault(BlockedUsersKey, string.Empty);

                if (string.IsNullOrWhiteSpace(rawValue))
                    return new List<string>();
                else
                    return new List<string>(rawValue.Split(new char[] { ',' }));
            }

            set
            {
                // serialize into csv
                string serialized = "";
                foreach (var item in value)
                {
                    serialized += item + ",";
                }

                serialized = serialized.Remove(serialized.Length - 1, 1);

                AppSettings.AddOrUpdateValue(BlockedUsersKey, serialized);
            }
        }
    }
}
