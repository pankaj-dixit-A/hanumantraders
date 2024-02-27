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
//using System.Printing;
using System.Net;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Net.Mime;
using iTextSharp.tool.xml;
using System.Web.UI.HtmlControls;
using System.Net.Mail;
using System.Data.SqlClient;
using System.Xml.Linq;

public partial class Sugar_Report_pgeItemwiseSale : System.Web.UI.Page
{
    string AL1 = string.Empty;
    string AL2 = string.Empty;
    string AL3 = string.Empty;
    string AL4 = string.Empty;
    string other = string.Empty;
    string bankdetail = string.Empty;

    string podetail = string.Empty;
    string no = string.Empty;
    string grade = string.Empty;

    string grade1 = "";
    string deliverytype = string.Empty;
    string salerate = string.Empty;
    string salerate1 = string.Empty;
    string commission = string.Empty;
    string Sale_Rate = string.Empty;
    string Sale_Rate1 = "";

    string billto = string.Empty;
    int company_code;
    int year_code;
    string closingbalance = "";
    string FromDt = string.Empty;
    string ToDt = string.Empty;
    string ac_code;
    string utr_no;
    string salebillparty;
    string AcType = string.Empty;
    string mail = string.Empty;
    string mailnew = string.Empty;
    string doc_no = string.Empty;
    string season = string.Empty;
    string FssaiNO = string.Empty;
    string TinNo = string.Empty;
    string GSTNO = string.Empty;
    string panno = string.Empty;
    string eway = string.Empty;
    string Inv = string.Empty;
    string sbno = string.Empty;
    string lorryno = string.Empty;
    string cornumber = string.Empty;
    string purno = string.Empty;
    string chkwaybillno = string.Empty;
    string transoprt;
    string millinvoice = string.Empty;
    string netqntl = string.Empty;
    string dono = string.Empty;

    string cornumberChecking = string.Empty;
    ReportDocument rprt1 = new ReportDocument();
    ReportDocument rprt2 = new ReportDocument();
    ReportDocument rpt = new ReportDocument();
    string company_name = string.Empty;

    string singleitem = "1";
    string millname = string.Empty;
    string partyname = string.Empty;
    string fromdt = "";
    string todt = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
         
            
            fromdt = Request.QueryString["fromdt"];
            todt = Request.QueryString["todt"];
            fromdt = DateTime.Parse(fromdt, System.Globalization.CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyy-MM-dd");
            todt = DateTime.Parse(todt, System.Globalization.CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyy-MM-dd");
            company_name = Session["Company_Name"].ToString();
        



            DataTable dt = GetData();
            
            SqlDataAdapter da = new SqlDataAdapter();
            rpt.Load(Server.MapPath("cryItemwisesale.rpt"));
            rpt.SetDataSource(dt);

            string docno = doc_no;
            


            cryfreightbill.ReportSource = rpt;
            rpt.DataDefinition.FormulaFields["companyname"].Text = "\"" + company_name + "\"";
         
            rpt.DataDefinition.FormulaFields["pagehead"].Text = "\"" + AL1 + "\"";
 
            cryfreightbill.RefreshReport();
        }
        catch (Exception)
        {

            throw;
        }

    }
    private DataTable GetData()
    {
        try
        {
            string sql = "select * from qryGrainSaleDetail where Doc_Date between '" + fromdt + "' and '" + todt    + "' and  Company_Code=" + Convert.ToInt32(Session["Company_Code"].ToString());
            DataTable dt = new DataTable();
            string strcon = System.Configuration.ConfigurationManager.ConnectionStrings["sqlconnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(strcon))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);

                singleitem = dt.Rows.Count.ToString();
            }

            

            return dt;






        }
        catch
        {
            return null;
        }
    }


    private string GetDefaultPrinter()
    {
        PrinterSettings settings = new PrinterSettings();

        foreach (string printer in PrinterSettings.InstalledPrinters)
        {
            settings.PrinterName = printer;
            if (settings.IsDefaultPrinter)
            {
                return printer;
            }
        }
        return string.Empty;
    }

    protected void btnPDF_Click(object sender, EventArgs e)
    {
        try
        {
            // string filepath=@"D:\pdffiles\cryChequePrinting.pdf";
            string filepath = @"D:\pdffiles";
            if (!System.IO.Directory.Exists(filepath))
            {
                System.IO.Directory.CreateDirectory("D:\\pdffiles");
            }
            string pdfname = filepath + "\\Salebill" + sbno + "-" + lorryno + ".pdf";
            rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pdfname);

            //open PDF File
            //System.Diagnostics.Process.Start(pdfname);
            // string FilePath = Server.MapPath("javascript1-sample.pdf");

            WebClient User = new WebClient();

            Byte[] FileBuffer = User.DownloadData(pdfname);

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
            string filepath = @"D:\pdffiles";
            if (!System.IO.Directory.Exists(filepath))
            {
                System.IO.Directory.CreateDirectory("D:\\pdffiles");
            }
            string pdfname = filepath + "\\SaleBillNO" + sbno + "" + lorryno + ".pdf";

            rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pdfname);

            if (txtEmail.Text != string.Empty && txtEmail.Text != "0")
            {
                //string fileName = "Saudapending.pdf";
                //string filepath1 = "~/PAN/" + fileName;

                mail = txtEmail.Text;

                if (lorryno != string.Empty)
                {
                    lorryno = "Lorry:" + lorryno;
                }
                if (netqntl != string.Empty)
                {
                    netqntl = "NETQNTL:" + netqntl;
                }
                if (partyname != string.Empty)
                {
                    partyname = "billtoname:" + partyname;
                }
                if (dono != string.Empty)
                {
                    dono = "DO_No:" + dono;
                }


                ContentType contentType = new ContentType();
                contentType.MediaType = MediaTypeNames.Application.Pdf;
                contentType.Name = "Salebill_DocNo:" + sbno + "-" + lorryno;
                Attachment attachment = new Attachment(pdfname, contentType);

                string mailFrom = Session["EmailId"].ToString();
                string smtpPort = "587";
                string emailPassword = Session["EmailPassword"].ToString();
                MailMessage msg = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
                SmtpServer.Host = clsGV.Email_Address;
                msg.From = new MailAddress(mailFrom);
                msg.To.Add(mail);
                msg.Body = "Salebill";
                msg.Attachments.Add(attachment);
                msg.IsBodyHtml = true;
                //msg.Subject = "DOC.No:";
                msg.Subject = "Bill No:" + sbno + " " + lorryno + " " + netqntl + " " + partyname + " " + dono;

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
                if (File.Exists(pdfname))
                {
                    File.Delete(pdfname);
                }
                Response.Write("<script>alert('Mail Send successfully');</script>");
            }
            else
            {
                Response.Write("<script>alert('Enter Email Id');</script>");
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
        this.cryfreightbill.ReportSource = null;

        cryfreightbill.Dispose();

        if (rpt != null)
        {

            rpt.Close();

            rpt.Dispose();

            rpt = null;

        }

        GC.Collect();

        GC.WaitForPendingFinalizers();
    }
}