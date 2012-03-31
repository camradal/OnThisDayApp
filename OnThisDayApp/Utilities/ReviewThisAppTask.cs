using System.IO.IsolatedStorage;
using System.Windows;
using OnThisDayApp.Resources;
using Microsoft.Phone.Tasks;

namespace Utilities
{
    public sealed class ReviewThisAppTask
    {
        private const int numberOfStartsThreshold = 5;

        public ReviewThisAppTask()
        {
            AppSettings.Instance.NumberOfStarts++;
        }

        public void ShowAfterThreshold()
        {
            if (AppSettings.Instance.NumberOfStarts == numberOfStartsThreshold &&
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
