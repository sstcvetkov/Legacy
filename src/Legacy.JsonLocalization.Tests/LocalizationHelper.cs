using System.Globalization;

namespace Legacy.JsonLocalization.Tests
{
	public static class LocalizationHelper
    {
        public static void SetCurrentCulture(string culture)
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);
        }
    }
}