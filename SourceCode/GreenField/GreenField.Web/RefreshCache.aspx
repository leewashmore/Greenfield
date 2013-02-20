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
        onload="Chart1_Load" Width="1094px">
        <Series>
            <asp:Series Name="Series1" IsValueShownAsLabel="True">
                <Points>
                    <asp:DataPoint YValues="<%=GetDataPoint(CacheKeyNames.EntitySelectionDataCache)%>" Label="hello1"  />
                    <asp:DataPoint AxisLabel="AxisLabel1" YValues="10.0" Label="hello1"  />
                    <asp:DataPoint AxisLabel="AxisLabel2" YValues="<%=x10%>" Label="hello1"  />

                    <asp:DataPoint YValues="<%=GetDataPoint(CacheKeyNames.AvailableDatesInPortfoliosCache)%>" Label="hello1"  />
                    <asp:DataPoint YValues="<%=GetDataPoint(CacheKeyNames.CountrySelectionDataCache)%>" Label="hello1"  />
                    <asp:DataPoint YValues="<%=GetDataPoint(CacheKeyNames.RegionSelectionDataCache)%>" Label="hello1"  />

                    <asp:DataPoint YValues="<%=GetDataPoint(CacheKeyNames.FXCommodityDataCache)%>" Label="hello1"  />
                    <asp:DataPoint YValues="<%=GetDataPoint(CacheKeyNames.MarketSnapshotSelectionDataCache)%>" Label="hello1"  />
                    <asp:DataPoint YValues="<%=GetDataPoint(CacheKeyNames.LastDayOfMonthsCache)%>" Label="hello1"  />
                </Points>
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

    <div>
        &nbsp;<asp:Button ID="btnInvalidateCache" runat="server" onclick="Button1_Click" 
            Text="Button" />
    </div>
    </form>
</body>
</html>
