using System;
using System.IO.IsolatedStorage;

namespace SettingsSample
{
    /// <summary>
    /// Class for loading application settings
    /// </summary>
    public sealed class AppSettings
    {
        #region Variables

        private readonly IsolatedStorageSettings settings;

        private const string NumberOfStartsKeyName = "NumberOfStarts";
        private const string FirstStartKeyName = "FirstStart";
        private const string InterfaceLanguageKeyName = "InterfaceLanguage";
        private const string ContentLanguageKeyName = "ContentLanguage";

        private const int NumberOfStartsDefault = 0;
        private const bool FirstStartDefault = false;
        private const string InterfaceLanguageDefault = "en";
        private const string ContentLanguageDefault = "en";

        #endregion

        #region Properties

        public int NumberOfStarts
        {
            get
            {
                return GetValueOrDefault<int>(NumberOfStartsKeyName, NumberOfStartsDefault);
            }
            set
            {
                if (AddOrUpdateValue(NumberOfStartsKeyName, value))
                {
                    Save();
                }
            }
        }

        public bool FirstStartSetting
        {
            get
            {
                return GetValueOrDefault<bool>(FirstStartKeyName, FirstStartDefault);
            }
            set
            {
                if (AddOrUpdateValue(FirstStartKeyName, value))
                {
                    Save();
                }
            }
        }

        public string InterfaceLanguage
        {
            get
            {
                return GetValueOrDefault<string>(InterfaceLanguageKeyName, InterfaceLanguageDefault);
            }
            set
            {
                if (AddOrUpdateValue(InterfaceLanguageKeyName, value))
                {
                    Save();
                }
            }
        }

        public string ContentLanguageSetting
        {
            get
            {
                return GetValueOrDefault<string>(ContentLanguageKeyName, ContentLanguageDefault);
            }
            set
            {
                if (AddOrUpdateValue(ContentLanguageKeyName, value))
                {
                    Save();
                }
            }
        }

        #endregion

        #region Constructor

        public AppSettings()
        {
            // Get the settings for this application.
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Update a setting value. If the setting does not, then add the setting.
        /// </summary>
        private bool AddOrUpdateValue(string key, Object value)
        {
            bool valueChanged = false;

            if (settings.Contains(key))
            {
                if (settings[key] != value)
                {
                    settings[key] = value;
                    valueChanged = true;
                }
            }
            else
            {
                settings.Add(key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        private T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            else
            {
                value = defaultValue;
            }
            return value;
        }

        private void Save()
        {
            settings.Save();
        }

        #endregion
    }
}