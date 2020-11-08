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

            Cellar celler = Cellar.BuildFromDocument(doc);
            MessageBox.Show("loaded");
        }
    }
}
