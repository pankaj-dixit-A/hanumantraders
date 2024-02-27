using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Sugar_Report_rptbrandrateUpdate : System.Web.UI.Page
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

                //txtFromDt.Text = clsGV.Start_Date;
                //txtToDt.Text = clsGV.To_date;

            }
            else
            {
                Response.Redirect("~/UnAuthorized/Unauthorized_User.aspx", false);
            }
        }
    }
    public void setFocusControl(WebControl wc)
    {
        objAsp = wc;
        System.Web.UI.ScriptManager.GetCurrent(this).SetFocus(wc);
    }
    protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].ControlStyle.Width = new Unit("50px");
            e.Row.Cells[1].ControlStyle.Width = new Unit("150px");
            e.Row.Cells[2].ControlStyle.Width = new Unit("50px");
            e.Row.Cells[3].ControlStyle.Width = new Unit("150px");
            e.Row.Cells[4].ControlStyle.Width = new Unit("70px");
            e.Row.Cells[5].ControlStyle.Width = new Unit("70px");
            e.Row.Cells[6].ControlStyle.Width = new Unit("50px");
            e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
            e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Center;
            e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Center;
            e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Center;
        }
           // e.Row.Cells[3].ControlStyle.Width = new Unit("100px");
        //e.Row.Cells[6].Visible = false;
    }
    protected void grdDetail_OnRowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            //GridViewRow row = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
            //int rowIndex = row.RowIndex;
        }
        catch (Exception)
        {
            throw;
        }
    }
    protected void Command_Click(object sender, CommandEventArgs e)
    {
        try
        {
            string qry = "";
            string rate = "";
            // string DOC_DTAE = DateTime.Parse(txtDate.Text, System.Globalization.CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyy/MM/dd");

            qry = "select Item_Code,ItemName,Brand_Code,brandName,qty,Wt_Per,isnull(Rate,0) as Rate From qryGrainBrandRateUpdate where balance!=0";


            dtData = new DataTable();
            DataSet ds = clsDAL.SimpleQuery(qry);
            dtData = ds.Tables[0];
            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                qry = "select rate from dailyrate where itemcode=" + dtData.Rows[i][0].ToString() + " and brandcode=" + dtData.Rows[i][2].ToString() + " and wtper=" + dtData.Rows[i][5].ToString() + "";
                rate = clsDAL.GetString(qry);
                if (rate != "")
                {
                    dtData.Rows[i][6] = rate;
                }
            }
            grdDetail.DataSource = dtData;
            grdDetail.DataBind();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    protected void btnUpdate_Click(object sender, CommandEventArgs e)
    {
         
        string insertqry = "delete from dailyrate";
        string action = string.Empty;
        string values = "";
        action = clsCommon.getString(insertqry);
         
        string insertvalues = "";
        if (grdDetail.Rows.Count > 0)
        {
            for (int i = 0; i < grdDetail.Rows.Count; i++)
            {
                TextBox txtRate = (TextBox)grdDetail.Rows[i].Cells[6].FindControl("txtRate");
                Label lblItemcode = (Label)grdDetail.Rows[i].Cells[0].FindControl("lblItemcode");
                Label lblBrandcode = (Label)grdDetail.Rows[i].Cells[2].FindControl("lblBrandcode");
                string Wt_Per = grdDetail.Rows[i].Cells[5].Text;
                if (txtRate.Text != "0.00")
                {
                    if (values != "")
                    {
                        values += ",(" + txtRate.Text + "," + lblItemcode.Text + "," + lblBrandcode.Text + "," + Wt_Per + ")";
                    }
                    else
                    {
                        values += "(" + txtRate.Text + "," + lblItemcode.Text + "," + lblBrandcode.Text + "," + Wt_Per + ")";
                    }
                }

            }
            if (values != "")
            {
                insertqry = "insert into dailyrate (rate,itemcode,brandcode,wtper) values " + values;
                action = clsCommon.getString(insertqry);
            }
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "alert('Successfully Updated!');", true);

        }
       // string a = insertqry;
      
    }

}