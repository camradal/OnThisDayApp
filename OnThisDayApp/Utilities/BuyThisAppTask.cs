using System.Windows;
using Microsoft.Phone.Tasks;
using OnThisDayApp.Resources;

namespace Utilities
{
    public sealed class BuyThisAppTask
    {
        private const int numberOfStartsThreshold = 9;

        public void ShowAfterThreshold()
        {
            int starts = AppSettings.NumberOfStarts;
            if ((starts == numberOfStartsThreshold) && GetMessageBoxResult() == MessageBoxResult.OK)
            {
                try
                {
                    var task = new MarketplaceDetailTask { ContentIdentifier = "60070dfd-ac08-4018-b6cf-9ccda9806158" };
                    task.Show();
                }
                catch
                {
                }
            }
        }

        private MessageBoxResult GetMessageBoxResult()
        {
            return MessageBox.Show(
                Strings.MessageBoxBuyThisAppSummary,
                Strings.MessageBoxBuyThisAppTitle,
                MessageBoxButton.OKCancel);
        }
    }
}