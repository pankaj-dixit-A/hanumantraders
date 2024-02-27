<%@ Page Language="C#" AutoEventWireup="true" CodeFile="rptLedgerCrystal.aspx.cs" Inherits="Foundman_Report_rptLedgerCrystal"%>

<%@ Register Assembly="CrystalDecisions.Web,Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
      <title></title>
     <script src="../Scripts/whatsapp-api.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/1.2.61/jspdf.min.js"></script>
    <script type="text/javascript" src="//cdnjs.cloudflare.com/ajax/libs/jquery/2.1.3/jquery.min.js"></script>
  
    <script type="text/javascript">
        function sendPdfToWatsapp(bill_no, content, fileName, instanceid, authKey, mobtxt) {
            debugger;
            // var Opening_Bal =
            var string = mobtxt;
            // var string = document.getElementById("")
            //var string =  $("#<%=txtWhatsapp.ClientID %>").val() == "" ? 0 : $("#<%=txtWhatsapp.ClientID %>").val();
            var stringArray = (new Function("return [" + string + "];")());
            var strcountarry = stringArray.length;
            debugger;
            for (var i = 0; i < strcountarry; i++) {
                var filenamenew = stringArray[i] + fileName;
                var message = 'PDF';
                debugger;
                whatsappApi(bill_no, content, filenamenew, instanceid, stringArray[i], authKey);
            }
        }
   </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="printReady">
     <asp:Button ID="btnExcel" runat="server" Text="Open Excel" Width="80px" OnClientClick="return CheckEmail();"
            OnClick="btnExcel_Click" />
        <asp:Button ID="btnPDF" runat="server" Text="Open PDF" Width="80px" OnClientClick="return CheckEmail();"
            OnClick="btnPDF_Click" />
        <asp:Button ID="btnMail" runat="server" Text="Mail PDF" Width="80px" OnClientClick="return CheckEmail();"
            OnClick="btnMail_Click" />
        <asp:TextBox runat="server" ID="txtEmail" Width="300px"></asp:TextBox>

            Whatsapp:
             <asp:TextBox runat="server" ID="txtWhatsapp" Width="150px" placeholder="Enter MobNo" AutoPostBack="True" ></asp:TextBox>
             <asp:Button runat="server" ID="btnWhatsApp" Text="WhatsApp" CssClass="btnHelp" Height="24px"
                    Width="80px" OnClick="btnWhatsApp_Click" />

        <CR:CrystalReportViewer ID="cryLedgerCrystal" runat="server" AutoDataBind="true"
            ShowAllPageIds="True" HasPageNavigationButtons="True" BestFitPage="False" Width="1500px" />
    </div>

    </form>
</body>
</html>
