using System;
using System.Globalization;

namespace CtLists
{
    public class WineDrinker
    {
        private CellarTrackerWeb m_ctWeb;

        public WineDrinker(CellarTrackerWeb ctWeb)
        {
            m_ctWeb = ctWeb;
        }

        public void FindAndDrinkWines()
        {
            m_ctWeb.Show();

            // m_ctWeb.EnsureLoggedIn();
            m_ctWeb.DrinkWine("0104574263", "was Sweeter than Rob likes. Tonya eleanor liked.", DateTime.Parse("2020-11-15 21:55:38.000", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal));
            m_ctWeb.DrinkWine("0116773813", "Good (was Good)", DateTime.Parse("2020-11-15 21:55:45.000", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal));

        }
    }
}