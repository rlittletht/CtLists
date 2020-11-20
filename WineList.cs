using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace CtLists
{
    public class WineList
    {
        private List<Bottle> m_bottles = new List<Bottle>();
        public string ListName { get; set; }

        public List<Bottle> Bottles => m_bottles;
        public int BottleCount { get; set; }

        public static WineList BuildFromCellar(Cellar cellar, string[] rgsLocations, string[] rgsColor, bool fGroupByVarietal)
        {
            WineList list = new WineList();
            Dictionary<string, int> bottlesSeen = new Dictionary<string, int>();
            StringBuilder sbListName = new StringBuilder();

            foreach (string sLocation in rgsLocations)
                sbListName.Append($"_{sLocation}");

            foreach (string sColor in rgsColor)
                sbListName.Append($"_{sColor}");

            if (fGroupByVarietal)
                sbListName.Append("_Varietal");

            list.ListName = sbListName.ToString();
            list.BottleCount = 0;

            // collect the number of bottles seen
            foreach (Bottle bottle in cellar.Bottles)
            {
                if (bottle.Bin == "BINLESS")
                    continue;

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

                list.BottleCount++;
                if (bottlesSeen.ContainsKey(bottle.Wine))
                    bottlesSeen[bottle.Wine]++;
                else
                    bottlesSeen.Add(bottle.Wine, 1);
            }

            foreach (Bottle bottle in cellar.Bottles)
            {
                if (bottle.Bin == "BINLESS")
                    continue;

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

        string SBinDescriptorFromBin(string sBin)
        {
            // bin is 4 digit column, 4 digit row
            if (sBin.Length != 8)
                return "";

            string sCol = sBin.Substring(0, 4);
            string sRow = sBin.Substring(4, 4);

            sRow = sRow.TrimStart(new char[] {'0'});
            int iCol = int.Parse(sCol);

            string sColChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(iCol - 1, 1);

            return $" ({sColChar}{sRow})";
        }

        public void CreateFile(string sOutputFile, bool fGroupByVarietal, bool fSingleColor)
        {
            if (Bottles.Count == 0)
            {
                MessageBox.Show("Wine list is empty");
                return;
            }

            sOutputFile = $"{sOutputFile}{ListName}.html";

            using (TextWriter tw = new StreamWriter(sOutputFile))
            {
                tw.WriteLine("<HTML xmlns:w='urn:schemas-microsoft-com:office:word'><meta http-equiv='Content-Type' content='text/html; charset=utf-8'>");
                tw.WriteLine("<BODY>");
                string sColorCurrent = "";
                string sCountryCurrent = "";
                string sSubRegionAppellationCurrent = "";

                if (!fGroupByVarietal)
                {
                    if (fSingleColor || Bottles.Count == 1)
                        sColorCurrent = Bottles[0].Color;
                }

                foreach (Bottle bottle in Bottles)
                {
                    if (fGroupByVarietal)
                    {
                        if (String.Compare(sColorCurrent, bottle.Varietal, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            sColorCurrent = bottle.Varietal;
                            tw.WriteLine($"<h1>{sColorCurrent}</h1>");
                            sCountryCurrent = "";
                        }
                    }
                    else
                    {
                        if (String.Compare(sColorCurrent, bottle.Color, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            sColorCurrent = bottle.Color;
                            tw.WriteLine($"<h1>{sColorCurrent}</h1>");
                            sCountryCurrent = "";
                        }
                    }

                    if (String.Compare(sCountryCurrent, bottle.Country, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        sCountryCurrent = bottle.Country;
                        sSubRegionAppellationCurrent = "";
                        tw.WriteLine($"<h2>{sCountryCurrent}</h2>");
                    }

                    if (String.Compare(sSubRegionAppellationCurrent, bottle.SubRegionAppellation, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        sSubRegionAppellationCurrent = bottle.SubRegionAppellation;
                        tw.WriteLine($"<h3>{sSubRegionAppellationCurrent}</h3>");
                    }

                    StringBuilder sbInfo = new StringBuilder();

                    if (bottle.HasValue("CT"))
                        sbInfo.AppendFormat("CT{0}", bottle.GetValue("CT"));

                    if (bottle.HasValue("Begin"))
                    {
                        if (sbInfo.Length > 0)
                            sbInfo.Append(",");
                        sbInfo.AppendFormat("{0}-", bottle.GetValue("Begin"));
                    }

                    if (bottle.HasValue("End"))
                    {
                        if (!bottle.HasValue("Begin"))
                            sbInfo.Append("-");

                        sbInfo.Append(bottle.GetValue("End"));
                    }

                    if (bottle.Count != 1)
                    {
                        if (sbInfo.Length == 0)
                            sbInfo.AppendFormat("({0})", bottle.Count); // could add "bottles" suffix here...
                        else
                            sbInfo.AppendFormat("({0})", bottle.Count);
                    }

                    sbInfo.Append(SBinDescriptorFromBin(bottle.Bin));

                    tw.Write("<p class=Wine>");
                    int iBreakPoint = 105 - sbInfo.Length;
                    if (bottle.Wine.Length > iBreakPoint)
                    {
                        // split into two lines
                        int iSplit = bottle.Wine.LastIndexOf(' ', iBreakPoint);
                        tw.Write(bottle.Wine.Substring(0, iSplit));
                        tw.Write("</p>");
                        tw.Write("<p class=Wine>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                        tw.Write(bottle.Wine.Substring(iSplit + 1));
                    }
                    else
                    {
                        tw.Write(bottle.Wine);
                    }

                    tw.Write("<w:PTab Alignment=\"RIGHT\" RelativeTo=\"MARGIN\" Leader=\"NONE\"/>");
                    tw.Write(sbInfo.ToString());
                    tw.WriteLine("</p>");
                }

                tw.WriteLine("</BODY></HTML>");
                tw.Close();
                MessageBox.Show($"Created WineList {sOutputFile} for {BottleCount} bottles");
            }

        }
    }
}