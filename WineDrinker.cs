using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtLists
{
    public class WineDrinker
    {
        private CellarTrackerWeb m_ctWeb;

        public WineDrinker(CellarTrackerWeb ctWeb)
        {
            m_ctWeb = ctWeb;
        }

        public async Task FindAndDrinkWines(Cellar cellar, CtSql ctsql)
        {
            Dictionary<string, Bottle> bottles = await ctsql.GetBottlesToDrink();

            MessageBox.Show($"Bottles we drank: {bottles.Count}");

            // now, how many wines are still in the cellar? (these are un-drunk on CT)
            int count = 0;
            foreach (Bottle bottle in bottles.Values)
            {
                if (cellar.Contains(bottle.Barcode))
                    count++;
            }

            MessageBox.Show($"There are {count} bottles to drink on CellarTracker");

            // m_ctWeb.Show();

            m_ctWeb.EnsureLoggedIn();
            m_ctWeb.Show();
            foreach (Bottle bottle in bottles.Values)
            {
                if (cellar.Contains(bottle.Barcode))
                {
                    DateTime dttm;

                    if (!DateTime.TryParse(
                        bottle.GetValueOrEmpty("Consumed"),
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                        out dttm))
                    {
                        dttm = DateTime.UtcNow;
                    }

                    m_ctWeb.DrinkWine(bottle.Barcode, bottle.GetValueOrEmpty("Notes"), dttm);

                }
            }
            // m_ctWeb.DrinkWine("0104574263", "was Sweeter than Rob likes. Tonya eleanor liked.", DateTime.Parse("2020-11-15 21:55:38.000", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal));
            // m_ctWeb.DrinkWine("0116773813", "Good (was Good)", DateTime.Parse("2020-11-15 21:55:45.000", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal));

        }
    }
}