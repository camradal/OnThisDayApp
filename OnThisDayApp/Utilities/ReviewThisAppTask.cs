using System.Windows;
using Microsoft.Phone.Tasks;
using OnThisDayApp.Resources;

namespace Utilities
{
    public sealed class ReviewThisAppTask
    {
        private const int numberOfStartsThreshold = 5;
        private const int numberOfStartsModulo = 50;

        public void ShowAfterThreshold()
        {
            int starts = AppSettings.NumberOfStarts;
            if ((starts == numberOfStartsThreshold || starts % numberOfStartsModulo == 0) &&
                GetMessageBoxResult() == MessageBoxResult.OK)
            {
                try
                {
                    var task = new MarketplaceReviewTask();
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
                Strings.MessageBoxRateThisAppSummary,
                Strings.MessageBoxRateThisAppTitle,
                MessageBoxButton.OKCancel);
        }
    }
}
