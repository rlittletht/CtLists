using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtLists
{
    public class WineMover
    {
        private CellarTrackerWeb m_ctWeb;

        public WineMover(CellarTrackerWeb ctWeb)
        {
            m_ctWeb = ctWeb;
        }

        public async Task<int> FindAndRelocateWines(Cellar cellar, CtSql ctsql, bool fPreflightOnly)
        {
            // NOTE: this list INCLUDES bottles that are consumed. be careful.
            Dictionary<string, Bottle> bottles = await ctsql.GetBottlesToRelocate();

            if (!fPreflightOnly)
                MessageBox.Show($"Bottles we have binned: {bottles.Count}");

            // how many of these don't match cellartracker?
            Dictionary<string, Bottle> bottlesToUpdateOnCT = new Dictionary<string, Bottle>();

            // find bottles in our list that don't match CT
            foreach (Bottle bottle in bottles.Values)
            {
                if (bottle.IsConsumed)
                    continue;

                if (cellar.Contains(bottle.Barcode))
                {
                    if (cellar[bottle.Barcode].Bin != bottle.Bin)
                        bottlesToUpdateOnCT.Add(bottle.Barcode, bottle);
                }
                else
                {
                    MessageBox.Show(
                        $"Found a bottle in our inventory that's not on CT: {bottle.Barcode}: {bottle.Wine}. Need to update CT?");
                }
            }

            // now go through all the bottles on CT that have a Bin and make sure its in our inventory
            foreach (Bottle bottle in cellar.Bottles)
            {
                if (string.IsNullOrEmpty(bottle.Bin) || bottle.Bin == "BINLESS")
                    continue;

                if (bottles.ContainsKey(bottle.Barcode))
                {
                    if (bottles[bottle.Barcode].Bin != bottle.Bin)
                    {
                        if (!bottlesToUpdateOnCT.ContainsKey(bottle.Barcode))
                        {
                            bottlesToUpdateOnCT.Add(bottle.Barcode, bottle);
                        }
                        else
                        {
                            if (bottlesToUpdateOnCT[bottle.Barcode].Bin != bottles[bottle.Barcode].Bin)
                                throw new Exception("broken symmetry");
                        }
                    }
                }
                else
                {
                    MessageBox.Show(
                        $"Found a bottle on cellar tracker that has a bin, but it isn't in our inventory anymore ({bottle.Barcode}: {bottle.Wine} {bottle.Bin}). Need to remove from inventory?");

                    // at the very least, should we unbin it (relocate it to nowhere?)
                }
            }

            if (!fPreflightOnly)
            {
                MessageBox.Show($"There are {bottlesToUpdateOnCT.Count} bottles to relocate on CellarTracker");

                m_ctWeb.EnsureLoggedIn();
                // m_ctWeb.Show();

                foreach (Bottle bottle in bottlesToUpdateOnCT.Values)
                    m_ctWeb.RelocateWine(bottle.Barcode, bottle.Bin);
            }

            return bottlesToUpdateOnCT.Count;
        }
    }
}