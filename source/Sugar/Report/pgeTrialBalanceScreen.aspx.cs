using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;
using System.IO;
using ClosedXML.Excel;
using System.Text;

public partial class Report_pgeTrialBalanceScreen : System.Web.UI.Page
{
    string qry = string.Empty;
    string isAuthenticate = string.Empty;
    string user = string.Empty;
    string tblPrefix = string.Empty;
    DataTable dtData;
    static WebControl objAsp = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        user = Session["user"].ToString();
        tblPrefix = Session["tblPrefix"].ToString();
        if (!Page.IsPostBack)
        {
            isAuthenticate = Security.Authenticate(tblPrefix, user);
            string User_Type = clsCommon.getString("Select User_Type from tblUser WHERE User_Name='" + user + "'");
            if (isAuthenticate == "1" || User_Type == "A")
            {
                //grdDetail.UseAccessibleHeader = true;
                //grdDetail.HeaderRow.TableSection = TableRowSection.TableHeader;
                txtDate.Text = clsGV.End_Date;
                txtFromDt.Text = clsGV.Start_Date;
                txtToDt.Text = clsGV.To_date;
                ViewState["sortOrder"] = "";
                ViewState["qry"] = null;
                ViewState["DrCr"] = null;
                ViewState["filterDt"] = null;
                //Data("", "", "", "");
            }
            else
            {
                Response.Redirect("~/UnAuthorized/Unauthorized_User.aspx", false);
            }
        }
        //this.RegisterPostBackControl();
    }
    private void RegisterPostBackControl()
    {
        foreach (GridViewRow row in grdDetail.Rows)
        {
            LinkButton lnkAcName = row.FindControl("lnkAcName") as LinkButton;
            ScriptManager.GetCurrent(this).RegisterPostBackControl(lnkAcName);
        }
    }
    public void setFocusControl(WebControl wc)
    {
        objAsp = wc;
        System.Web.UI.ScriptManager.GetCurrent(this).SetFocus(wc);
    }
    public static DataTable Data(string qry, string DrCr, string sortExp, string sortDir)
    {
        try
        {
            DataTable dtT = new DataTable();
            dtT.Columns.Add("accode", typeof(Int32));
            dtT.Columns.Add("acname", typeof(string));
            dtT.Columns.Add("city", typeof(string));
            dtT.Columns.Add("debitAmt", typeof(double));
            dtT.Columns.Add("creditAmt", typeof(double));
            dtT.Columns.Add("mobile", typeof(string));
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataView dv;
            ds = clsDAL.SimpleQuery(qry);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        dv = new DataView();
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DataRow dr = dtT.NewRow();
                            dr["accode"] = dt.Rows[i]["AC_CODE"].ToString();
                            dr["acname"] = dt.Rows[i]["Ac_Name_E"].ToString();
                            dr["city"] = dt.Rows[i]["CityName"].ToString();
                            dr["mobile"] = dt.Rows[i]["Mobile_No"].ToString();
                            double bal = Convert.ToDouble(ds.Tables[0].Rows[i]["Balance"].ToString());
                            if (DrCr == "Dr")
                            {
                                if (bal > 0)
                                {
                                    dr["debitAmt"] = bal.ToString();
                                    dr["creditAmt"] = 0.00;
                                    dtT.Rows.Add(dr);
                                }
                            }
                            else if (DrCr == "Cr")
                            {
                                if (bal < 0)
                                {
                                    dr["debitAmt"] = 0.00;
                                    dr["creditAmt"] = Math.Abs(bal);
                                    dtT.Rows.Add(dr);
                                }
                            }
                            else
                            {
                                if (bal > 0)
                                {
                                    // groupdebitamt += bal;
                                    dr["debitAmt"] = bal.ToString();
                                    dr["creditAmt"] = 0.00;
                                }
                                else
                                {
                                    //  groupcreditamt += -bal;
                                    dr["debitAmt"] = 0.00;
                                    dr["creditAmt"] = Math.Abs(bal);
                                }
                                dtT.Rows.Add(dr);
                            }
                        }
                        dv = sortingDT(sortExp, sortDir, dtT, dv);
                        dtT = (DataTable)dv.ToTable();
                    }
                }
            }
            return dtT;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static DataView sortingDT(string sortExp, string sortDir, DataTable dtT, DataView dv)
    {
        dv = dtT.DefaultView;
        if (sortExp != string.Empty)
        {
            dv.Sort = string.Format("{0} {1}", sortExp, sortDir);
        }
        return dv;
    }


    protected void Command_Click(object sender, CommandEventArgs e)
    {
        try
        {
            string qry = "";
            string Ac_type = drpType.SelectedValue.ToString();
            string DOC_DTAE = DateTime.Parse(txtDate.Text, System.Globalization.CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyy/MM/dd");

            if (Ac_type == "A")
            {
                qry = "select AC_CODE,Ac_Name_E,CityName, SUM(case drcr when 'D' then AMOUNT when 'C' then -amount end) as Balance ,Mobile_No" +
                            " from qrygledger where  DOC_DATE<='" + DOC_DTAE + "' and COMPANY_CODE=" + Convert.ToInt32(Session["Company_Code"].ToString())
                            + "  group by AC_CODE,Ac_Name_E,CityName,Mobile_No having SUM(case drcr when 'D' then AMOUNT when 'C' then -amount end) !=0 order by Ac_Name_E";

              
            }
            else
            {
                qry = "select AC_CODE,Ac_Name_E,CityName, SUM(case drcr when 'D' then AMOUNT when 'C' then -amount end) as Balance ,Mobile_No" +
                " from qrygledger where Ac_type='" + Ac_type + "' and DOC_DATE<='" + DOC_DTAE + "' and COMPANY_CODE=" + Convert.ToInt32(Session["Company_Code"].ToString()) 
                + "  group by AC_CODE,Ac_Name_E,CityName,Mobile_No having SUM(case drcr when 'D' then AMOUNT when 'C' then -amount end) !=0 order by Ac_Name_E";

              
            }
            dtData = new DataTable();

            switch (e.CommandName)
            {
                case "DrCr":
                    ViewState["qry"] = qry;
                    ViewState["DrCr"] = "DrCr";
                    ViewState["filterDt"] = null;
                    dtData = Data(qry, "DrCr", "", "");
                    break;

                case "Dr":
                    ViewState["qry"] = qry;
                    ViewState["DrCr"] = "Dr";
                    ViewState["filterDt"] = null;
                    dtData = Data(qry, "Dr", "", "");
                    break;

                case "Cr":
                    ViewState["qry"] = qry;
                    ViewState["DrCr"] = "Cr";
                    ViewState["filterDt"] = null;
                    dtData = Data(qry, "Cr", "", "");
                    break;
            }
            ViewState["gridData"] = dtData;
            grdDetail.DataSource = dtData;
            grdDetail.DataBind();
            this.RegisterPostBackControl();
        }
        catch (Exception)
        {

            throw;
        }
    }

    protected void Command1_Click(object sender, CommandEventArgs e)
    {
        try
        {
            string qry = "";
            string Ac_type = drpType.SelectedValue.ToString();
            string DOC_DTAE = DateTime.Parse(txtDate.Text, System.Globalization.CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyy/MM/dd");

            string FromDT = DateTime.Parse(txtFromDt.Text, System.Globalization.CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyy/MM/dd");
            string ToDT = DateTime.Parse(txtToDt.Text, System.Globalization.CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyy/MM/dd");

            if (Ac_type == "A")
            {
                qry = "select AC_CODE,Ac_Name_E,CityName, SUM(case drcr when 'D' then AMOUNT when 'C' then -amount end) as Balance ,Mobile_No" +
                            " from qrygledger where  DOC_DATE between '" + FromDT + "' and '" + ToDT + "' and COMPANY_CODE=" + Convert.ToInt32(Session["Company_Code"].ToString())
                            + "  group by AC_CODE,Ac_Name_E,CityName,Mobile_No having SUM(case drcr when 'D' then AMOUNT when 'C' then -amount end) !=0 order by Ac_Name_E";


            }
            else
            {
                qry = "select AC_CODE,Ac_Name_E,CityName, SUM(case drcr when 'D' then AMOUNT when 'C' then -amount end) as Balance ,Mobile_No" +
                " from qrygledger where Ac_type='" + Ac_type + "' and DOC_DATE between '" + FromDT + "' and '" + ToDT + "' and COMPANY_CODE=" + Convert.ToInt32(Session["Company_Code"].ToString())
                + "  group by AC_CODE,Ac_Name_E,CityName,Mobile_No having SUM(case drcr when 'D' then AMOUNT when 'C' then -amount end) !=0 order by Ac_Name_E";


            }
            dtData = new DataTable();

            switch (e.CommandName)
            {
                case "DrCr":
                    ViewState["qry"] = qry;
                    ViewState["DrCr"] = "DrCr";
                    ViewState["filterDt"] = null;
                    dtData = Data(qry, "DrCr", "", "");
                    break;

                case "Dr":
                    ViewState["qry"] = qry;
                    ViewState["DrCr"] = "Dr";
                    ViewState["filterDt"] = null;
                    dtData = Data(qry, "Dr", "", "");
                    break;

                case "Cr":
                    ViewState["qry"] = qry;
                    ViewState["DrCr"] = "Cr";
                    ViewState["filterDt"] = null;
                    dtData = Data(qry, "Cr", "", "");
                    break;
            }
            ViewState["gridData"] = dtData;
            grdDetail.DataSource = dtData;
            grdDetail.DataBind();
            this.RegisterPostBackControl();
        }
        catch (Exception)
        {

            throw;
        }
    }

    protected void btnSendSMS_Click(object sender, EventArgs e)
    {
        try
        {
            DataTable dtT = new DataTable();
            dtT.Columns.Add("mobile", typeof(string));
            dtT.Columns.Add("msgBody", typeof(string));

            for (int i = 0; i < grdDetail.Rows.Count; i++)
            {
                CheckBox chk = (CheckBox)grdDetail.Rows[i].Cells[7].FindControl("chkIsPrint");
                if (chk.Checked)
                {
                    TextBox txtMobile = (TextBox)grdDetail.Rows[i].Cells[5].FindControl("txtMobile");
                    Label lblDebit = (Label)grdDetail.Rows[i].Cells[3].FindControl("lblDebit");
                    string debitAmount = lblDebit.Text;
                    string mobile = txtMobile.Text;
                    if (mobile != string.Empty)
                    {
                        if (debitAmount != "0")
                        {
                            string msgAPI = clsGV.msgAPI;
                            msgAPI = msgAPI + "mobile=" + mobile + "&message=" + Session["Company_Name"].ToString() + ":- Your A/c shows debit balance Rs." + debitAmount + ". Please Send Urgently";
                            clsCommon.apicall(msgAPI);
                        }
                    }
                }
            }
        }
        catch
        {

        }
    }
    protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TextBox txtMobile = (TextBox)e.Row.Cells[5].FindControl("txtMobile");
            txtMobile.Text = e.Row.Cells[6].Text;

            if (txtMobile.Text.Contains("&nb"))
            {
                txtMobile.Text = "";
            }
        }
        e.Row.Cells[6].Visible = false;
    }
    protected void grdDetail_OnRowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            //GridViewRow row = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
            //int rowIndex = row.RowIndex;
            //string accode = grdDetail.Rows[rowIndex].Cells[0].Text.ToString();
            //string fromdt = DateTime.Parse(clsGV.Start_Date, System.Globalization.CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyy/MM/dd");
            //string todt = DateTime.Parse(clsGV.End_Date, System.Globalization.CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyy/MM/dd");
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ky", "javascript:sp('" + accode + "','" + fromdt + "','" + todt + "','DrCr')", true);
            //string rowID = "row" + rowIndex;
            //int lastCount = grdDetail.Rows.Count - rowIndex;
            //int remain = lastCount - 9;
            //if (remain <= 0)
            //{
            //    grdDetail.Rows[grdDetail.Rows.Count - 1].Cells[1].Focus();
            //}
            //else
            //{
            //    grdDetail.Rows[rowIndex + 9].Cells[1].Focus();
            //}
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ks", "javascript:ChangeRowColor(" + "'" + rowID + "'" + ")", true);
        }
        catch (Exception)
        {
            throw;
        }
    }

    protected void grdDetail_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //foreach (GridViewRow row in grdDetail.Rows)
            //{
            //    if (row.RowIndex == grdDetail.SelectedIndex)
            //    {
            //        row.BackColor = ColorTranslator.FromHtml("#A1DCF2");
            //    }
            //    else
            //    {
            //        row.BackColor = ColorTranslator.FromHtml("#FFFFFF");
            //    }
            //}
        }
        catch (Exception)
        {
            throw;
        }
    }
    protected void grdDetail_RowCreated(object sender, GridViewRowEventArgs e)
    {
        string rowID = String.Empty;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            rowID = "row" + e.Row.RowIndex;
            e.Row.Attributes.Add("id", "row" + e.Row.RowIndex);
            e.Row.Attributes.Add("onclick", "ChangeRowColor(" + "'" + rowID + "'" + ")");
        }
    }
    protected void drpFilter_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            bool isValidated = true;
            if (txtFromRs.Text != string.Empty)
            {
                isValidated = true;
            }
            else
            {
                isValidated = false;
                setFocusControl(txtFromRs);
                return;
            }
            if (txtToRs.Text != string.Empty)
            {
                isValidated = true;
            }
            else
            {
                isValidated = false;
                setFocusControl(txtToRs);
                return;

            }
            string filterExpr = drpFilter.SelectedValue.ToString();
            dtData = new DataTable();
            dtData = (DataTable)ViewState["gridData"];
            double FromRs = Convert.ToDouble(txtFromRs.Text);
            double ToRs = Convert.ToDouble(txtToRs.Text);
            DataRow[] result;
            if (filterExpr == "C")
            {
                result = dtData.Select("creditAmt >= " + FromRs + " and creditAmt <= " + ToRs + "");
            }
            else
            {
                result = dtData.Select("debitAmt >= " + FromRs + " and debitAmt <= " + ToRs + "");
            }

            DataTable dtClone = dtData.Clone();
            foreach (DataRow row in result)
            {
                dtClone.ImportRow(row);
            }
            ViewState["filterDt"] = dtClone;
            grdDetail.DataSource = dtClone;
            grdDetail.DataBind();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public string sortOrder
    {
        get
        {
            if (ViewState["sortOrder"].ToString() == "desc")
            {
                ViewState["sortOrder"] = "asc";
            }
            else
            {
                ViewState["sortOrder"] = "desc";
            }
            return ViewState["sortOrder"].ToString();
        }
        set
        {
            ViewState["sortOrder"] = value;
        }
    }
    protected void lnkAcName_Click(object sender, EventArgs e)
    {
        try
        {
            LinkButton lnkAcName = (LinkButton)sender;
            GridViewRow row = (GridViewRow)lnkAcName.NamingContainer;
            int rowIndex = row.RowIndex;
            Label lblAc_Code = grdDetail.Rows[rowIndex].Cells[0].FindControl("lblAc_Code") as Label;

            string accode = lblAc_Code.Text.ToString();
            string fromdt = DateTime.Parse(clsGV.Start_Date, System.Globalization.CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyy/MM/dd");
            string todt = DateTime.Parse(txtDate.Text.ToString(), System.Globalization.CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyy/MM/dd");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ky", "javascript:sp('" + accode + "','" + fromdt + "','" + todt + "','DrCr')", true);
            string rowID = "row" + rowIndex;
            int lastCount = grdDetail.Rows.Count - rowIndex;
            int remain = lastCount - 9;
            if (remain <= 0)
            {
                grdDetail.Rows[grdDetail.Rows.Count - 1].Cells[1].Focus();
            }
            else
            {
                grdDetail.Rows[rowIndex + 9].Cells[1].Focus();
            }
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ks", "javascript:ChangeRowColor(" + "'" + rowID + "'" + ")", true);
        }
        catch (Exception)
        {
            throw;
        }
    }

    protected void grdDetail_Sorting(object sender, GridViewSortEventArgs e)
    {
        try
        {
            DataTable sortedDt;
            string qry2 = string.Empty;
            string drcr2 = string.Empty;
            if (ViewState["qry"] != null)
            {
                qry2 = ViewState["qry"].ToString();
            }
            if (ViewState["DrCr"] != null)
            {
                drcr2 = ViewState["DrCr"].ToString();
            }
            if (ViewState["filterDt"] != null)
            {
                sortedDt = new DataTable();
                sortedDt = (DataTable)ViewState["filterDt"];
                DataView dv = new DataView();
                dv = sortingDT(e.SortExpression, sortOrder, sortedDt, dv);
                sortedDt = (DataTable)dv.ToTable();
            }
            else
            {
                sortedDt = new DataTable();
                sortedDt = Data(qry2, drcr2, e.SortExpression, sortOrder);
            }
            ViewState["gridData"] = sortedDt;
            grdDetail.DataSource = sortedDt;
            grdDetail.DataBind();
        }
        catch (Exception)
        {
            throw;
        }
    }

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        try
        {
           DataTable dtGrid = new DataTable();
            dtGrid = (DataTable)ViewState["gridData"];
            string attachment = "attachment; filename=trialbalacescreen.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";
            string tab = "";
            foreach (DataColumn dc in dtGrid.Columns)
            {
                Response.Write(tab + dc.ColumnName);
                tab = "\t";
            }
            Response.Write("\n");
            int i;
            foreach (DataRow dr in dtGrid.Rows)
            {
                tab = "";
                for (i = 0; i < dtGrid.Columns.Count; i++)
                {
                    Response.Write(tab + dr[i].ToString());
                    tab = "\t";
                }
                Response.Write("\n");
            }
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            Response.End();
        //    using (XLWorkbook wb = new XLWorkbook())
        //    {
        //        wb.Worksheets.Add(dtGrid);

        //        Response.Clear();
        //        Response.Buffer = true;
        //        Response.Charset = "";
        //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        Response.AddHeader("content-disposition", "attachment;filename=GridView.xlsx");
        //        using (MemoryStream MyMemoryStream = new MemoryStream())
        //        {
        //            wb.SaveAs(MyMemoryStream);
        //            MyMemoryStream.WriteTo(Response.OutputStream);
        //            Response.Flush();
        //            Response.End();
        //        }
        //    }
        }
        catch (Exception ex)
        {
          
        }
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        /* Verifies that the control is rendered */
    }

    protected void btnPrintGrid_Click(object sender, EventArgs e)
    {
        //grdDetail.PagerSettings.Visible = false;
        //grdDetail.DataBind();
        //StringWriter sw = new StringWriter();
        //HtmlTextWriter hw = new HtmlTextWriter(sw);
        //grdDetail.RenderControl(hw);
        //string gridHTML = sw.ToString().Replace("\"", "'")
        //    .Replace(System.Environment.NewLine, "");
        //StringBuilder sb = new StringBuilder();
        //sb.Append("<script type = 'text/javascript'>");
        //sb.Append("window.onload = new function(){");
        //sb.Append("var printWin = window.open('', '', 'left=0");
        //sb.Append(",top=0,width=1000,height=600,status=0');");
        //sb.Append("printWin.document.write(\"");
        //sb.Append(gridHTML);
        //sb.Append("\");");
        ////sb.Append("printWin.document.close();");
        //sb.Append("printWin.focus();");
        //sb.Append("printWin.print();");
        ////sb.Append("printWin.close();};");
        //sb.Append("</script>");
        //ClientScript.RegisterStartupScript(this.GetType(), "GridPrint", sb.ToString());
       // ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "script", "<script type='text/javascript'>SB();</script>", false);
        //grdDetail.PagerSettings.Visible = true;
        //grdDetail.DataBind();
    }
}