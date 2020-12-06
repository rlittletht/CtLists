using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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
using StatusBox;
using TCore.UI;

namespace CtLists
{
    public partial class CtLists : Form
    {
        private string m_username;
        private string m_password;
        private HttpClient m_client = new HttpClient();
        private StatusRpt m_srpt;

        public CtLists(string username, string password)
        {
            m_username = username;
            m_password = password;

            InitializeComponent();
            m_headingWineList.Text = "";
            m_headingCellarTrackerUpdate.Text = "";

            m_srpt = new StatusRpt(m_recStatus);
            //m_srpt.SetLogLevel(5);
            //m_srpt.SetFilter(StatusRpt.MSGT.Body);
        }

        private void RenderHeadingLine(object sender, PaintEventArgs e)
        {
            RenderSupp.RenderHeadingLine(sender, e);
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

            list.CreateFile(m_ebOutFile.Text, fGroupByVarietal, rgsColors == null || rgsColors.Length == 1);
        }

        private CtSql m_ctsql;

        void EnsureCtSql()
        {
            if (m_ctsql == null)
                m_ctsql = new CtSql();
        }

        async Task EnsureCellarDownloaded()
        {
            if (m_cellar == null)
                await DownloadCellar();
        }

        private async void UpdateSql(object sender, EventArgs e)
        {
            await EnsureCellarDownloaded();
            EnsureCtSql();

            await m_ctsql.UpdateLocalDatabaseFromDownloadedCellar(m_cellar, false, false);
        }
        
        private CellarTrackerWeb m_ctWeb;

        private async void DoDrinkWines(object sender, EventArgs e)
        {
            await EnsureCellarDownloaded();

            if (m_ctWeb == null)
                m_ctWeb = new CellarTrackerWeb(m_username, m_password, m_srpt);

            WineDrinker drinker = new WineDrinker(m_ctWeb);

            EnsureCtSql();
            await drinker.FindAndDrinkWines(m_cellar, m_ctsql, false);
        }

        private async void DoRelocateWines(object sender, EventArgs e)
        {
            await EnsureCellarDownloaded();

            if (m_ctWeb == null)
                m_ctWeb = new CellarTrackerWeb(m_username, m_password, m_srpt);

            WineMover uhaul = new WineMover(m_ctWeb);

            EnsureCtSql();
            await uhaul.FindAndRelocateWines(m_cellar, m_ctsql, false);
        }

        private async void DoSyncCheck(object sender, EventArgs e)
        {
            await EnsureCellarDownloaded();

            WineMover uhaul = new WineMover(null);
            WineDrinker drinker = new WineDrinker(null);

            EnsureCtSql();

            int leadingZero1, leadingZero2, inOurs, inTheirs;

            (leadingZero1, leadingZero2, inOurs, inTheirs) = await m_ctsql.UpdateLocalDatabaseFromDownloadedCellar(m_cellar, true, true);

            int cNeedToDrink = await drinker.FindAndDrinkWines(m_cellar, m_ctsql, true);
            int cNeedToMove = await uhaul.FindAndRelocateWines(m_cellar, m_ctsql, true);

            StringBuilder sb = new StringBuilder();

            if (cNeedToDrink > 0)
                sb.Append($"Need to drink: {cNeedToDrink}. RUN DRINKWINES! ");

            if (cNeedToMove > 0)
                sb.Append($"Need to move: {cNeedToDrink}. RUN RELOCATE! ");

            if (leadingZero1 > 0)
                sb.Append($"Leading zeroes broken: {leadingZero1}. FIX LEADING ZEROS! ");

            if (leadingZero2 > 0)
                sb.Append($"Leading double zeroes broken: {leadingZero2}. FIX LEADING ZEROS! ");

            if (inOurs > 0)
                sb.Append($"In our celler, but not CellarTracker: {inOurs}. DON'T KNOW HOW TO FIX!! ");

            if (inTheirs > 0)
                sb.Append($"In CellarTracker but not ours: {inTheirs}. RUN UPDATELOCAL!");

            if (sb.Length == 0)
                MessageBox.Show("Everything is up to date!!");
            else
                MessageBox.Show(sb.ToString());
        }
    }
}
