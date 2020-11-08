using System.Collections.Generic;
using System.Net;

namespace CtLists
{
    public class WineList
    {
        private List<Bottle> m_bottles = new List<Bottle>();

        static WineList BuildFromCellar(Cellar cellar, string[] rgsLocations, string[] rgsColor)
        {
            WineList list = new WineList();

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

                list.m_bottles.Add(bottle);
            }

            return list;
        }
    }
}