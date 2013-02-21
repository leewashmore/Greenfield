<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RefreshCache.aspx.cs" Inherits="GreenField.Web.RefreshCache" %>
<%@ Import Namespace="GreenField.Web.Helpers" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:chart ID="Chart1" runat="server" EnableTheming="False" Height="377px" 
        Width="1094px">
        <Series>
            <asp:Series Name="Series1" IsValueShownAsLabel="True">
            </asp:Series>
        </Series>
        
        <ChartAreas>
            <asp:ChartArea Name="ChartArea1">
                <Area3DStyle Enable3D="True" />
            </asp:ChartArea>
        </ChartAreas>
        <Titles>
            <asp:Title Name="Title1fdf">
            </asp:Title>
        </Titles>
        <Annotations>
            <asp:LineAnnotation Name="LineAnnotation1">
            </asp:LineAnnotation>
        </Annotations>
    </asp:chart>

    <div align="left">
        <asp:Button ID="btnInvalidateCache" runat="server" onclick="RefreshEntities_Click" 
            Text="Reset Entities" /> &nbsp;* includes SECURITY, BENCHMARK, INDEX, COMMODITY, and CURRENCY
            <br /><br />
        <asp:Button ID="btnResetAll" runat="server" onclick="RefreshAll_Click" 
            Text="Reset All" /> *except Entities
    </div>
    </form>
</body>
</html>
