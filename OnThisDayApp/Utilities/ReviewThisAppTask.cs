﻿using System.IO.IsolatedStorage;
using System.Windows;
using OnThisDayApp.Resources;
using Microsoft.Phone.Tasks;

namespace Utilities
{
    public sealed class ReviewThisAppTask
    {
        private const string numberOfStarts = "NumberOfStarts";
        private const string firstStart = "FirstStart";
        private const int numberOfStartsThreshold = 5;
        private static readonly IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        public static int NumberOfStarts
        {
            get
            {
                if (settings.Contains(numberOfStarts))
                {
                    return int.Parse(settings[numberOfStarts].ToString());
                }

                return 0;
            }
            set
            {
                settings[numberOfStarts] = NumberOfStarts + 1;
            }
        }

        // TODO: remove in the next update
        public static bool FirstStart
        {
            get
            {
                return !settings.Contains(firstStart);
            }
            set
            {
                settings[firstStart] = true;
            }
        }

        public void ShowAfterThreshold()
        {
            if (NumberOfStarts == numberOfStartsThreshold &&
                GetMessageBoxResult() == MessageBoxResult.OK)
            {
                MarketplaceReviewTask task = new MarketplaceReviewTask();
                task.Show();
            }
        }

        private MessageBoxResult GetMessageBoxResult()
        {
            return MessageBox.Show(
                Strings.MessageBoxRateThisAppSummary,
                Strings.MessageBoxRateThisAppTitle,
                MessageBoxButton.OKCancel);
        }
    }
}
