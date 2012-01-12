using OnThisDayApp.Resources;

namespace OnThisDayApp
{
    /// <summary>
    /// Localized resource provider
    /// </summary>
    public sealed class StringProvider
    {
        private readonly Strings resources = new Strings();

        public Strings Resources
        {
            get
            {
                return resources;
            }
        }
    }
}
