using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Reporting;
using CrystalDecisions.ReportSource;
using CrystalDecisions.Shared;
using System.IO;
using System.Configuration;
using System.Drawing.Printing;

using System.Net;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Net.Mime;
using iTextSharp.tool.xml;
using System.Web.UI.HtmlControls;
using System.Net.Mail;
using System.Data.SqlClient;

public partial class Sugar_Report_rptudhariSupplierreport : System.Web.UI.Page
{

    string fromdt = string.Empty;
    string todt = string.Empty;
    string mail = string.Empty;
    int company_code;
    int year_code;
    string unitno = string.Empty;
    string Accode = string.Empty;
    string company_name = string.Empty;
    string Email = string.Empty;
    string State = string.Empty;
    string Address = string.Empty;
    ReportDocument rprt1 = new ReportDocument();
    string datefrom = "";
    string dateto = "";
    string pagehead = string.Empty;
    string tblPrefix = string.Empty;
    string isAuthenticate = string.Empty;
    string user = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            user = Session["user"].ToString();
            isAuthenticate = Security.Authenticate(tblPrefix, user);
            string User_Type = clsCommon.getString("Select User_Type from tblUser WHERE User_Name='" + user + "'");
            if (isAuthenticate == "1" || User_Type == "A")
            {

                Accode = Request.QueryString["Group_Code"];
                company_code = Convert.ToInt32(Session["Company_Code"].ToString());
                year_code = Convert.ToInt32(Session["year"].ToString());
                fromdt = Request.QueryString["fromdt"];
                todt = Request.QueryString["todt"];
                pagehead = "Supplier Udhari Register from " + fromdt + " To " + todt;

                datefrom = DateTime.Parse(fromdt, System.Globalization.CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyy-MM-dd");
                dateto = DateTime.Parse(todt, System.Globalization.CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyy-MM-dd");

                DataTable dt = GetData(fromdt, todt);
                SqlDataAdapter da = new SqlDataAdapter();

                rprt1.Load(Server.MapPath("cryudhari.rpt"));
                rprt1.SetDataSource(dt);
                CryPurchaseRegister.ReportSource = rprt1;

                company_name = Session["Company_Name"].ToString();
                // State = Session["state"].ToString();
                //Address = Session["address"].ToString();
                rprt1.DataDefinition.FormulaFields["company_name"].Text = "\"" + company_name + "\"";
                rprt1.DataDefinition.FormulaFields["pagehead"].Text = "\"" + pagehead + "\"";
            }
            else
            {
                Response.Redirect("~/UnAuthorized/Unauthorized_User.aspx", false);
            }
        }
       
        

        catch (Exception)
        {
            throw;
        }
    }
      private DataTable GetData(string fromdt, string todt)
    {
        DataTable dt = new DataTable();
        DataTable dtreturn = new DataTable();
        string strcon = System.Configuration.ConfigurationManager.ConnectionStrings["sqlconnection"].ConnectionString;
        using (SqlConnection con = new SqlConnection(strcon))
        {
            string qry = "";
            if (Accode != "0")
            {
                qry = "select * from qryGledgernew where ac_code in(select ac_code from qrygledgernew group by AC_CODE  having  isnull(sum(case when drcr='D' then amount else -amount end),0)  <>0) and  DRCR='C' and TRAN_TYPE in('PG','OP') "
                 + " and group_code=" + Accode + "  and amount is not null and  Company_Code=" + Convert.ToInt32(Session["Company_Code"].ToString()) + " ORDER BY DOC_DATE DESC";

                SqlCommand cmd = new SqlCommand(qry, con);
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            else
            {
                qry = "select * from qryGledgernew where ac_code in(select ac_code from qrygledgernew having  sum(case when drcr='D' then amount else -amount end)  <>0) DRCR='C' and TRAN_TYPE in('PG','OP') "
                  + "' and amount is not null and  Company_Code=" + Convert.ToInt32(Session["Company_Code"].ToString());

                SqlCommand cmd = new SqlCommand(qry, con);
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
        }
        dt.DefaultView.Sort = "DOC_DATE desc,DOC_NO desc";
        dt = dt.DefaultView.ToTable();
        dtreturn.Columns.Add("DOC_NO", typeof(Int32));
        dtreturn.Columns.Add("DOC_DATE", typeof(string));

        dtreturn.Columns.Add("AMOUNT", typeof(double));
        dtreturn.Columns.Add("balance", typeof(double));
        dtreturn.Columns.Add("ClosingBlance", typeof(double));
        dtreturn.Columns.Add("Ac_Name_E", typeof(string));
        dtreturn.Columns.Add("AC_CODE", typeof(string));
        if (dt.Rows.Count > 0)
        {
            #region [accode]
            DataView view = new DataView(dt);
            DataTable distinctValues = view.ToTable(true, "AC_CODE");
            for (int i = 0; i < distinctValues.Rows.Count; i++)
            {
                string accode = distinctValues.Rows[i]["AC_CODE"].ToString();
                DataView view11;
                DataTable newdt;
                view11 = new DataView(dt, "AC_CODE='" + distinctValues.Rows[i]["AC_CODE"].ToString() + "'", "AC_CODE", DataViewRowState.CurrentRows);
                newdt = view11.ToTable(true, "AC_CODE", "DOC_NO", "Ac_Name_E", "AMOUNT", "DOC_DATE");

                string qry = "select SUM(case drcr when 'D' then AMOUNT when 'C' then -amount end) as Balance" +
                                " from qrygledger where AC_CODE=" + accode + " and COMPANY_CODE=" + Convert.ToInt32(Session["Company_Code"].ToString())
                                + "  group by AC_CODE,Ac_Name_E,CityName,Mobile_No having SUM(case drcr when 'D' then AMOUNT when 'C' then -amount end) !=0 order by Ac_Name_E";

                double closingbalance = Convert.ToDouble(clsCommon.getString(qry));
                if (newdt.Rows.Count > 0)
                {
                    #region [accodeamount]
                    double addamount = 0.00;
                    double billbalance = 0.00;
                    double balance = closingbalance;
                    for (int j = 0; j < newdt.Rows.Count; j++)
                    {
                        DataRow dr = dtreturn.NewRow();
                        double amount = Convert.ToDouble(newdt.Rows[j]["AMOUNT"].ToString());

                        if (amount >= balance)
                        {
                            billbalance = balance;

                            dr[0] = newdt.Rows[j]["DOC_NO"].ToString();
                            dr[1] = newdt.Rows[j]["DOC_DATE"].ToString();
                            dr[2] = newdt.Rows[j]["amount"].ToString();
                            dr[3] = balance;
                            dr[4] = closingbalance;
                            dr[5] = newdt.Rows[j]["Ac_Name_E"].ToString();
                            dr[6] = newdt.Rows[j]["AC_CODE"].ToString();

                            dtreturn.Rows.Add(dr);
                            break;

                        }
                        else
                        {
                            billbalance = amount;

                            dr[0] = newdt.Rows[j]["DOC_NO"].ToString();
                            dr[1] = newdt.Rows[j]["DOC_DATE"].ToString();
                            dr[2] = newdt.Rows[j]["amount"].ToString();
                            dr[3] = newdt.Rows[j]["amount"].ToString();
                            dr[4] = closingbalance;
                            dr[5] = newdt.Rows[j]["Ac_Name_E"].ToString();
                            dr[6] = newdt.Rows[j]["AC_CODE"].ToString();
                            balance = balance - amount;
                            dtreturn.Rows.Add(dr);
                        }

                    }
                    #endregion
                }

            }
            #endregion

        }



        return dtreturn;
    }




    protected void btnPDF_Click(object sender, EventArgs e)
    {
        try
        {
            // string filepath=@"D:\pdffiles\cryChequePrinting.pdf";
            string filepath = "C:\\PDFFiles";

            if (!System.IO.Directory.Exists(filepath))
            {
                System.IO.Directory.CreateDirectory("C:\\PDFFiles");
            }
            string filename = filepath + "\\SupplierPurchaseOrder" + company_code + "_" + year_code + "_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".pdf";
            rprt1.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filename);

            //open PDF File

            //System.Diagnostics.Process.Start(filename);
            WebClient User = new WebClient();

            Byte[] FileBuffer = User.DownloadData(filename);

            if (FileBuffer != null)
            {

                Response.ContentType = "application/pdf";

                Response.AddHeader("content-length", FileBuffer.Length.ToString());

                Response.BinaryWrite(FileBuffer);

            }
        }
        catch (Exception e1)
        {
            Response.Write("PDF err:" + e1);
            return;
        }
        //   Response.Write("<script>alert('PDF successfully Generated');</script>");

    }
    protected void btnMail_Click(object sender, EventArgs e)
    {
        try
        {
            // string filepath = @"D:\ashwini\bhavani10012019\accowebBhavaniNew\PAN\cryChequePrinting.pdf";
            //string filepath = @"E:\Lata Software Backup\accowebnavkar\PAN\Saudapending.pdf";
            string filepath = "C:\\PDFFiles";

            if (!System.IO.Directory.Exists(filepath))
            {
                System.IO.Directory.CreateDirectory("C:\\PDFFiles");
            }
            string filename = filepath + "\\SupplierPurchaseOrder_" + company_code + "_" + year_code + "_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".pdf";
            rprt1.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filename);
            //rprt1.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filepath);

            if (txtEmail.Text != string.Empty)
            {
                //string fileName = "Saudapending.pdf";
                //string filepath1 = "~/PAN/" + fileName;

                mail = txtEmail.Text;

                ContentType contentType = new ContentType();
                contentType.MediaType = MediaTypeNames.Application.Pdf;
                contentType.Name = "SupplierPurchaseOrder";
                // Attachment attachment = new Attachment(Server.MapPath(filename), contentType);
                Attachment attachment = new Attachment(filename);
                string mailFrom = Session["EmailId"].ToString();
                string smtpPort = "587";
                string emailPassword = Session["EmailPassword"].ToString();
                EncryptPass enc = new EncryptPass();
                emailPassword = enc.Decrypt(emailPassword);
                MailMessage msg = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
                SmtpServer.Host = clsGV.Email_Address;
                msg.From = new MailAddress(mailFrom);
                msg.To.Add(mail);
                msg.Body = "SupplierPurchaseOrder";
                msg.Attachments.Add(attachment);
                msg.IsBodyHtml = true;
                msg.Subject = "No:";
                //msg.IsBodyHtml = true;
                if (smtpPort != string.Empty)
                {
                    SmtpServer.Port = Convert.ToInt32(smtpPort);
                }
                SmtpServer.EnableSsl = true;
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(mailFrom, emailPassword);
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate(object k,
                    System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                    System.Security.Cryptography.X509Certificates.X509Chain chain,
                    System.Net.Security.SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };
                SmtpServer.Send(msg);
                attachment.Dispose();
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
                Response.Write("<script>alert('Mail Send successfully');</script>");
            }


        }
        catch (Exception e1)
        {
            Response.Write("Mail err:" + e1);
            return;
        }


    }

    protected void Page_Unload(object sender, EventArgs e)
    {
        //rprt1.Close();
        //rprt1.Clone();
        //rprt1.Dispose();
        //GC.Collect();
        this.CryPurchaseRegister.ReportSource = null;

        CryPurchaseRegister.Dispose();

        if (rprt1 != null)
        {

            rprt1.Close();

            rprt1.Dispose();

            rprt1 = null;

        }

        GC.Collect();

        GC.WaitForPendingFinalizers();
    }
}
