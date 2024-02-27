<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="pgeHamalimaster.aspx.cs" Inherits="Sugar_Master_pgeHamalimaster" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table width="90%" align="left">
 
        </tr>
        <tr>
            <td align="center">Hamali 1-10 Kgs:
                 <asp:TextBox ID="txtStage1" runat="Server" CssClass="txt" TabIndex="2" Width="80px"
                     Style="text-align: right;" AutoPostBack="false"
                     Height="24px"></asp:TextBox>
            </td>
            <tr>
                <td align="center">Hamali Above 10 to 30 kg
                 <asp:TextBox ID="txtStage2" runat="Server" CssClass="txt" TabIndex="2" Width="80px"
                     Style="text-align: right;" AutoPostBack="false"
                     Height="24px"></asp:TextBox>
                </td>
                <tr>
                    <td align="center">Hamali Above 30  Kgs:
                 <asp:TextBox ID="txtStage3" runat="Server" CssClass="txt" TabIndex="2" Width="80px"
                     Style="text-align: right;" AutoPostBack="false"
                     Height="24px"></asp:TextBox>
                    </td>

                </tr>
                  <td align="center">
                                <asp:Button ID="btnUpdate" runat="server" Text="Update" ToolTip="Update" CssClass="btnHelp"
                                    OnClick="btnUpdate_Click" Width="90px" Height="24px" />
                      </td>
                </tr>

    </table>
</asp:Content>

