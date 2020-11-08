﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HtmlAgilityPack;
using NUnit.Framework;
using NUnit.Framework.Internal;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace CtLists
{
    public class Bottle : IComparable<Bottle>
    {
        public int CompareTo(Bottle other)
        {
            int n = 0;

            // first, compare the country
            n = String.Compare(this.Country, other.Country, StringComparison.OrdinalIgnoreCase);
            if (n != 0)
                return n;

            n = String.Compare(this.SubRegion, other.SubRegion, StringComparison.OrdinalIgnoreCase);
            if (n != 0)
                return n;

            n = String.Compare(this.Appelation, other.Appelation, StringComparison.OrdinalIgnoreCase);
            if (n != 0)
                return n;

            if (!Int32.TryParse(this.Vintage, out int leftVintage))
                leftVintage = 0;

            if (!Int32.TryParse(other.Vintage, out int rightVintage))
                rightVintage = 0;

            if (leftVintage != rightVintage)
                return leftVintage - rightVintage;

            n = String.Compare(this.Wine, other.Wine, StringComparison.OrdinalIgnoreCase);
            if (n != 0)
                return n;

            return String.Compare(this.Location, other.Location, StringComparison.OrdinalIgnoreCase);
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

        public int Count => m_countBottles;

        public bool HasValue(string sKey)
        {
            if (!m_bottleValues.ContainsKey(sKey))
                return false;

            if (string.IsNullOrEmpty(m_bottleValues[sKey]))
                return false;

            return true;
        }

        public string Appelation => GetValueOrUnk("Appelation");
        public string SubRegion => GetValueOrUnk("SubRegion");
        public string Country => GetValueOrUnk("Country");
        public string Vintage => GetValueOrUnk("Vintage");
        public string Color => GetValueOrUnk("Color");
        public string Location => GetValueOrUnk("Location");

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

        public Bottle() // only here for unit tests
        {

        }
        
        public Bottle(BottleBuilder builder, HtmlNode row)
        {
            foreach (string s in m_valueKeys)
                m_bottleValues.Add(s, builder.GetStringFromRow(s, row));
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

            Assert.AreEqual(list[0].Vintage, "200");
            Assert.AreEqual(list[1].Vintage, "1999");
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

            Assert.AreEqual(list[0].Vintage, "200");
            Assert.AreEqual(list[1].Vintage, "1999");
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
            return row.ChildNodes[m_headerMapping[sKey]].InnerText;
        }

        public Bottle BuildBottleFromRow(HtmlNode row)
        {
            return new Bottle(this, row);
        }
    }
}