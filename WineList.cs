using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CtLists
{
    public class WineList
    {
        private List<Bottle> m_bottles = new List<Bottle>();

        public List<Bottle> Bottles => m_bottles;

        public static WineList BuildFromCellar(Cellar cellar, string[] rgsLocations, string[] rgsColor, bool fGroupByVarietal)
        {
            WineList list = new WineList();
            Dictionary<string, int> bottlesSeen = new Dictionary<string, int>();

            // collect the number of bottles seen
            foreach (Bottle bottle in cellar.Bottles)
            {
                if (rgsLocations != null)
                {
                    bool fMatchLocation = false;

                    foreach (string sLocation in rgsLocations)
                    {
                        if (string.Compare(bottle.Location, sLocation, true) == 0)
                        {
                            fMatchLocation = true;
                            break;
                        }
                    }

                    if (!fMatchLocation)
                        continue;
                }

                if (bottlesSeen.ContainsKey(bottle.Wine))
                    bottlesSeen[bottle.Wine]++;
                else
                    bottlesSeen.Add(bottle.Wine, 1);
            }

            foreach (Bottle bottle in cellar.Bottles)
            {
                if (rgsLocations != null)
                {
                    bool fMatchLocation = false;

                    foreach (string sLocation in rgsLocations)
                    {
                        if (string.Compare(bottle.Location, sLocation, true) == 0)
                        {
                            fMatchLocation = true;
                            break;
                        }
                    }

                    if (!fMatchLocation)
                        continue;
                }

                if (rgsColor != null)
                {
                    bool fMatchColor = false;

                    foreach (string sColor in rgsColor)
                    {
                        if (string.Compare(bottle.Color, sColor, true) == 0)
                        {
                            fMatchColor = true;
                            break;
                        }
                    }

                    if (!fMatchColor)
                        continue;
                }

                if (bottlesSeen[bottle.Wine] == 0)
                    continue; // we already added this bottle to the list

                Bottle bottleNew = new Bottle(bottle) {Count = bottlesSeen[bottle.Wine]};

                bottlesSeen[bottle.Wine] = 0;   // we've added it to the list, so don't add again...

                list.m_bottles.Add(bottleNew);
            }

            if (fGroupByVarietal)
                list.m_bottles.Sort(Bottle.SortByVarietal);
            else
                list.m_bottles.Sort(Bottle.SortByColor);

            return list;
        }
    }
}