using System;
using System.Collections.Generic;
using System.Windows.Forms;
using mshtml;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V125.Debugger;
using TCore.StatusBox;
using TCore.UI;
using TCore.WebControl;

namespace CtLists
{
    public class CellarTrackerWeb
    {
        private bool m_fLoggedIn;
        private string m_sUsername;
        private string m_sPassword;

        private StatusBox m_srpt;
        private WebControl m_wc;

        static string _s_Login = @"https://www.cellartracker.com/password.asp";
        private static string _s_LoginLegacy = @"https://www.cellartracker.com/classic/password.asp";

        private static string _s_CtlName_LoginName = @"szUser";
        private static string _s_CtlName_LoginPassword = @"szPassword";

        public CellarTrackerWeb(string sUsername, string sPassword, StatusBox srpt)
        {
            m_sUsername = sUsername;
            m_sPassword = sPassword;
            m_srpt = srpt;
            m_wc = new WebControl(srpt, true);
        }

        public void Show()
        {
            // m_wc.Visible = true;
        }

        void WaitForLoginPageAdvance(IEnumerable<string> names)
        {
            m_wc.WaitForCondition(
                (d) =>
                {
                    foreach (string name in names)
                    {
                        try
                        {
                            d.FindElement(By.Name(name));
                            return true;
                        }
                        catch
                        {
                        }
                    }

                    return false;
                },
                5000);
        }

        public void EnsureLoggedIn()
        {
            if (m_fLoggedIn)
                return;

            m_srpt.AddMessage("Logging in...");
            m_srpt.PushLevel();


            int cRetry = 2;

            while (cRetry > 0)
            {
                try
                {
                    if (!m_wc.FNavToPage(_s_LoginLegacy))
                        throw new Exception("couldn't navigate to login page");

                    WaitForLoginPageAdvance(
                        new[]
                        {
                            "szPassword"
                        });

                    m_wc.FSetTextForInputControlName("szUser", m_sUsername, false);
                    m_wc.FSetTextForInputControlName("szPassword", m_sPassword, false);

                    WebControl.FClickSubmitControlByValue(m_wc.Driver, "LOGIN");

                    if (!WebControl.WaitForControl(m_wc.Driver, m_srpt, "mywine"))
                        throw new Exception("couldn't find mywine control");

                    break;
                }
                catch (Exception e)
                {
                    MessageBox.Show("We might be stuck at a captcha. Please check and then continue", "Login", MessageBoxButtons.OK);
                    cRetry--;
                }
            }

            m_fLoggedIn = true;
            m_srpt.PopLevel();
            m_srpt.AddMessage("Login complete");
        }

        private static string _s_MyCellarPage = @"https://www.cellartracker.com/classic/list.asp?Table=List";
        private static string _s_CtlName_Cellar_Search = @"S";
        private static string _s_CtlName_DrinkOrRemove_Notes = @"ConsumptionNote";
        private static string _s_CtlName_DrinkOrRemove_DrinkDate = @"DrinkDate";
        private static string _s_CtlId_Cellar_MyWine = @"mywine";
        private static string _s_CtlName_DrinkOrRemove_Submit = @"CONSUME THIS BOTTLE";
        private static string _s_CtlId_WineInfo_Heading = @"hproduct";

        public void DrinkWine(string sBarcode, string sNotes, DateTime dttmConsumedUtc)
        {
            EnsureLoggedIn();

            m_srpt.AddMessage($"Drinking wine {sBarcode}...");
            m_srpt.PushLevel();

            if (!m_wc.FNavToPage(_s_MyCellarPage))
                throw new Exception("couldn't navigate to cellar page");

            m_wc.FSetTextForInputControlName(_s_CtlName_Cellar_Search, sBarcode, false);

            WebControl.FClickSubmitControlByValue(m_wc.Driver, "GO!");

            if (!WebControl.WaitForControlName(m_wc.Driver, m_srpt, _s_CtlName_DrinkOrRemove_Notes))
                throw new Exception("couldn't navigate to wine");

            if (!string.IsNullOrEmpty(sNotes))
                m_wc.FSetTextForInputControlName(_s_CtlName_DrinkOrRemove_Notes, sNotes, false);

            m_wc.FSetTextForInputControlName(_s_CtlName_DrinkOrRemove_DrinkDate, dttmConsumedUtc.ToLocalTime().ToString("MM/dd/yyyy"), false);

            m_wc.FSetSelectedOptionTextForControlName("ConsumptionType", "Drank from my cellar");

            WebControl.FClickSubmitControlByValue(m_wc.Driver, _s_CtlName_DrinkOrRemove_Submit);

            if (!WebControl.WaitForControl(m_wc.Driver, m_srpt, _s_CtlId_WineInfo_Heading))
                throw new Exception("couldn't submit wine");

            m_srpt.PopLevel();
            m_srpt.AddMessage("Drink complete");
        }

        public static bool FHasWineInTableRowOnPage(IHTMLDocument2 oDoc2, string sBarcode)
        {
            IHTMLElementCollection ihec;

            ihec = oDoc2.all.tags("table");

            foreach (IHTMLElement ihe in ihec)
            {
                IHTMLTable ihet = ihe as IHTMLTable;

                if (ihe.className == "editList")
                {
                    // we have the table, now see if we have a row that matches us.
                    if (ihet.rows.length < 2)
                        return false; // need to have at least 2 children

                    IHTMLTableRow iheRow = ihet.rows.item(1) as IHTMLTableRow;

                    if (iheRow.cells.length < 2)
                        return false; // has to have at least 2 children

                    IHTMLElement iheCell = iheRow.cells.item(1);

                    if (iheCell.innerText == sBarcode)
                        return true;

                    return false;
                }
            }

            return false;
        }


        public static bool FHasCorrectBinInTableRowOnPage(IHTMLDocument2 oDoc2, string sBin)
        {
            IHTMLElementCollection ihec;

            ihec = oDoc2.all.tags("table");

            foreach (IHTMLElement ihe in ihec)
            {
                IHTMLTable ihet = ihe as IHTMLTable;

                if (ihe.className == "editList")
                {
                    // we have the table, now see if we have a row that matches us.
                    if (ihet.rows.length < 2)
                        return false; // need to have at least 2 children

                    IHTMLTableRow iheRow = ihet.rows.item(1) as IHTMLTableRow;

                    if (iheRow.cells.length < 8)
                        return false; // has to have at least 2 children

                    IHTMLElement iheCell = iheRow.cells.item(7);

                    if (iheCell.innerText == sBin)
                        return true;

                    return false;
                }
            }

            return false;
        }

        private static string _s_InventoryScanPageRoot = @"https://www.cellartracker.com/classic/list.asp?Table=Scan";
        private static string _s_CtlName_InventoryScanPageRoot_SetBin = @"SetBin";
        private static string _s_CtlName_InventoryScanPageRoot_Submit_BulkUpdate = @"Bulk Update";

        public void RelocateWine(string sBarcode, string sBin)
        {
            EnsureLoggedIn();
            m_srpt.AddMessage($"Relocating wine {sBarcode}...");
            m_srpt.PushLevel();

            // navigate directly to the relocation page
            string sPage = $"{_s_InventoryScanPageRoot}&iInventoryList={sBarcode}";
            if (!m_wc.FNavToPage(sPage))
            {
                throw new Exception("couldn't navigate to cellar page");
            }

            string xpath = $"//tr[@class='properties']//td[2][contains(text(), '{sBarcode}')]";

            while (true)
            {
                try
                {
                    var d = m_wc.Driver.FindElement(By.XPath(xpath));
                    if (d != null)
                        break;
                }
                catch(Exception e)
                {
                    MessageBox.Show($"caught exception waiting for barcode {e.Message}");
                }

                InputBox.ShowInputBox("XPath", xpath, out xpath);
            }
            m_wc.WaitForXpath($"//tr[@class='properties']//td[2][contains(text(), '{sBarcode}')]", 5000);

            m_wc.FSetTextForInputControlName(_s_CtlName_InventoryScanPageRoot_SetBin, sBin, false);

            WebControl.FClickSubmitControlByValue(m_wc.Driver, _s_CtlName_InventoryScanPageRoot_Submit_BulkUpdate);

            m_wc.WaitForXpath($"//tr[@class='properties']//td[8][contains(text(), '{sBin}')]", 5000);

            m_srpt.PopLevel();
            m_srpt.AddMessage("Relocate complete");
        }

        public void Close()
        {
            if (m_wc.Driver != null)
            {
                m_wc.Driver.Close();
                m_wc.Driver.Quit();
                m_wc.Driver.Dispose();
                m_wc = null;
            }
        }
    }
}
