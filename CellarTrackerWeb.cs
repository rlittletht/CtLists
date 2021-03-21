using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HtmlAgilityPack;
using mshtml;
using TCore.StatusBox;
using TCore.WebControl;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

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
        private static string _s_CtlName_LoginPassword  = @"szPassword";

        public CellarTrackerWeb(string sUsername, string sPassword, StatusBox srpt, bool fShowUI)
        {
            m_sUsername = sUsername;
            m_sPassword = sPassword;
            m_srpt = srpt;
            m_wc = new WebControl(srpt, fShowUI);
        }

        public void EnsureLoggedIn()
        {
            if (m_fLoggedIn)
                return;

            m_srpt.AddMessage("Logging in...");
            m_srpt.PushLevel();

            if (!m_wc.FNavToPage(_s_LoginLegacy))
                throw new Exception("couldn't navigate to login page");

            m_wc.WaitForPageLoad();
            
            m_wc.FSetTextForInputControlName(_s_CtlName_LoginName, m_sUsername, false);
            m_wc.FSetTextForInputControlName(_s_CtlName_LoginPassword, m_sPassword, false);

            if (!m_wc.FClickSubmitControlByValue(null))
	            throw new Exception("can't click submit button");

            m_wc.WaitForPageLoad();
            
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

            if (!m_wc.FNavToPage(_s_MyCellarPage))
                throw new Exception("couldn't navigate to cellar page");

            m_wc.WaitForPageLoad();
            
            m_wc.FSetTextForInputControlName(_s_CtlName_Cellar_Search, sBarcode, false);
            
            if (!m_wc.FClickSubmitControlByValue("GO!"))
                throw new Exception("no submit control?");

            m_wc.WaitForPageLoad();

            if (!string.IsNullOrEmpty(sNotes))
	            m_wc.FSetTextForInputControlName(_s_CtlName_DrinkOrRemove_Notes, sNotes, false);

            m_wc.FSetTextForInputControlName(_s_CtlName_DrinkOrRemove_DrinkDate, dttmConsumedUtc.ToLocalTime().ToString("MM/dd/yyyy"), false);

            Dictionary<string, string> valueMappings = m_wc.GetOptionsTextValueMappingFromControlName("ConsumptionType");

            m_wc.FSetSelectedOptionTextForControlName("ConsumptionType", "Drank from my cellar");
            m_wc.FClickSubmitControlByValue(_s_CtlName_DrinkOrRemove_Submit);
            
            m_wc.WaitForPageLoad();
            m_srpt.PopLevel();
            m_srpt.AddMessage("Drink complete");
        }

        public static bool FHasWineInTableRowOnPage(WebControl webControl, string sBarcode)
        {
	        string sHtml = webControl.GetOuterHtmlForControlByXPath("//table[@class='editList']");
	        HtmlDocument html = new HtmlDocument();
	        html.LoadHtml(sHtml);

	        HtmlNode table = html.DocumentNode.SelectSingleNode("/table");
	        HtmlNodeCollection rows = table.SelectNodes("//tr");

	        // we have the table, now see if we have a row that matches us.
	        if (rows.Count < 2)
		        return false; // need to have at least 2 children

	        HtmlNode rowToCheck = rows[1];
	        HtmlNodeCollection cells = rowToCheck.SelectNodes("td");

	        if (cells.Count < 2)
		        return false; // has to have at least 2 children

	        HtmlNode cell = cells[1];

	        if (cell.InnerText.Trim() == sBarcode)
		        return true;

	        return false;
        }


        public static bool FHasCorrectBinInTableRowOnPage(WebControl webControl, string sBin)
        {
	        string sHtml = webControl.GetOuterHtmlForControlByXPath("//table[@class='editList']");
	        HtmlDocument html = new HtmlDocument();
	        html.LoadHtml(sHtml);

	        HtmlNode table = html.DocumentNode.SelectSingleNode("/table");
	        HtmlNodeCollection rows = table.SelectNodes("//tr");

	        // we have the table, now see if we have a row that matches us.
	        if (rows.Count < 2)
		        return false; // need to have at least 2 children

	        HtmlNode rowToCheck = rows[1];
	        HtmlNodeCollection cells = rowToCheck.SelectNodes("td");

	        if (cells.Count < 8)
		        return false; // has to have at least 2 children

	        HtmlNode cell = cells[7];

	        if (cell.InnerText.Trim() == sBin)
		        return true;

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

            m_wc.WaitForCondition((d) => FHasWineInTableRowOnPage(m_wc, sBarcode), 2500);
            
            m_wc.FSetTextForInputControlName(_s_CtlName_InventoryScanPageRoot_SetBin, sBin, false);
            m_wc.FClickSubmitControlByValue(_s_CtlName_InventoryScanPageRoot_Submit_BulkUpdate);
            m_wc.WaitForPageLoad();

            m_wc.WaitForCondition((d) => FHasCorrectBinInTableRowOnPage(m_wc, sBin), 2500);

            m_srpt.PopLevel();
            m_srpt.AddMessage("Relocate complete");
        }
    }
}