<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RefreshCache.aspx.cs" Inherits="GreenField.Web.RefreshCache" %>

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
    
            <div align="center">
                <asp:Button ID="btnRefreshGraph" runat="server" onclick="RefreshGraph_Click" 
                Text="Refresh Graph" Width="120px" />
            </div>
            <br/>
            <table>
                <tr>
                    <td>
                        <asp:Button ID="btnEntitiesCache" runat="server" onclick="RefreshEntitiesCache_Click" 
                                    Text="Reset Entities Cache" Width="200px" />
                    </td>
                    <td>* includes SECURITY, BENCHMARK, INDEX, COMMODITY, and CURRENCY</td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnBenchmarkCache" runat="server" onclick="RefreshBenchmarkCache_Click" 
                                    Text="Reset Benchmark Cache" Width="200px" />
                    </td>
                    <td>* includes Portfolio Selection Data, and Available Dates In Portfolios</td>
                </tr>
                
                <tr>
                    <td>
                        <asp:Button ID="btnModelFXCache" runat="server" onclick="RefreshModelFXCache_Click" 
                                    Text="Reset ModelFX Cache" Width="200px" /></td>
                    <td>* includes Country Selection Data, Region Selection Data, and FX Commodity Data</td>
                </tr>
                
                <tr>
                    <td>
                        <asp:Button ID="btnPerformanceCache" runat="server" onclick="RefreshPerformanceCache_Click" 
                                    Text="Reset Performance Cache" Width="200px" /></td>
                    <td>* includes Last Day Of Months</td>
                </tr>
                
                <tr>
                    <td>
                        <asp:Button ID="btnResetAll" runat="server" onclick="RefreshAll_Click" 
                                    Text="Reset All" Width="200px" /></td><td>* except Entities</td>
                </tr>
                
                <tr>
                    <td><br/><br/>
                        <asp:Button ID="Button1" runat="server" onclick="InvalidateAll_Click" 
                                    Text="Invalidate All" Width="200px" /></td><td></td>
                </tr>

            </table>
        </form>
    </body>
</html>