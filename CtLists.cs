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
            MessageBox.Show("loaded");
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
    }
}
