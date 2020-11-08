using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace CtLists
{
    public class Bottle
    {
        private Dictionary<string, string> m_bottleValues = new Dictionary<string, string>();

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

        public bool HasValue(string sKey)
        {
            if (!m_bottleValues.ContainsKey(sKey))
                return false;

            if (string.IsNullOrEmpty(m_bottleValues[sKey]))
                return false;

            return true;
        }

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

        public string Color
        {
            get
            {
                if (HasValue("Color"))
                    return GetValue("Color");

                return "Unk";
            }
        }

        public string Location
        {
            get
            {
                if (HasValue("Location"))
                    return GetValue("Location");

                return "Unk";
            }

        }
        public Bottle(BottleBuilder builder, HtmlNode row)
        {
            foreach (string s in m_valueKeys)
                m_bottleValues.Add(s, builder.GetStringFromRow(s, row));
        }

        public string GetValue(string sKey)
        {
            return m_bottleValues[sKey];
        }
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