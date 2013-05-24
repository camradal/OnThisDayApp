using System;
using Microsoft.Phone.Tasks;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp
{
    static internal class ShareHelper
    {
        internal static void ShareViaEmail(ShareModel model)
        {
            try
            {
                var task = new EmailComposeTask
                {
                    Subject = model.Title,
                    Body = model.Description+ "\n\n" + @"http://en.wikipedia.org" + model.Link
                };
                task.Show();
            }
            catch (Exception)
            {
                // fast-clicking can result in exception, so we just handle it
            }
        }

        internal static void ShareViaSocial(ShareModel model)
        {
            try
            {
                var task = new ShareLinkTask
                {
                    Title = model.Title,
                    Message = model.Description,
                    LinkUri = new Uri(@"http://en.wikipedia.org" + model.Link, UriKind.Absolute)
                };
                task.Show();
            }
            catch (Exception)
            {
                // fast-clicking can result in exception, so we just handle it
            }
        }

        internal static void ShareViaSms(ShareModel model)
        {
            try
            {
                var task = new SmsComposeTask()
                {
                    Body = model.Description + "\n" + @"http://en.wikipedia.org" + model.Link
                };
                task.Show();
            }
            catch (Exception)
            {
                // fast-clicking can result in exception, so we just handle it
            }
        }
    }
}
