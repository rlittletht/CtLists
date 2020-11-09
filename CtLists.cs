using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NUnit.Framework.Internal.Execution;

namespace CtLists
{
    public partial class CtLists : Form
    {
        private string m_username;
        private string m_password;
        private HttpClient m_client = new HttpClient();

        public CtLists(string username, string password)
        {
            m_username = username;
            m_password = password;

            InitializeComponent();
        }

        private Cellar m_cellar;

        async private void DoTestIt(object sender, EventArgs e)
        {
            string sRequest = $"https://www.cellartracker.com/xlquery.asp?table=Inventory&User={m_username}&Password={m_password}";
            String sHtml = null;

            try
            {
                HttpResponseMessage response = await m_client.GetAsync(sRequest);

                response.EnsureSuccessStatusCode();

                sHtml = await response.Content.ReadAsStringAsync();
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Couldn't get stream from cellartracker: {exc.Message}");
                return;
            }

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(sHtml);

            m_cellar = Cellar.BuildFromDocument(doc);
            PopulateFilters();
        }

        private void PopulateFilters()
        {
            m_lbxColor.Items.Clear();
            m_lbxLocation.Items.Clear();

            HashSet<string> locations = new HashSet<string>();
            HashSet<string> colors = new HashSet<string>();

            foreach (Bottle bottle in m_cellar.Bottles)
            {
                if (!locations.Contains(bottle.Location))
                    locations.Add(bottle.Location);
                if (!colors.Contains(bottle.Color))
                    colors.Add(bottle.Color);
            }

            foreach (string sLocation in locations)
                m_lbxLocation.Items.Add(sLocation);

            foreach (string sColor in colors)
                m_lbxColor.Items.Add(sColor);
        }

        string[] BuildStringArrayFromCheckedListbox(CheckedListBox lbx)
        {
            string[] rgs = null;

            if (lbx.CheckedItems.Count > 0)
            {
                rgs = new string[lbx.CheckedItems.Count];
                int i = 0;

                foreach (string s in lbx.CheckedItems)
                    rgs[i++] = s;
            }

            return rgs;
        }
        private void MakeList(object sender, EventArgs e)
        {
            // build the filters
            string[] rgsLocations = null;
            string[] rgsColors = null;
            bool fGroupByVarietal = m_fVarietalGrouping.Checked;

            rgsLocations = BuildStringArrayFromCheckedListbox(m_lbxLocation);
            rgsColors = BuildStringArrayFromCheckedListbox(m_lbxColor);

            WineList list = WineList.BuildFromCellar(m_cellar, rgsLocations, rgsColors, fGroupByVarietal);

            if (list.Bottles.Count == 0)
            {
                MessageBox.Show("Wine list is empty");
                return;
            }

            using (TextWriter tw = new StreamWriter(m_ebOutFile.Text))
            {
                tw.WriteLine("<HTML xmlns:w='urn:schemas-microsoft-com:office:word'><meta http-equiv='Content-Type' content='text/html; charset=utf-8'>");
                tw.WriteLine("<BODY>");
                string sColorCurrent = "";
                string sCountryCurrent = "";
                string sSubRegionAppellationCurrent = "";

                if (!fGroupByVarietal)
                {
                    if ((rgsColors != null && rgsColors.Length == 1) || list.Bottles.Count == 1)
                        sColorCurrent = list.Bottles[0].Color;
                }

                foreach (Bottle bottle in list.Bottles)
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
                            sbInfo.AppendFormat("{0}", bottle.Count); // could add "bottles" suffix here...
                        else
                            sbInfo.AppendFormat(",{0}", bottle.Count);
                    }

                    tw.Write("<p class=Wine>");
                    if (bottle.Wine.Length > 70)
                    {
                        // split into two lines
                        int iSplit = bottle.Wine.LastIndexOf(' ', 70);
                        tw.Write(bottle.Wine.Substring(0, iSplit));
                        tw.Write("</p>");
                        tw.Write("<p class=Wine>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                        tw.Write(bottle.Wine.Substring(iSplit + 1));
                    }
                    else
                    {
                        tw.Write(bottle.Wine);
                    }

                    tw.Write("<w:PTab Alignment=\"RIGHT\" RelativeTo=\"MARGIN\" Leader=\"NONE\">");
                    tw.Write(sbInfo.ToString());
                    tw.WriteLine("</w:PTab></p>");
                }

                tw.WriteLine("</BODY></HTML>");
                tw.Close();
            }
        }
    }
}
