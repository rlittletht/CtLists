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
using TCore.KeyVault;
using TCore;

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

        private async Task DownloadCellar()
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

        private async void DoDownloadCellar(object sender, EventArgs e)
        {
            await DownloadCellar();
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

        private string s_sAppID = "bfbaffd7-2217-4deb-a85a-4f697e6bdf94";
        private string m_sAppTenant = "b90f9ef3-5e11-43e0-a75c-1f45e6b223fb";
        private string s_sConnectionStringSecretID = "Thetasoft-Azure-ConnectionString/324deaac388a480ab992ccef03072b61";

        private Client m_clientKeyVault = null;
        string sSqlConnectionString = null;

        private async void UpdateSql(object sender, EventArgs e)
        {
            if (m_cellar == null)
                await DownloadCellar();

            if (m_clientKeyVault == null)
            {
                m_clientKeyVault = new Client(m_sAppTenant, s_sAppID);

                sSqlConnectionString = await m_clientKeyVault.GetSecret(s_sConnectionStringSecretID);
            }

            // loop over all of our bottles and add the ones that are missing, and/or fix the scancodes for those
            // that have missing leading zeros
            SR sr;

            sr = TCore.Sql.OpenConnection(out Sql sql, sSqlConnectionString);

            if (!sr.Succeeded)
                throw new Exception($"can't open SQL connection: {sr.Reason}");

            string sSelect = "select ScanCode from upc_wines";

            sql.ExecuteReader(sSelect, out SqlReader sqlr, null);
            HashSet<string> hashOurBottles = new HashSet<string>();

            while (sqlr.Reader.Read())
            {
                string s = sqlr.Reader.GetString(0);
                hashOurBottles.Add(s);
            }

            sqlr.Close();

            // now we have all of the bottles we know about
            // build all the bottles from cellartracker

            HashSet<string> hashTheirBottles = new HashSet<string>();
            foreach (Bottle bottle in m_cellar.Bottles)
            {
                hashTheirBottles.Add(bottle.Barcode);
            }

            // now, which bottles do they have, but we don't
            HashSet<string> hashBottlesOnlyInCellarTracker = new HashSet<string>();
            HashSet<string> hashBottlesWithMissingLeadingZero = new HashSet<string>();
            HashSet<string> hashBottlesWithMissingLeadingZero2 = new HashSet<string>();
            HashSet<string> hashBottlesWithMissingLeadingZero3 = new HashSet<string>();
            HashSet<string> hashBottlesOnlyInOurCellar = new HashSet<string>();

            foreach (string s in hashTheirBottles)
            {
                if (!hashOurBottles.Contains(s))
                {
                    // bottle is missing. check to see if there's a missing leading zero
                    if (s.StartsWith("0") && hashOurBottles.Contains(s.Substring(1)))
                    {
                        hashBottlesWithMissingLeadingZero.Add(s);
                    }
                    else if (s.StartsWith("00") && hashOurBottles.Contains(s.Substring(2)))
                    {
                        hashBottlesWithMissingLeadingZero2.Add(s);
                    }
                    else if (s.StartsWith("000") && hashOurBottles.Contains(s.Substring(3)))
                    {
                        hashBottlesWithMissingLeadingZero3.Add(s);
                    }

                    else
                    {
                        hashBottlesOnlyInCellarTracker.Add(s);
                    }
                }
            }

            // now, let's not have the jurassic park problem...check the other direction too
            foreach (string s in hashOurBottles)
            {
                if (!hashTheirBottles.Contains(s))
                {
                    // bottle is not on cellar tracker...check to see if missing leading zero
                    if (hashTheirBottles.Contains($"0{s}"))
                    {
                        if (!hashBottlesWithMissingLeadingZero.Contains($"0{s}"))
                        {
                            MessageBox.Show($"Strange. We had a missing leading zero, didn't find it in the first pass... ({s})");
                            hashBottlesWithMissingLeadingZero.Add($"0{s}");
                        }
                    }
                    else
                    {
                        hashBottlesOnlyInOurCellar.Add(s);
                    }
                }
            }

            // at this point, we know what we have to add to our cellar, and what we have to fix
            MessageBox.Show(
                $"BrokenZeros1: {hashBottlesWithMissingLeadingZero.Count}, BrokenZeros2: {hashBottlesWithMissingLeadingZero2.Count}, BrokenZeros3: {hashBottlesWithMissingLeadingZero3.Count}. {hashBottlesOnlyInOurCellar.Count} only in our cellar, and {hashBottlesOnlyInCellarTracker.Count} only in CellarTracker");

            if (m_cbFixLeadingZeros.Checked)
            {
                sql.BeginTransaction();
                foreach (string s in hashBottlesWithMissingLeadingZero2)
                {
                    sql.SExecuteScalar($"update upc_wines set ScanCode='{s}' where ScanCode='{s.Substring(2)}'");
                    sql.SExecuteScalar($"update upc_codes set ScanCode='{s}' where ScanCode='{s.Substring(2)}'");
                }

                sql.Commit();
            }

            // now add any bottles that haven't been added yet...
            //
            sql.BeginTransaction();

            foreach (string s in hashBottlesOnlyInCellarTracker)
            {
                Bottle bottle = m_cellar[s];

                string sInsert =
                    "insert into upc_wines (ScanCode, Wine, Vintage, Locale, Country, Region, SubRegion, Appelation, Producer, [Type], Color, Category, Varietal, Designation, Vineyard, Score, [Begin], [End], iWine, Consumed, Notes, UpdatedCT, Bin, Location)"
                    + " VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}',{21},'{22}','{23}')";

                string sConsumed = bottle.GetValueOrEmpty("Consumed");

                if (sConsumed.Length == 0)
                    sConsumed = "1900-01-01 00";

                string sUpdatedCT = bottle.GetValueOrEmpty("UpdatedCT");

                if (sUpdatedCT.Length == 0)
                    sUpdatedCT = "0";

                string sQuery = String.Format(
                    sInsert,
                    Sql.Sqlify(bottle.GetValueOrEmpty("Barcode")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Wine")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Vintage")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Locale")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Country")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Region")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("SubRegion")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Appellation")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Producer")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Type")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Color")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Category")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Varietal")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Designation")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Vineyard")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Score")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Begin")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("End")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("iWine")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Consumed")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Notes")),
                    Sql.Sqlify(sUpdatedCT),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Bin")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Location")));

                string sResult = sql.SExecuteScalar(sQuery);

                sQuery = String.Format(
                    "insert into upc_codes (ScanCode, DescriptionShort, FirstScanDate,LastScanDate) VALUES ('{0}','{1}','{2}','{3}')",
                    Sql.Sqlify(bottle.GetValueOrEmpty("Barcode")),
                    Sql.Sqlify(bottle.GetValueOrEmpty("Wine")),
                    "1900-01-01 00:00:00.000",
                    "1900-01-01 00:00:00.000");

                sResult = sql.SExecuteScalar(sQuery);

            }

            sql.Commit();
        }
    }
}
