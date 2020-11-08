using System.Collections.Generic;
using HtmlAgilityPack;

namespace CtLists
{
    public class Cellar
    {
        private List<Bottle> m_bottles = new List<Bottle>();

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

                cellar.m_bottles.Add(builder.BuildBottleFromRow(node));
            }
            return cellar;
        }
    }
}