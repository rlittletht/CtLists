using System.Collections.Generic;
using HtmlAgilityPack;

namespace CtLists
{
    public class Cellar
    {
        private List<Bottle> m_bottles = new List<Bottle>();

        public List<Bottle> Bottles => m_bottles;

        public static Cellar BuildFromDocument(HtmlDocument doc)
        {
            BottleBuilder builder = new BottleBuilder();

            builder.SetColumns(doc);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//tr");
            Cellar cellar = new Cellar();

            Dictionary<string, Bottle> bottlesSeen = new Dictionary<string, Bottle>();

            foreach (HtmlNode node in nodes)
            {
                if (node.ChildNodes[0].Name == "th")
                    continue;

                Bottle bottle = builder.BuildBottleFromRow(node);

                if (bottlesSeen.ContainsKey(bottle.Wine))
                {
                    bottlesSeen[bottle.Wine].AddBottle();
                }
                else
                {
                    cellar.m_bottles.Add(bottle);
                    bottlesSeen.Add(bottle.Wine, bottle);;
                }
            }
            return cellar;
        }
    }
}