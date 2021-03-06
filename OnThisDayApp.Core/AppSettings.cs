﻿using System;
using System.IO.IsolatedStorage;

namespace Utilities
{
    /// <summary>
    /// Class for handling application settings
    /// </summary>
    public sealed class AppSettings
    {
        #region Variables

        private static readonly IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        private static readonly object locker = new object();

        private const string DataStoreUpdate21KeyName = "DataStoreUpdate21";
        private const string NumberOfStartsKeyName = "NumberOfStarts";
        private const string FirstStartKeyName = "FirstStart";
        private const string InterfaceLanguageKeyName = "InterfaceLanguage";
        private const string ContentLanguageKeyName = "ContentLanguage";
        private const string LiveTileDisabledKeyName = "LiveTileDisabled";
        private const string ShowNewestItemsFirstKeyName = "ShowNewestItemsFirst";
        private const string DisplayFontSizeKeyName = "DisplayFontSize";
        private const string BrowserSelectionKeyName = "BrowserSelection";
        private const string OrientationLockKeyName = "OrientationLock";

        private const bool DataStoreUpdate21Default = false;
        private const int NumberOfStartsDefault = 0;
        private const bool FirstStartDefault = false;
        private const string InterfaceLanguageDefault = "en";
        private const string ContentLanguageDefault = "en";
        private const bool LiveTileDisabledDefault = false;
        private const bool ShowNewestItemsFirstDefault = false;
        private const int DisplayFontSizeDefault = 0;
        private const bool BrowserSelectionDefault = false;
        private const bool OrientationLockDefault = false;

        #endregion

        #region Properties

        public static bool DataStoreUpdate21
        {
            get { return GetValueOrDefault<bool>(DataStoreUpdate21KeyName, DataStoreUpdate21Default); }
            set { AddOrUpdateValue(DataStoreUpdate21KeyName, value); }
        }

        public static int NumberOfStarts
        {
            get { return GetValueOrDefault<int>(NumberOfStartsKeyName, NumberOfStartsDefault); }
            set { AddOrUpdateValue(NumberOfStartsKeyName, value); }
        }

        public static bool FirstStartSetting
        {
            get { return GetValueOrDefault<bool>(FirstStartKeyName, FirstStartDefault); }
            set { AddOrUpdateValue(FirstStartKeyName, value); }
        }

        public static string InterfaceLanguage
        {
            get { return GetValueOrDefault<string>(InterfaceLanguageKeyName, InterfaceLanguageDefault); }
            set { AddOrUpdateValue(InterfaceLanguageKeyName, value); }
        }

        public static string ContentLanguageSetting
        {
            get { return GetValueOrDefault<string>(ContentLanguageKeyName, ContentLanguageDefault); }
            set { AddOrUpdateValue(ContentLanguageKeyName, value); }
        }

        public static bool IsLowMemDevice
        {
            get { return LowMemoryHelper.IsLowMemDevice; }
        }

        public static bool LiveTileEnabled
        {
            get { return !IsLowMemDevice && !LiveTileDisabled; }
            set { LiveTileDisabled = !value; }
        }

        public static bool LiveTileDisabled
        {
            get { return GetValueOrDefault<bool>(LiveTileDisabledKeyName, LiveTileDisabledDefault); }
            set { AddOrUpdateValue(LiveTileDisabledKeyName, value); }
        }

        public static bool ShowNewestItemsFirst
        {
            get { return GetValueOrDefault<bool>(ShowNewestItemsFirstKeyName, ShowNewestItemsFirstDefault); }
            set { AddOrUpdateValue(ShowNewestItemsFirstKeyName, value); }
        }

        public static int DisplayFontSize
        {
            get { return GetValueOrDefault<int>(DisplayFontSizeKeyName, DisplayFontSizeDefault); }
            set { AddOrUpdateValue(DisplayFontSizeKeyName, value); }
        }

        public static bool BrowserSelection
        {
            get { return GetValueOrDefault<bool>(BrowserSelectionKeyName, BrowserSelectionDefault); }
            set { AddOrUpdateValue(BrowserSelectionKeyName, value); }
        }

        public static bool OrientationLock
        {
            get { return GetValueOrDefault<bool>(OrientationLockKeyName, OrientationLockDefault); }
            set { AddOrUpdateValue(OrientationLockKeyName, value); }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Update a setting value. If the setting does not, then add the setting.
        /// </summary>
        private static bool AddOrUpdateValue(string key, Object value)
        {
            lock (locker)
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
                if (valueChanged)
                {
                    settings.Save();
                }
                return valueChanged;
            }
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        private static T GetValueOrDefault<T>(string key, T defaultValue)
        {
            lock (locker)
            {
                T value = settings.Contains(key) ? (T)settings[key] : defaultValue;
                return value;
            }
        }

        #endregion
    }
}