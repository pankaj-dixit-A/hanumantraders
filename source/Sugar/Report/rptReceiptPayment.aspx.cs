﻿using System;
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


public partial class Sugar_Report_rptReceiptPayment : System.Web.UI.Page
{
    string fromdt = string.Empty;
    string todt = string.Empty;
    string mail = string.Empty;
    int company_code;
    int year_code;
    string unitno = string.Empty;
    string Accode = string.Empty;
    string TranType = string.Empty;
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
    string ac_code=string.Empty;
    string instanceid = string.Empty;
    string ac_no = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            user = Session["user"].ToString();
            isAuthenticate = Security.Authenticate(tblPrefix, user);
            string User_Type = clsCommon.getString("Select User_Type from tblUser WHERE User_Name='" + user + "'");
            if (isAuthenticate == "1" || User_Type == "A")
            {

                Accode = Request.QueryString["doc_no"];
                company_code = Convert.ToInt32(Session["Company_Code"].ToString());
                year_code = Convert.ToInt32(Session["year"].ToString());
                TranType = Request.QueryString["TranType"];
                fromdt = Request.QueryString["fromdt"];
                todt = Request.QueryString["todt"];


                //pagehead = "Receipt Payment Register from " + fromdt + " To " + todt;

                //datefrom = DateTime.Parse(fromdt, System.Globalization.CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyy-MM-dd");
                //dateto = DateTime.Parse(todt, System.Globalization.CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyy-MM-dd");

                DataTable dt = GetData();
                SqlDataAdapter da = new SqlDataAdapter();

                string TotalAmount = dt.Rows[0]["total"].ToString();
                string inWords = clsNoToWord.ctgword(TotalAmount);
                ac_no = dt.Rows[0]["credit_ac"].ToString();

                ac_code = dt.Rows[0]["credit_ac"].ToString();
                if (!IsPostBack)
                {

                    txtEmail.Text = clsCommon.getString("Select Email_Id from qrymstaccountmaster where  Ac_Code='" + ac_code + "' and Company_Code=" + Convert.ToInt32(Session["Company_Code"].ToString()));

                }
                string closingbalance = "";
                closingbalance = clsCommon.getString("select SUM(case when drcr='D' then amount else -amount end) from nt_1_gledger where AC_CODE=" + ac_code );

                rprt1.Load(Server.MapPath("CryReiceptPayment.rpt"));
                rprt1.SetDataSource(dt);
                CryReceiptPayment.ReportSource = rprt1;
                string logo = clsCommon.getString("select ImagePath from tblsign where ImageOrLogo='S' and Company_Code='"
           + Session["Company_Code"].ToString() + "'");
                String logopath = Server.MapPath("") + "\\" + logo; 
                logo = logopath.Replace("Report", "Images");
                company_name = Session["Company_Name"].ToString();
               
                Address = Session["address"].ToString();
                rprt1.DataDefinition.FormulaFields["company_name"].Text = "\"" + company_name + "\"";
                rprt1.DataDefinition.FormulaFields["address"].Text = "\"" + Address + "\"";
                //rprt1.DataDefinition.FormulaFields["pagehead"].Text = "\"" + pagehead + "\"";
                rprt1.DataDefinition.FormulaFields["Inword"].Text = "\"" + inWords + "\"";
                rprt1.DataDefinition.FormulaFields["closingbalance"].Text = "\"" + closingbalance + "\"";
                string imagepath = clsCommon.getString("select ImagePath from tblsign where ImageOrLogo='L' and Company_Code='"
                + Session["Company_Code"].ToString() + "'");
                String path = Server.MapPath("") + "\\" + imagepath;
                imagepath = path.Replace("Report", "Images");
                rprt1.DataDefinition.FormulaFields["img"].Text = "\"" + imagepath + "\"";
                rprt1.DataDefinition.FormulaFields["logo"].Text = "\"" + logo + "\"";
                //btnPDF_Click(null,null);
            }
            else
            {
                Response.Redirect("~/UnAuthorized/Unauthorized_User.aspx", false);
            }
            if (!IsPostBack)
            {
                txtWhatsapp.Text = clsCommon.getString("Select Mobile_No from qrymstaccountmaster where  Ac_Code='" + ac_no + "' and Company_Code=" + Convert.ToInt32(Session["Company_Code"].ToString()));
            }
        }

        catch (Exception)
        {
            throw;
        }
    }

    private DataTable GetData()
    {
        DataTable dt = new DataTable();
        string strcon = System.Configuration.ConfigurationManager.ConnectionStrings["sqlconnection"].ConnectionString;
        using (SqlConnection con = new SqlConnection(strcon))
        {

            SqlCommand cmd = new SqlCommand("select * from qrytransheaddetail where doc_no=" + Accode + " and  tran_type='" + TranType + "' and  Company_Code=" + Convert.ToInt32(Session["Company_Code"].ToString()) + " and "
             + "  year_Code=" + Convert.ToInt32(Session["year"].ToString()) + "", con);
           
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
        }
        return dt;
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
        this.CryReceiptPayment.ReportSource = null;

        CryReceiptPayment.Dispose();

        if (rprt1 != null)
        {

            rprt1.Close();

            rprt1.Dispose();

            rprt1 = null;

        }

        GC.Collect();

        GC.WaitForPendingFinalizers();
    }


    protected void btnWhatsApp_Click(object sender, EventArgs e)
    {

        #region Main
        string mobileNumber = txtWhatsapp.Text.Trim();
        if (mobileNumber == string.Empty)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), " ", "alert('Please Enter Whatsapp No !');", true);
            return;
        }
        instanceid = clsCommon.getString("select Instance_Id from tblWhatsAppURL where Company_Code=" + Convert.ToInt32(Session["Company_Code"].ToString()));

        string bill_no = "DocNo:" + Accode;
        var base64String = "";
        string uploadedFileName = "";
        DataTable table = new DataTable();
        table.Columns.Add("mobno", typeof(string));
        table.Columns.Add("filename", typeof(string));
        string[] names = mobileNumber.Split(',');
        for (int i = 0; i < names.Length; i++)
            table.Rows.Add(new object[] { names[i] });
        string Moblie_Number = string.Empty;
        string respString = string.Empty;
        string filepath = @"C:\PDFFILES\DO";
        if (!System.IO.Directory.Exists(filepath))
        {
            System.IO.Directory.CreateDirectory(filepath);
        }
        string DoDate = DateTime.Now.ToString();
        string DDate = DoDate.Trim();
        uploadedFileName = "HDO.pdf";

        string pdfname = filepath + "\\" + uploadedFileName;
        rprt1.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pdfname);
        var fileBytes = File.ReadAllBytes(pdfname);
        base64String = System.Convert.ToBase64String(fileBytes);


        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "api-call", "javascript:sendPdfToWatsapp('" + bill_no + "','" + base64String + "', '" + uploadedFileName + "','" + instanceid + "','LATASOFTWARE','" + mobileNumber + "');", true);


        #endregion
    }
}

    