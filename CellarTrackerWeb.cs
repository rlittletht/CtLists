using System;
using System.Collections.Generic;
using System.Windows.Forms;
using mshtml;
using StatusBox;

namespace CtLists
{
    public class CellarTrackerWeb
    {
        private bool m_fLoggedIn;
        private string m_sUsername;
        private string m_sPassword;

        private StatusRpt m_srpt;
        private WebControl m_wc;

        static string _s_Login = @"https://www.cellartracker.com/password.asp";
        private static string _s_LoginLegacy = @"https://www.cellartracker.com/classic/password.asp";

        private static string _s_CtlName_LoginName = @"szUser";
        private static string _s_CtlName_LoginPassword  = @"szPassword";

        public CellarTrackerWeb(string sUsername, string sPassword, StatusRpt srpt)
        {
            m_sUsername = sUsername;
            m_sPassword = sPassword;
            m_srpt = srpt;
            m_wc = new WebControl(srpt);
        }

        public void Show()
        {
            m_wc.Visible = true;
        }

        public void EnsureLoggedIn()
        {
            if (m_fLoggedIn)
                return;

            m_srpt.AddMessage("Logging in...");
            m_srpt.PushLevel();

            if (!m_wc.FNavToPage(_s_LoginLegacy) || !m_wc.FWaitForNavFinish())
                throw new Exception("couldn't navigate to login page");

            IHTMLDocument2 oDoc2 = m_wc.Document2;
            WebControl.FSetInputControlText(oDoc2, _s_CtlName_LoginName, m_sUsername, false);
            WebControl.FSetInputControlText(oDoc2, _s_CtlName_LoginPassword, m_sPassword, false);

            m_wc.ResetNav();
            IHTMLElement ihe = WebControl.FindSubmitControlByValueText(oDoc2, null);
            if (ihe == null)
                throw new Exception("no submit control?");

            ihe.click();
            m_wc.FWaitForNavFinish(_s_CtlId_Cellar_MyWine);
            m_fLoggedIn = true;
            m_srpt.PopLevel();
            m_srpt.AddMessage("Login complete");
            
        }

        private static string _s_MyCellarPage = @"https://www.cellartracker.com/classic/list.asp?Table=List";
        private static string _s_CtlName_Cellar_Search = @"S";
        private static string _s_CtlName_DrinkOrRemove_Notes = @"ConsumptionNote";
        private static string _s_CtlName_DrinkOrRemove_DrinkDate = @"DrinkDate";
        private static string _s_CtlId_Cellar_MyWine = @"mywine";
        private static string _s_CtlName_DrinkOrRemove_Submit = @"bSubmit";
        private static string _s_CtlId_WineInfo_Heading = @"hproduct";

        public void DrinkWine(string sBarcode, string sNotes, DateTime dttmConsumedUtc)
        {
            EnsureLoggedIn();

            m_srpt.AddMessage($"Drinking wine {sBarcode}...");
            m_srpt.PushLevel();

            if (!m_wc.FNavToPage(_s_MyCellarPage) || !m_wc.FWaitForNavFinish(_s_CtlId_Cellar_MyWine))
                throw new Exception("couldn't navigate to cellar page");

            IHTMLDocument2 oDoc2 = m_wc.Document2;
            WebControl.FSetInputControlText(oDoc2, _s_CtlName_Cellar_Search, sBarcode, false);

            m_wc.ResetNav();
            IHTMLElement ihe = WebControl.FindSubmitControlByValueText(oDoc2, "GO!");
            if (ihe == null)
                throw new Exception("no submit control?");

            ihe.click();
            m_wc.FWaitForNavFinish();

            oDoc2 = m_wc.Document2;
            if (!string.IsNullOrEmpty(sNotes))
                WebControl.FSetInputControlText(oDoc2, _s_CtlName_DrinkOrRemove_Notes, sNotes, false);

            WebControl.FSetInputControlText(oDoc2, _s_CtlName_DrinkOrRemove_DrinkDate, dttmConsumedUtc.ToLocalTime().ToString("MM/dd/yyyy"), false);

            Dictionary<string, string> valueMappings = WebControl.MpGetSelectValues(m_srpt, oDoc2, "ConsumptionType");

            WebControl.FSetSelectControlValue(oDoc2, "ConsumptionType", valueMappings["Drank from my cellar"], false);

            m_wc.ResetNav();
            ihe = WebControl.FindSubmitControlByName(oDoc2, _s_CtlName_DrinkOrRemove_Submit);
            if (ihe == null)
                throw new Exception("no submit control?");

            ihe.click();
            m_wc.FWaitForNavFinish(_s_CtlId_WineInfo_Heading);
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
            if (!m_wc.FNavToPage(sPage) || !m_wc.FWaitForNavFinish((doc2) => FHasWineInTableRowOnPage(doc2, sBarcode)))
            {
                throw new Exception("couldn't navigate to cellar page");
            }

            IHTMLDocument2 oDoc2 = m_wc.Document2;

            WebControl.FSetInputControlText(oDoc2, _s_CtlName_InventoryScanPageRoot_SetBin, sBin, false);

            m_wc.ResetNav();
            IHTMLElement ihe = WebControl.FindSubmitControlByValueText(oDoc2, _s_CtlName_InventoryScanPageRoot_Submit_BulkUpdate);
            if (ihe == null)
                throw new Exception("no submit control?");

            ihe.click();
            if (!m_wc.FWaitForNavFinish((doc2) => FHasCorrectBinInTableRowOnPage(doc2, sBin)))
                throw new Exception($"failed to relocate wine {sBarcode} to {sBin}");

            m_srpt.PopLevel();
            m_srpt.AddMessage("Relocate complete");
        }
    }
}