using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Text;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
public partial class Sugar_Master_pgeHamalimaster : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            showrecord();
        }
    }
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        string qry2 = "delete from Hamalirate";
        DataSet dsRR = new DataSet();
        dsRR = clsDAL.SimpleQuery(qry2);
        qry2 = "insert into Hamalirate (stage1,stage2,stage3) values ('" + txtStage1.Text + "','" + txtStage2.Text + "','" + txtStage3.Text + "')";
        dsRR = new DataSet();
        dsRR = clsDAL.SimpleQuery(qry2);
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), " ", "alert('Record Successfully Update !')", true);
    }
    protected void showrecord()
    {
        
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        string qry = "select * from Hamalirate";
        ds = clsDAL.SimpleQuery(qry);
        if (ds != null)
        {
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {

                    txtStage1.Text  = dt.Rows[0]["stage1"].ToString();
                    txtStage2.Text = dt.Rows[0]["stage2"].ToString();
                    txtStage3.Text = dt.Rows[0]["stage3"].ToString();
                    Session["Stage1"] = txtStage1.Text;
                    Session["Stage2"] = txtStage2.Text;
                    Session["Stage3"] = txtStage3.Text;
                }
            }
        }

    }
}