using System.Collections.Generic;
using HtmlAgilityPack;
using Microsoft.Identity.Client;
using NUnit.Framework.Constraints;

namespace CtLists
{
    public class Cellar
    {
        private Dictionary<string, Bottle> m_bottles = new Dictionary<string, Bottle>();

        public IEnumerable<Bottle> Bottles => m_bottles.Values;

        public Bottle this[string sScanCode] => m_bottles[sScanCode];

        public static Cellar BuildFromDocument(HtmlDocument doc)
        {
            BottleBuilder builder = new BottleBuilder();

            builder.SetColumns(doc);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//tr");
            Cellar cellar = new Cellar();

            foreach (HtmlNode node in nodes)
            {
                if (node.ChildNodes[0].Name == "th")
                    continue;

                Bottle bottle = builder.BuildBottleFromRow(node);

                cellar.m_bottles.Add(bottle.Barcode, bottle);
            }
            return cellar;
        }
    }
}