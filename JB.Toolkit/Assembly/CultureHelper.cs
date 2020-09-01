using System.Threading;

namespace JBToolkit.AssemblyHelper
{
    public class CultureHelper
    {
        /// <summary>
        /// Used for 3rd party solution template, where the culture is set to US and so messes up DateTimes (i.e. M-Files)
        /// </summary>
        public static void GloballySetCultureToGB()
        {
            var culture = new System.Globalization.CultureInfo("en-GB");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
