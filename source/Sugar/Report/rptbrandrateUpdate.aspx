<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="rptbrandrateUpdate.aspx.cs" Inherits="Sugar_Report_rptbrandrateUpdate" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../print.css" rel="stylesheet" type="text/css" media="print" />
    <script type="text/javascript" src="../JS/DateValidation.js">
    </script>
    <script type="text/javascript" language="javascript">
        function sp(accode, fromdt, todt, DrCr) {
            var tn;
            window.open('rptLedger.aspx?accode=' + accode + '&fromdt=' + fromdt + '&todt=' + todt + '&DrCr=' + DrCr);    //R=Redirected  O=Original
        }
    </script>
    <script language="javascript" type="text/javascript">
        document.body.style.cursor = 'pointer';
        var oldColor = '';

        function ChangeRowColor(rowID) {
            var color = document.getElementById(rowID).style.backgroundColor;
            if (color != 'yellow')
                oldColor = color;
            if (color == 'yellow')
                document.getElementById(rowID).style.backgroundColor = oldColor;
            else document.getElementById(rowID).style.backgroundColor = 'yellow';
        }

    </script>
    <script language="javascript" type="text/javascript">
        function PrintPage() {
            var printContent = document.getElementById('<%= pnlGrid.ClientID %>');
            var printWindow = window.open("All Records", "Print Panel", 'left=50000,top=50000,width=0,height=0');

            printWindow.document.write(printContent.innerHTML);
            printWindow.document.close();
            printWindow.focus();
            printWindow.print();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <fieldset style="border-top: 1px dotted rgb(131, 127, 130); border-radius: 3px; width: 90%; margin-left: 30px; float: left; border-bottom: 0px; padding-top: 0px; padding-bottom: 10px; border-left: 0px; border-right: 0px; height: 7px;">
        <legend style="text-align: center;">
            <asp:Label ID="label1" runat="server" Text="     " Font-Names="verdana"
                ForeColor="White" Font-Bold="true" Font-Size="12px"></asp:Label></legend>
    </fieldset>
    <table width="80%" align="center" cellspacing="10">
       
     
        

         <tr>
           
         
            <td colspan="2">
                <asp:Button ID="btnGet" runat="server" CssClass="btnHelp" Text="Get Data" Width="80px"
                    CommandName="DrCr" OnCommand="Command_Click" Height="24px" />
                &nbsp;&nbsp;&nbsp;
                
           
           
         </td>
            <td colspan="2">
                <asp:Button ID="btnUpdate" runat="server" CssClass="btnHelp" Text="Update" Width="80px"
                    CommandName="Update" OnCommand="btnUpdate_Click" Height="24px" />
                &nbsp;&nbsp;&nbsp;
                
            </td>
        
        </tr>
    </table>
    <br />
    <asp:Panel ID="pnlGrid" runat="server" Height="500px" ScrollBars="Both" BorderStyle="Double"
        BackColor="White" BorderWidth="1px" BorderColor="Blue" Width="1500">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:GridView ID="grdDetail" runat="server" AutoGenerateColumns="false" OnRowCommand="grdDetail_OnRowCommand"
                    HeaderStyle-BackColor="#397CBB" HeaderStyle-ForeColor="White" HeaderStyle-Height="30px"
                    GridLines="Both" EmptyDataText="No Records found" Width="100%" CellPadding="5"
                    CellSpacing="5" Font-Bold="true" OnRowDataBound="grdDetail_RowDataBound" ForeColor="Black"
                    Font-Names="Verdana" Font-Size="12px" Style="overflow: hidden; table-layout: auto;"
                    AllowSorting="true" >
                    <Columns>
                        <asp:TemplateField HeaderText="Item_Code" SortExpression="accode">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblItemcode" Text='<%#Eval("Item_Code") %>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                       
                      
                         <asp:BoundField DataField="ItemName" HeaderText="ItemName" 
                             ItemStyle-HorizontalAlign="Left" />
                        <asp:TemplateField HeaderText="Brand_Code" SortExpression="debitAmt">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblBrandcode" Text='<%#Eval("Brand_Code") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                      
                        <asp:BoundField DataField="brandName" HeaderText="brandName" 
                            ItemStyle-HorizontalAlign="Left" />
                         
                    
                        <asp:BoundField DataField="qty" HeaderText="qty" 
                             ItemStyle-HorizontalAlign="Right" />
                         <asp:BoundField DataField="Wt_Per" HeaderText="Wt_Per" 
                            ItemStyle-HorizontalAlign="Right" />
                      
                         <asp:TemplateField HeaderText="Rate" SortExpression="Rate">
                            <ItemTemplate>
                                <asp:TextBox runat="server" ID="txtRate" Text='<%#Eval("Rate") %>'>
                                </asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                       

                        <%--  <asp:BoundField DataField="" HeaderText="Status" NullDisplayText="N" ControlStyle-Width="10px"
                            ItemStyle-HorizontalAlign="Left" />--%>
                    </Columns>
                    <RowStyle Height="25px" Wrap="false" ForeColor="Black" />
                </asp:GridView>
            </ContentTemplate>
           
        </asp:UpdatePanel>
    </asp:Panel>
</asp:Content>


