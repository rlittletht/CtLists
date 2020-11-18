using System;
using System.Collections.Generic;
using System.Security.Cryptography.Pkcs;
using System.Windows.Forms;
using HtmlAgilityPack;
using NUnit.Framework;
using NUnit.Framework.Internal;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace CtLists
{
    public class Bottle
    {
        public Bottle(Bottle bottle)
        {
            m_bottleValues = new Dictionary<string, string>(bottle.m_bottleValues);
        }

        public static int CompareBottle(Bottle left, Bottle right, bool fVarietal)
        {
            int n = 0;

            if (fVarietal)
            {
                n = String.Compare(left.Varietal, right.Varietal, StringComparison.OrdinalIgnoreCase);
                if (n != 0)
                    return n;
            }
            else
            {
                n = String.Compare(left.Color, right.Color, StringComparison.OrdinalIgnoreCase);
                if (n != 0)
                    return n;
            }

            // first, compare the country
            n = String.Compare(left.Country, right.Country, StringComparison.OrdinalIgnoreCase);
            if (n != 0)
                return n;

            n = String.Compare(left.SubRegion, right.SubRegion, StringComparison.OrdinalIgnoreCase);
            if (n != 0)
                return n;

            n = String.Compare(left.Appellation, right.Appellation, StringComparison.OrdinalIgnoreCase);
            if (n != 0)
                return n;

            if (!Int32.TryParse(left.Vintage, out int leftVintage))
                leftVintage = 0;

            if (!Int32.TryParse(right.Vintage, out int rightVintage))
                rightVintage = 0;

            if (leftVintage != rightVintage)
                return leftVintage - rightVintage;

            n = String.Compare(left.Wine, right.Wine, StringComparison.OrdinalIgnoreCase);
            if (n != 0)
                return n;

            return String.Compare(left.Location, right.Location, StringComparison.OrdinalIgnoreCase);
        }

        public static int SortByVarietal(Bottle left, Bottle right)
        {
            return CompareBottle(left, right, true /*fVarietal*/);
        }

        public static int SortByColor(Bottle left, Bottle right)
        {
            return CompareBottle(left, right, false /*fVarietal*/);
        }


        private Dictionary<string, string> m_bottleValues = new Dictionary<string, string>();

        private int m_countBottles = 1;

        private string[] m_valueKeys =
        {
            "iWine",
            "Barcode",
            "Location",
            "Bin",
            "Size",
            "Currency",
            "ExchangeRate",
            "Valuation",
            "Price",
            "NativePrice",
            "NativePriceCurrency",
            "StoreName",
            "PurchaseDate",
            "Note",
            "Vintage",
            "Wine",
            "Locale",
            "Country",
            "Region",
            "SubRegion",
            "Appellation",
            "Producer",
            "SortProducer",
            "Type",
            "Color",
            "Category",
            "Varietal",
            "MasterVarietal",
            "Designation",
            "Vineyard",
            "CT",
            "CNotes",
            "MY",
            "PNotes",
            "Begin",
            "End"
        };

        public void AddBottle()
        {
            m_countBottles++;
        }

        public int Count { get; set; }

        public bool HasValue(string sKey)
        {
            if (!m_bottleValues.ContainsKey(sKey))
                return false;

            if (string.IsNullOrEmpty(m_bottleValues[sKey]) || m_bottleValues[sKey] == "Unknown")
                return false;

            return true;
        }

        public string GetValueOrEmpty(string sValue)
        {
            if (HasValue(sValue))
                return m_bottleValues[sValue];

            return "";
        }
        public string Barcode => GetValueOrUnk("Barcode");
        public string Appellation => GetValueOrUnk("Appellation");
        public string SubRegion => GetValueOrUnk("SubRegion");
        public string Country => GetValueOrUnk("Country");
        public string Vintage => GetValueOrUnk("Vintage");
        public string Color => GetValueOrUnk("Color");
        public string Location => GetValueOrUnk("Location");
        public string Varietal => GetValueOrUnk("Varietal");

        public string Wine
        {
            get
            {
                if (HasValue("Vintage"))
                    return String.Format("{0} {1}", GetValue("Vintage"), GetValue("Wine"));
                else
                {
                    return GetValue("Wine");
                }
            }
        }

        public string SubRegionAppellation
        {
            get
            {
                if (HasValue("SubRegion"))
                {
                    if (HasValue("Appellation"))
                        return String.Format("{0} {1}", GetValue("SubRegion"), GetValue("Appellation"));

                    return GetValue("SubRegion");
                }

                if (HasValue("Appellation"))
                    return GetValue("Appellation");

                return "";
            }
        }

        public Bottle()
        {
        }
        
        public Bottle(BottleBuilder builder, HtmlNode row)
        {
            foreach (string s in m_valueKeys)
                m_bottleValues.Add(s, builder.GetStringFromRow(s, row));
        }

        public void SetValue(string sKey, string sValue)
        {
            m_bottleValues.Add(sKey, sValue);
        }

        public string GetValueOrUnk(string sKey)
        {
            if (HasValue(sKey))
                return m_bottleValues[sKey];

            return "Unk";
        }


        public string GetValue(string sKey)
        {
            return m_bottleValues[sKey];
        }
        #region Tests

        [Test]
        public static void TestVintageCompareLess()
        {
            Bottle bottleLeft = new Bottle();
            bottleLeft.m_bottleValues.Add("Vintage", "1999");
            bottleLeft.m_bottleValues.Add("Wine", "Wine 1");
            Bottle bottleRight = new Bottle();
            bottleRight.m_bottleValues.Add("Vintage", "200");
            bottleRight.m_bottleValues.Add("Wine", "Wine 1");

            List<Bottle> list = new List<Bottle>( new Bottle[] {bottleRight, bottleLeft});

            list.Sort();

            Assert.AreEqual("200", list[0].Vintage);
            Assert.AreEqual("1999", list[1].Vintage);
        }

        [Test]
        public static void TestVintageCompareGreater()
        {
            Bottle bottleLeft = new Bottle();
            bottleLeft.m_bottleValues.Add("Vintage", "1999");
            bottleLeft.m_bottleValues.Add("Wine", "Wine 1");
            Bottle bottleRight = new Bottle();
            bottleRight.m_bottleValues.Add("Vintage", "200");
            bottleRight.m_bottleValues.Add("Wine", "Wine 1");

            List<Bottle> list = new List<Bottle>(new Bottle[] { bottleLeft, bottleRight });

            list.Sort();

            Assert.AreEqual("200", list[0].Vintage);
            Assert.AreEqual("1999", list[1].Vintage);
        }

        [Test]
        public static void TestAppellationCompare()
        {
            Bottle bottle1 = new Bottle();
            bottle1.m_bottleValues.Add("Vintage", "1999");
            bottle1.m_bottleValues.Add("Wine", "Wine 1");
            bottle1.m_bottleValues.Add("Country", "Country 1");
            bottle1.m_bottleValues.Add("SubRegion", "SR 1");
            bottle1.m_bottleValues.Add("Appellation", "App 1");

            Bottle bottle2 = new Bottle();
            bottle2.m_bottleValues.Add("Vintage", "1999");
            bottle2.m_bottleValues.Add("Wine", "Wine 2");
            bottle2.m_bottleValues.Add("Country", "Country 1");
            bottle2.m_bottleValues.Add("SubRegion", "SR 1");
            bottle2.m_bottleValues.Add("Appellation", "App 2");

            Bottle bottle3 = new Bottle();
            bottle3.m_bottleValues.Add("Vintage", "1999");
            bottle3.m_bottleValues.Add("Wine", "Wine 3");
            bottle3.m_bottleValues.Add("Country", "Country 1");
            bottle3.m_bottleValues.Add("SubRegion", "SR 1");
            bottle3.m_bottleValues.Add("Appellation", "App 1");

            List<Bottle> list = new List<Bottle>(new Bottle[] { bottle1, bottle2, bottle3 });

            list.Sort();

            Assert.AreEqual("1999 Wine 1", list[0].Wine);
            Assert.AreEqual("1999 Wine 3", list[1].Wine);
            Assert.AreEqual("1999 Wine 2", list[2].Wine);
        }
        #endregion

    }

    public class BottleBuilder
    {
        private Dictionary<string, int> m_headerMapping = new Dictionary<string, int>();

        public void SetColumns(HtmlDocument doc)
        {
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//th");
            int i = 0;

            foreach (HtmlNode node in nodes)
            {
                m_headerMapping.Add(node.InnerText, i++);
            }
        }

        public string GetStringFromRow(string sKey, HtmlNode row)
        {
            string s = row.ChildNodes[m_headerMapping[sKey]].InnerText;

            if (string.IsNullOrWhiteSpace(s) || s == "&nbsp;")
                return "";
            return s;
        }

        public Bottle BuildBottleFromRow(HtmlNode row)
        {
            return new Bottle(this, row);
        }
    }
}