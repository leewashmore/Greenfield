using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.ComponentModel;
using Drawing = DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text.pdf.draw;


namespace GreenField.Web.Helpers
{
    public static class PptRead
    {
        public static ICPresentation Fetch(String location, SecurityInformation securityInformation)
        {
            ICPresentation result = null;

            using (PresentationDocument presentationDocument = PresentationDocument.Open(location, true))
            {
                result = new ICPresentation();
                PresentationPart presentatioPart = presentationDocument.PresentationPart;
                SlideId[] slideIds = presentatioPart.Presentation.SlideIdList.Elements<SlideId>().ToArray();

                string relId = (slideIds[0] as SlideId).RelationshipId;

                // Get the slide part from the relationship ID.
                SlidePart slide = (SlidePart)presentatioPart.GetPartById(relId);

                List<String> companyOverview = GetCompanyOverview(slide);

                result.CompanyOverviewInstance = new CompanyOverview
                {
                    SecurityInfo = securityInformation,
                    CompanyOverviewList = companyOverview
                };

                string relId1 = (slideIds[1] as SlideId).RelationshipId;
                // Get the slide part from the relationship ID.
                SlidePart slide1 = (SlidePart)presentatioPart.GetPartById(relId1);

                List<String> investmentThesis = GetInvestmentThesis(slide1);
                result.InvestmentThesisInstance = new InvestmentThesis { ThesisPoints = investmentThesis };

                result.InvestmentThesisInstance.HighlightedRisks = GetInvestmentRisk(slide1);

                string relId2 = (slideIds[2] as SlideId).RelationshipId;
                // Get the slide part from the relationship ID.
                SlidePart slide2 = (SlidePart)presentatioPart.GetPartById(relId2);

                List<String> keyOperatingAssumpations = GetKeyOperatingAssumpations(slide2);
                result.KeyOperatingAssumpationsInstance =
                    new KeyOperatingAssumpations { Assumptions = keyOperatingAssumpations };

                string relId3 = (slideIds[3] as SlideId).RelationshipId;
                // Get the slide part from the relationship ID.
                SlidePart slide3 = (SlidePart)presentatioPart.GetPartById(relId3);

                Dictionary<int, List<String>> vqg = GetVQG(slide3);
                result.VQGInstance = new VQG();
                foreach (KeyValuePair<int, List<String>> kvp in vqg)
                {
                    if (kvp.Key == 0)
                    {
                        result.VQGInstance.Value = kvp.Value;
                    }
                    if (kvp.Key == 1)
                    {
                        result.VQGInstance.Growth = kvp.Value;
                    }
                    if (kvp.Key == 2)
                    {
                        result.VQGInstance.Quality = kvp.Value;
                    }
                }

                string relId4 = (slideIds[4] as SlideId).RelationshipId;
                // Get the slide part from the relationship ID.
                SlidePart slide4 = (SlidePart)presentatioPart.GetPartById(relId4);

                Dictionary<int, List<String>> swotAnalysis = GetSWOTAnalysis(slide4);
                result.SWOTAnalysisInstance = new SWOTAnalysis();
                foreach (KeyValuePair<int, List<String>> kvp in swotAnalysis)
                {
                    if (kvp.Key == 0)
                    {
                        result.SWOTAnalysisInstance.Opportunity = kvp.Value;
                    }
                    if (kvp.Key == 1)
                    {
                        result.SWOTAnalysisInstance.Strength = kvp.Value;
                    }
                    if (kvp.Key == 2)
                    {
                        result.SWOTAnalysisInstance.Weakness = kvp.Value;
                    }
                    if (kvp.Key == 3)
                    {
                        result.SWOTAnalysisInstance.Threat = kvp.Value;
                    }

                }                
            }

            return result;
        }

        public static void GeneratePresentationPdf(string outFile, ICPresentation presentation)
        {
            float listItemSpacing = 15F;
            string strLogoPath = @"C:\Users\rvig\Pictures\AshmoreEMMLogo.png";
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(strLogoPath);

            Document doc = new Document(PageSize.A4, 10F, 10F, 10F, 10F);
            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            PdfWriter PDFWriter = PdfWriter.GetInstance(doc, new FileStream(outFile, FileMode.Create));
            PdfFooter PageEventHandler = new PdfFooter();
            PDFWriter.PageEvent = PageEventHandler;
            PageEventHandler.FooterDate = DateTime.UtcNow.ToString("MMMM dd, yyyy HH:mm tt UTC");

            doc.Open();
            Rectangle page = doc.PageSize;


            #region CompanyOverviewInstance
            #region Header
            PdfPTable coiHeaderTable = new PdfPTable(2);
            coiHeaderTable.WidthPercentage = 100;
            coiHeaderTable.SpacingBefore = 10;

            coiHeaderTable.SetWidths(new float[] { 5, 1 });
            PdfPCell coiHeaderContentCell = new PdfPCell(new Phrase
                (String.Format("{0} - {1}", presentation.CompanyOverviewInstance.SecurityInfo.SecurityName
                    , presentation.CompanyOverviewInstance.SecurityInfo.Recommendation)
                , PDFFontStyle.STYLE_11));
            coiHeaderContentCell.PaddingTop = 10;
            coiHeaderContentCell.PaddingBottom = 10;
            AddTextCell(coiHeaderTable, coiHeaderContentCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);
            PdfPCell coiHeaderImageCell = new PdfPCell(image, false);
            coiHeaderImageCell.PaddingTop = 10;
            coiHeaderImageCell.PaddingBottom = 10;
            AddTextCell(coiHeaderTable, coiHeaderImageCell, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);
            doc.Add(coiHeaderTable);
            #endregion

            #region Security Information
            PdfPTable coiSecurityInfoTable = new PdfPTable(4);
            coiSecurityInfoTable.WidthPercentage = 100;
            coiSecurityInfoTable.SpacingBefore = 10;
            coiSecurityInfoTable.SetWidths(new float[] { 1, 1, 1, 1 });

            PdfPCell coiSecurityInfoAnalystCell = new PdfPCell(new Phrase("Analyst:", PDFFontStyle.STYLE_10)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoAnalystCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_TOP);
            PdfPCell coiSecurityInfoAnalystValueCell = new PdfPCell(new Phrase(presentation.CompanyOverviewInstance.SecurityInfo.Analyst, PDFFontStyle.STYLE_8)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoAnalystValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.TOP);
            PdfPCell coiSecurityInfoCurrentHoldingCell = new PdfPCell(new Phrase("Current Holdings:", PDFFontStyle.STYLE_10)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoCurrentHoldingCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.TOP);
            PdfPCell coiSecurityInfoCurrentHoldingValueCell = new PdfPCell(new Phrase(presentation.CompanyOverviewInstance.SecurityInfo.CurrentHoldings, PDFFontStyle.STYLE_8)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoCurrentHoldingValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.RIGHT_TOP);

            PdfPCell coiSecurityInfoCountryCell = new PdfPCell(new Phrase("Country:", PDFFontStyle.STYLE_10)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoCountryCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.LEFT);
            PdfPCell coiSecurityInfoCountryValueCell = new PdfPCell(new Phrase(presentation.CompanyOverviewInstance.SecurityInfo.Country, PDFFontStyle.STYLE_8)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoCountryValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.NONE);
            PdfPCell coiSecurityInfoNAVCell = new PdfPCell(new Phrase("% of NAV:", PDFFontStyle.STYLE_10)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoNAVCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.NONE);
            PdfPCell coiSecurityInfoNAVValueCell = new PdfPCell(new Phrase(presentation.CompanyOverviewInstance.SecurityInfo.NAV, PDFFontStyle.STYLE_8)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoNAVValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.RIGHT);

            PdfPCell coiSecurityInfoIndustryCell = new PdfPCell(new Phrase("Industry:", PDFFontStyle.STYLE_10)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoIndustryCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.LEFT);
            PdfPCell coiSecurityInfoIndustryValueCell = new PdfPCell(new Phrase(presentation.CompanyOverviewInstance.SecurityInfo.Industry, PDFFontStyle.STYLE_8)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoIndustryValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.NONE);
            PdfPCell coiSecurityInfoBMCell = new PdfPCell(new Phrase("BM Weight:", PDFFontStyle.STYLE_10)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoBMCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.NONE);
            PdfPCell coiSecurityInfoBMValueCell = new PdfPCell(new Phrase(presentation.CompanyOverviewInstance.SecurityInfo.BenchmarkWeight, PDFFontStyle.STYLE_8)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoBMValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.RIGHT);

            PdfPCell coiSecurityInfoMktCapCell = new PdfPCell(new Phrase("Mkt Cap:", PDFFontStyle.STYLE_10)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoMktCapCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.LEFT);
            PdfPCell coiSecurityInfoMktCapValueCell = new PdfPCell(new Phrase(presentation.CompanyOverviewInstance.SecurityInfo.MktCap, PDFFontStyle.STYLE_8)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoMktCapValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.NONE);
            PdfPCell coiSecurityInfoActiveWeightCell = new PdfPCell(new Phrase("Active Weight:", PDFFontStyle.STYLE_10)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoActiveWeightCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.NONE);
            PdfPCell coiSecurityInfoActiveWeightValueCell = new PdfPCell(new Phrase(presentation.CompanyOverviewInstance.SecurityInfo.ActiveWeight, PDFFontStyle.STYLE_8)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoActiveWeightValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.RIGHT);

            PdfPCell coiSecurityInfoPriceCell = new PdfPCell(new Phrase("Price:", PDFFontStyle.STYLE_10)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoPriceCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.LEFT);
            PdfPCell coiSecurityInfoPriceValueCell = new PdfPCell(new Phrase(presentation.CompanyOverviewInstance.SecurityInfo.Price, PDFFontStyle.STYLE_8)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoPriceValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.NONE);
            PdfPCell coiSecurityInfoRetAbsoluteCell = new PdfPCell(new Phrase("12m Ret - Absolute:", PDFFontStyle.STYLE_10)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoRetAbsoluteCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.NONE);
            PdfPCell coiSecurityInfoRetAbsoluteValueCell = new PdfPCell(new Phrase(presentation.CompanyOverviewInstance.SecurityInfo.RetAbsolute, PDFFontStyle.STYLE_8)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoRetAbsoluteValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.RIGHT);

            PdfPCell coiSecurityInfoFVCalcCell = new PdfPCell(new Phrase("FV Calc:", PDFFontStyle.STYLE_10)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoFVCalcCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.LEFT);
            PdfPCell coiSecurityInfoFVCalcValueCell = new PdfPCell(new Phrase(presentation.CompanyOverviewInstance.SecurityInfo.FVCalc, PDFFontStyle.STYLE_8)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoFVCalcValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.NONE);
            PdfPCell coiSecurityInfoRetLocCell = new PdfPCell(new Phrase("12m Ret - Rel to loc:", PDFFontStyle.STYLE_10)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoRetLocCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.NONE);
            PdfPCell coiSecurityInfoRetLocValueCell = new PdfPCell(new Phrase(presentation.CompanyOverviewInstance.SecurityInfo.RetLoc, PDFFontStyle.STYLE_8)) { Padding = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoRetLocValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.RIGHT);

            PdfPCell coiSecurityInfoBSRCell = new PdfPCell(new Phrase("Buy/Sell vs Crnt:", PDFFontStyle.STYLE_10)) { PaddingLeft = 5, PaddingRight = 5, PaddingBottom = 10, PaddingTop = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoBSRCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_BOTTOM);
            PdfPCell coiSecurityInfoBSRValueCell = new PdfPCell(new Phrase(presentation.CompanyOverviewInstance.SecurityInfo.BSR, PDFFontStyle.STYLE_8)) { PaddingLeft = 5, PaddingRight = 5, PaddingBottom = 10, PaddingTop = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoBSRValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);
            PdfPCell coiSecurityInfoRetEMVCell = new PdfPCell(new Phrase("12m Ret - Rel to EM:", PDFFontStyle.STYLE_10)) { PaddingLeft = 5, PaddingRight = 5, PaddingBottom = 10, PaddingTop = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoRetEMVCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);
            PdfPCell coiSecurityInfoRetEMVValueCell = new PdfPCell(new Phrase(presentation.CompanyOverviewInstance.SecurityInfo.RetEMV, PDFFontStyle.STYLE_8)) { PaddingLeft = 5, PaddingRight = 5, PaddingBottom = 10, PaddingTop = 5 };
            AddTextCell(coiSecurityInfoTable, coiSecurityInfoRetEMVValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.RIGHT_BOTTOM);

            doc.Add(coiSecurityInfoTable);
            #endregion

            #region Company Overview
            PdfPTable coiCompanyOverViewTable = new PdfPTable(1);
            coiCompanyOverViewTable.WidthPercentage = 100;
            coiCompanyOverViewTable.SpacingBefore = 10;

            Paragraph coiCompanyOverviewHeader = new Paragraph();
            coiCompanyOverviewHeader.SpacingBefore = 10;
            coiCompanyOverviewHeader.Add(new Phrase("Company Overview", PDFFontStyle.STYLE_10));

            ZapfDingbatsList coiCompanyOverviewList = new ZapfDingbatsList(110, 10);
            coiCompanyOverviewList.IndentationLeft = 2;
            coiCompanyOverviewList.Autoindent = true;
            coiCompanyOverviewList.Symbol.Font.Size = PDFFontStyle.STYLE_8.Size;
            coiCompanyOverviewList.Symbol.Font.Color = BaseColor.BLUE;

            foreach (String overviewDetail in presentation.CompanyOverviewInstance.CompanyOverviewList)
            {
                ListItem item = new ListItem(new Phrase(overviewDetail, PDFFontStyle.STYLE_8));

                if (presentation.CompanyOverviewInstance.CompanyOverviewList.IndexOf(overviewDetail) == 0)
                {
                    item.SpacingBefore = listItemSpacing;
                }

                if (presentation.CompanyOverviewInstance.CompanyOverviewList.IndexOf(overviewDetail) !=
                    presentation.CompanyOverviewInstance.CompanyOverviewList.Count - 1)
                {
                    item.SpacingAfter = listItemSpacing;
                }
                coiCompanyOverviewList.Add(item);
            }

            coiCompanyOverviewHeader.Add(coiCompanyOverviewList);
            PdfPCell coiCompanyOverviewCell = new PdfPCell();
            coiCompanyOverviewCell.BorderWidth = 1;
            coiCompanyOverviewCell.PaddingBottom = listItemSpacing;
            coiCompanyOverviewCell.Border = PDFBorderType.LEFT_RIGHT_TOP_BOTTOM;
            coiCompanyOverviewCell.AddElement(coiCompanyOverviewHeader);
            coiCompanyOverviewCell.MinimumHeight = doc.PageSize.Height - (2 * doc.TopMargin) - coiSecurityInfoTable.TotalHeight - 110;
            coiCompanyOverViewTable.AddCell(coiCompanyOverviewCell);
            doc.Add(coiCompanyOverViewTable);
            #endregion

            //doc.Add(new LineSeparator() { Offset = -10 });

            #endregion

            #region InvestmentThesisInstance
            doc.NewPage();

            #region Header
            PdfPTable itiHeaderTable = new PdfPTable(2);
            itiHeaderTable.WidthPercentage = 100;
            itiHeaderTable.SpacingBefore = 10;

            coiHeaderTable.SetWidths(new float[] { 5, 1 });
            PdfPCell itiHeaderContentCell = new PdfPCell(new Phrase("Investment Thesis", PDFFontStyle.STYLE_11));
            itiHeaderContentCell.PaddingTop = 10;
            itiHeaderContentCell.PaddingBottom = 10;
            AddTextCell(itiHeaderTable, itiHeaderContentCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);
            PdfPCell itiHeaderImageCell = new PdfPCell(image, false);
            itiHeaderImageCell.PaddingTop = 10;
            itiHeaderImageCell.PaddingBottom = 10;
            AddTextCell(itiHeaderTable, itiHeaderImageCell, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);
            doc.Add(itiHeaderTable);
            #endregion

            #region Investment Thesis Content
            PdfPTable itiInvestmentThesisContentTable = new PdfPTable(1);
            itiInvestmentThesisContentTable.WidthPercentage = 100;
            ZapfDingbatsList itiInvestmentThesisContentList = new ZapfDingbatsList(110, 10);
            itiInvestmentThesisContentList.IndentationLeft = 2;
            itiInvestmentThesisContentList.Autoindent = true;
            itiInvestmentThesisContentList.Symbol.Font.Size = PDFFontStyle.STYLE_8.Size;
            itiInvestmentThesisContentList.Symbol.Font.Color = BaseColor.BLUE;

            foreach (String thesisPoint in presentation.InvestmentThesisInstance.ThesisPoints)
            {
                ListItem item = new ListItem(new Phrase(thesisPoint, PDFFontStyle.STYLE_8));
                if (presentation.InvestmentThesisInstance.ThesisPoints.IndexOf(thesisPoint) == 0)
                {
                    item.SpacingBefore = listItemSpacing;
                }

                if (presentation.InvestmentThesisInstance.ThesisPoints.IndexOf(thesisPoint) !=
                    presentation.InvestmentThesisInstance.ThesisPoints.Count - 1)
                {
                    item.SpacingAfter = listItemSpacing;
                }
                itiInvestmentThesisContentList.Add(item);
            }
            PdfPCell itiInvestmentThesisContentCell = new PdfPCell();
            itiInvestmentThesisContentCell.AddElement(itiInvestmentThesisContentList);
            itiInvestmentThesisContentCell.MinimumHeight = (doc.PageSize.Height - itiHeaderTable.TotalHeight - 100) * Convert.ToSingle(0.75);
            itiInvestmentThesisContentCell.Border = PDFBorderType.NONE;
            itiInvestmentThesisContentTable.AddCell(itiInvestmentThesisContentCell);
            doc.Add(itiInvestmentThesisContentTable);
            #endregion

            #region Risk
            PdfPTable itiRiskTable = new PdfPTable(1);
            itiRiskTable.WidthPercentage = 100;
            itiRiskTable.SpacingBefore = 10;

            PdfPCell itiRiskHeaderCell = new PdfPCell(new Phrase("Risks to Investment Thesis – What could go wrong?", PDFFontStyle.STYLE_10));
            itiRiskHeaderCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            AddTextCell(itiRiskTable, itiRiskHeaderCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_TOP);

            List itiRiskList = new List(List.UNORDERED, 10);
            itiRiskList.IndentationLeft = 2;
            itiRiskList.Autoindent = true;
            itiRiskList.Symbol.Font.Size = PDFFontStyle.STYLE_8.Size;
            itiRiskList.Symbol.Font.Color = BaseColor.BLACK;
            foreach (String risk in presentation.InvestmentThesisInstance.HighlightedRisks)
            {
                ListItem item = new ListItem(new Phrase(risk, PDFFontStyle.STYLE_8));
                if (presentation.InvestmentThesisInstance.HighlightedRisks.IndexOf(risk) !=
                    presentation.InvestmentThesisInstance.HighlightedRisks.Count - 1)
                {
                    item.SpacingAfter = 5F;
                }

                itiRiskList.Add(item);
            }

            PdfPCell itiRiskContentCell = new PdfPCell();
            itiRiskContentCell.AddElement(itiRiskList);
            itiRiskContentCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            itiRiskContentCell.PaddingTop = 5;
            itiRiskContentCell.PaddingBottom = 5;
            AddTextCell(itiRiskTable, itiRiskContentCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_BOTTOM);

            doc.Add(itiRiskTable);
            #endregion

            //doc.Add(new LineSeparator() { Offset = -10 });
            #endregion

            #region KeyOperatingAssumpationsInstance
            doc.NewPage();

            #region Header
            PdfPTable koaiHeaderTable = new PdfPTable(2);
            koaiHeaderTable.WidthPercentage = 100;
            koaiHeaderTable.SpacingBefore = 10;
            koaiHeaderTable.SetWidths(new float[] { 5, 1 });

            PdfPCell koaiHeaderContentCell = new PdfPCell(new Phrase("Key Operating Assumptions", PDFFontStyle.STYLE_11));
            koaiHeaderContentCell.PaddingTop = 10;
            koaiHeaderContentCell.PaddingBottom = 10;
            AddTextCell(koaiHeaderTable, koaiHeaderContentCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);
            PdfPCell koaiHeaderImageCell = new PdfPCell(image, false);
            koaiHeaderImageCell.PaddingTop = 10;
            koaiHeaderImageCell.PaddingBottom = 10;
            AddTextCell(koaiHeaderTable, koaiHeaderImageCell, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);
            doc.Add(koaiHeaderTable);
            #endregion

            #region Key Operating Assumpations Content
            ZapfDingbatsList koaiInvestmentThesisContentList = new ZapfDingbatsList(110, 10);
            koaiInvestmentThesisContentList.IndentationLeft = 2;
            koaiInvestmentThesisContentList.Autoindent = true;
            koaiInvestmentThesisContentList.Symbol.Font.Size = PDFFontStyle.STYLE_8.Size;
            koaiInvestmentThesisContentList.Symbol.Font.Color = BaseColor.BLUE;

            foreach (String assumption in presentation.KeyOperatingAssumpationsInstance.Assumptions)
            {
                ListItem item = new ListItem(new Phrase(assumption, PDFFontStyle.STYLE_8));
                if (presentation.KeyOperatingAssumpationsInstance.Assumptions.IndexOf(assumption) == 0)
                {
                    item.SpacingBefore = listItemSpacing;
                }

                if (presentation.KeyOperatingAssumpationsInstance.Assumptions.IndexOf(assumption) !=
                    presentation.KeyOperatingAssumpationsInstance.Assumptions.Count - 1)
                {
                    item.SpacingAfter = listItemSpacing;
                }
                koaiInvestmentThesisContentList.Add(item);
            }
            doc.Add(koaiInvestmentThesisContentList);
            #endregion

            //doc.Add(new LineSeparator() { Offset = -10 });
            #endregion

            #region VQGInstance
            doc.NewPage();

            #region Header
            PdfPTable vqgiHeaderTable = new PdfPTable(2);
            vqgiHeaderTable.WidthPercentage = 100;
            vqgiHeaderTable.SpacingBefore = 10;
            vqgiHeaderTable.SetWidths(new float[] { 5, 1 });

            PdfPCell vqgiHeaderContentCell = new PdfPCell(new Phrase("Value, Growth, Quality", PDFFontStyle.STYLE_11));
            vqgiHeaderContentCell.PaddingTop = 10;
            vqgiHeaderContentCell.PaddingBottom = 10;
            AddTextCell(vqgiHeaderTable, vqgiHeaderContentCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);
            PdfPCell vqgiHeaderImageCell = new PdfPCell(image, false);
            vqgiHeaderImageCell.PaddingTop = 10;
            vqgiHeaderImageCell.PaddingBottom = 10;
            AddTextCell(vqgiHeaderTable, vqgiHeaderImageCell, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);
            doc.Add(vqgiHeaderTable);
            #endregion

            #region VQG Content
            #region Value
            PdfPTable vqgiValueTable = new PdfPTable(2);

            vqgiValueTable.WidthPercentage = 100;
            vqgiValueTable.SpacingBefore = 5;
            vqgiValueTable.SetWidths(new float[] { 1, 25 });

            PdfPCell vqgiValueCell = new PdfPCell(new Phrase("Value", PDFFontStyle.STYLE_9));
            vqgiValueCell.MinimumHeight = (doc.PageSize.Height - (2 * doc.TopMargin) - vqgiHeaderTable.TotalHeight - 60) / 3;
            vqgiValueCell.BackgroundColor = BaseColor.GRAY;
            vqgiValueCell.Rotation = 90;
            AddTextCell(vqgiValueTable, vqgiValueCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.NONE);

            PdfPCell vqgiValueContentCell = new PdfPCell();
            ZapfDingbatsList vqgiValueContentList = new ZapfDingbatsList(110, 10);
            vqgiValueContentList.IndentationLeft = 2;
            vqgiValueContentList.Symbol.Font.Size = 10F;
            vqgiValueContentList.Symbol.Font.Color = BaseColor.BLUE;

            foreach (String value in presentation.VQGInstance.Value)
            {
                ListItem item = new ListItem(new Phrase(value, PDFFontStyle.STYLE_8));
                if (presentation.VQGInstance.Value.IndexOf(value) == 0)
                {
                    item.SpacingBefore = listItemSpacing;
                }

                if (presentation.VQGInstance.Value.IndexOf(value) !=
                    presentation.VQGInstance.Value.Count - 1)
                {
                    item.SpacingAfter = listItemSpacing;
                }
                vqgiValueContentList.Add(item);
            }
            vqgiValueContentCell.AddElement(vqgiValueContentList);
            AddTextCell(vqgiValueTable, vqgiValueContentCell, Element.ALIGN_LEFT, Element.ALIGN_TOP, PDFBorderType.NONE);

            PdfPCell vqgiValueDashedCell = new PdfPCell();
            vqgiValueDashedCell.AddElement(new DottedLineSeparator());
            AddTextCell(vqgiValueTable, new PdfPCell(), Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.NONE);
            AddTextCell(vqgiValueTable, vqgiValueDashedCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.NONE);

            doc.Add(vqgiValueTable);
            #endregion

            #region Growth
            PdfPTable vqgiGrowthTable = new PdfPTable(2);
            vqgiGrowthTable.WidthPercentage = 100;
            vqgiGrowthTable.SpacingBefore = 5;
            vqgiGrowthTable.SetWidths(new float[] { 1, 25 });

            PdfPCell vqgiGrowthCell = new PdfPCell(new Phrase("Growth", PDFFontStyle.STYLE_9));
            vqgiGrowthCell.MinimumHeight = (doc.PageSize.Height - (2 * doc.TopMargin) - vqgiHeaderTable.TotalHeight - 60) / 3;
            vqgiGrowthCell.BackgroundColor = BaseColor.GRAY;
            vqgiGrowthCell.Rotation = 90;
            AddTextCell(vqgiGrowthTable, vqgiGrowthCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.NONE);

            PdfPCell vqgiGrowthContentCell = new PdfPCell();
            ZapfDingbatsList vqgiGrowthContentList = new ZapfDingbatsList(110, 10);
            vqgiGrowthContentList.IndentationLeft = 2;
            vqgiGrowthContentList.Symbol.Font.Size = 10F;
            vqgiGrowthContentList.Symbol.Font.Color = BaseColor.BLUE;

            foreach (String value in presentation.VQGInstance.Growth)
            {
                ListItem item = new ListItem(new Phrase(value, PDFFontStyle.STYLE_8));
                if (presentation.VQGInstance.Growth.IndexOf(value) == 0)
                {
                    item.SpacingBefore = listItemSpacing;
                }

                if (presentation.VQGInstance.Growth.IndexOf(value) !=
                    presentation.VQGInstance.Growth.Count - 1)
                {
                    item.SpacingAfter = listItemSpacing;
                }
                vqgiGrowthContentList.Add(item);
            }
            vqgiGrowthContentCell.AddElement(vqgiGrowthContentList);
            AddTextCell(vqgiGrowthTable, vqgiGrowthContentCell, Element.ALIGN_LEFT, Element.ALIGN_TOP, PDFBorderType.NONE);

            PdfPCell vqgiGrowthDashedCell = new PdfPCell();
            vqgiGrowthDashedCell.AddElement(new DottedLineSeparator());
            AddTextCell(vqgiGrowthTable, new PdfPCell(), Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.NONE);
            AddTextCell(vqgiGrowthTable, vqgiGrowthDashedCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.NONE);

            doc.Add(vqgiGrowthTable);
            #endregion

            #region Quality
            PdfPTable vqgiQualityTable = new PdfPTable(2);
            vqgiQualityTable.WidthPercentage = 100;
            vqgiQualityTable.SpacingBefore = 5;
            vqgiQualityTable.SetWidths(new float[] { 1, 25 });

            PdfPCell vqgiQualityCell = new PdfPCell(new Phrase("Quality", PDFFontStyle.STYLE_9));
            vqgiQualityCell.MinimumHeight = (doc.PageSize.Height - (2 * doc.TopMargin) - vqgiHeaderTable.TotalHeight - 60) / 3;
            vqgiQualityCell.BackgroundColor = BaseColor.GRAY;
            vqgiQualityCell.Rotation = 90;
            AddTextCell(vqgiQualityTable, vqgiQualityCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.NONE);

            PdfPCell vqgiQualityContentCell = new PdfPCell();
            ZapfDingbatsList vqgiQualityContentList = new ZapfDingbatsList(110, 10);
            vqgiQualityContentList.IndentationLeft = 2;
            vqgiQualityContentList.Symbol.Font.Size = 10F;
            vqgiQualityContentList.Symbol.Font.Color = BaseColor.BLUE;

            foreach (String value in presentation.VQGInstance.Quality)
            {
                ListItem item = new ListItem(new Phrase(value, PDFFontStyle.STYLE_8));
                if (presentation.VQGInstance.Quality.IndexOf(value) == 0)
                {
                    item.SpacingBefore = listItemSpacing;
                }

                if (presentation.VQGInstance.Quality.IndexOf(value) !=
                    presentation.VQGInstance.Quality.Count - 1)
                {
                    item.SpacingAfter = listItemSpacing;
                }
                vqgiQualityContentList.Add(item);
            }
            vqgiQualityContentCell.AddElement(vqgiQualityContentList);
            AddTextCell(vqgiQualityTable, vqgiQualityContentCell, Element.ALIGN_LEFT, Element.ALIGN_TOP, PDFBorderType.NONE);

            doc.Add(vqgiQualityTable);
            #endregion
            #endregion

            //doc.Add(new LineSeparator() { Offset = -10 });
            #endregion

            #region SWOTAnalysisInstance
            doc.NewPage();

            #region Header
            PdfPTable saiHeaderTable = new PdfPTable(2);
            saiHeaderTable.WidthPercentage = 100;
            saiHeaderTable.SpacingBefore = 10;
            saiHeaderTable.SetWidths(new float[] { 5, 1 });

            PdfPCell saiHeaderContentCell = new PdfPCell(new Phrase("SWOT Analysis", PDFFontStyle.STYLE_11));
            saiHeaderContentCell.PaddingTop = 10;
            saiHeaderContentCell.PaddingBottom = 10;
            AddTextCell(saiHeaderTable, saiHeaderContentCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);
            PdfPCell saiHeaderImageCell = new PdfPCell(image, false);
            saiHeaderImageCell.PaddingTop = 10;
            saiHeaderImageCell.PaddingBottom = 10;
            AddTextCell(saiHeaderTable, saiHeaderImageCell, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);
            doc.Add(saiHeaderTable);
            #endregion

            #region SWOT Analysis Content
            PdfPTable saiValueTable = new PdfPTable(3);
            saiValueTable.WidthPercentage = 100;
            saiValueTable.SpacingBefore = 5;
            saiValueTable.SetWidths(new float[] { 25, 1, 25 });

            PdfPCell saiStrengthCell = new PdfPCell(new Phrase("STRENGTH", PDFFontStyle.STYLE_9));
            saiStrengthCell.BackgroundColor = BaseColor.GRAY;
            AddTextCell(saiValueTable, saiStrengthCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.NONE);
            AddTextCell(saiValueTable, new PdfPCell(), Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.NONE);
            PdfPCell saiWeaknessCell = new PdfPCell(new Phrase("WEAKNESS", PDFFontStyle.STYLE_9));
            saiWeaknessCell.BackgroundColor = BaseColor.GRAY;
            AddTextCell(saiValueTable, saiWeaknessCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.NONE);


            PdfPCell saiStrengthContentCell = new PdfPCell();
            saiStrengthContentCell.MinimumHeight = (doc.PageSize.Height - saiHeaderTable.TotalHeight - 100) * Convert.ToSingle(0.5);
            ZapfDingbatsList saiStrengthContentList = new ZapfDingbatsList(110, 10);
            saiStrengthContentList.IndentationLeft = 2;
            saiStrengthContentList.Symbol.Font.Size = 10F;
            saiStrengthContentList.Symbol.Font.Color = BaseColor.BLUE;

            foreach (String value in presentation.SWOTAnalysisInstance.Strength)
            {
                ListItem item = new ListItem(new Phrase(value, PDFFontStyle.STYLE_8));
                if (presentation.SWOTAnalysisInstance.Strength.IndexOf(value) == 0)
                {
                    item.SpacingBefore = listItemSpacing;
                }

                if (presentation.SWOTAnalysisInstance.Strength.IndexOf(value) !=
                    presentation.SWOTAnalysisInstance.Strength.Count - 1)
                {
                    item.SpacingAfter = listItemSpacing;
                }
                saiStrengthContentList.Add(item);
            }
            saiStrengthContentCell.AddElement(saiStrengthContentList);
            saiStrengthContentCell.PaddingBottom = 20;
            AddTextCell(saiValueTable, saiStrengthContentCell, Element.ALIGN_LEFT, Element.ALIGN_TOP, PDFBorderType.NONE);

            AddTextCell(saiValueTable, new PdfPCell() { PaddingBottom = 20 }, Element.ALIGN_TOP, Element.ALIGN_CENTER, PDFBorderType.NONE);

            PdfPCell saiWeaknessContentCell = new PdfPCell();
            ZapfDingbatsList saiWeaknessContentList = new ZapfDingbatsList(110, 10);
            saiWeaknessContentList.IndentationLeft = 2;
            saiWeaknessContentList.Symbol.Font.Size = 10F;
            saiWeaknessContentList.Symbol.Font.Color = BaseColor.BLUE;

            foreach (String value in presentation.SWOTAnalysisInstance.Weakness)
            {
                ListItem item = new ListItem(new Phrase(value, PDFFontStyle.STYLE_8));
                if (presentation.SWOTAnalysisInstance.Weakness.IndexOf(value) == 0)
                {
                    item.SpacingBefore = listItemSpacing;
                }

                if (presentation.SWOTAnalysisInstance.Weakness.IndexOf(value) !=
                    presentation.SWOTAnalysisInstance.Weakness.Count - 1)
                {
                    item.SpacingAfter = listItemSpacing;
                }
                saiWeaknessContentList.Add(item);
            }
            saiWeaknessContentCell.AddElement(saiWeaknessContentList);
            saiWeaknessContentCell.PaddingBottom = 20;
            AddTextCell(saiValueTable, saiWeaknessContentCell, Element.ALIGN_LEFT, Element.ALIGN_TOP, PDFBorderType.NONE);

            PdfPCell saiOpportunityCell = new PdfPCell(new Phrase("OPPORTUNITY", PDFFontStyle.STYLE_9));
            saiOpportunityCell.BackgroundColor = BaseColor.GRAY;
            AddTextCell(saiValueTable, saiOpportunityCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.NONE);
            AddTextCell(saiValueTable, new PdfPCell(), Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.NONE);
            PdfPCell saiThreatCell = new PdfPCell(new Phrase("THREAT", PDFFontStyle.STYLE_9));
            saiThreatCell.BackgroundColor = BaseColor.GRAY;
            AddTextCell(saiValueTable, saiThreatCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.NONE);


            PdfPCell saiOpportunityContentCell = new PdfPCell();
            saiOpportunityContentCell.MinimumHeight = (doc.PageSize.Height - saiHeaderTable.TotalHeight - 100) * Convert.ToSingle(0.5);
            ZapfDingbatsList saiOpportunityContentList = new ZapfDingbatsList(110, 10);
            saiOpportunityContentList.IndentationLeft = 2;
            saiOpportunityContentList.Symbol.Font.Size = 10F;
            saiOpportunityContentList.Symbol.Font.Color = BaseColor.BLUE;

            foreach (String value in presentation.SWOTAnalysisInstance.Opportunity)
            {
                ListItem item = new ListItem(new Phrase(value, PDFFontStyle.STYLE_8));
                if (presentation.SWOTAnalysisInstance.Opportunity.IndexOf(value) == 0)
                {
                    item.SpacingBefore = listItemSpacing;
                }

                if (presentation.SWOTAnalysisInstance.Opportunity.IndexOf(value) !=
                    presentation.SWOTAnalysisInstance.Opportunity.Count - 1)
                {
                    item.SpacingAfter = listItemSpacing;
                }
                saiOpportunityContentList.Add(item);
            }
            saiOpportunityContentCell.AddElement(saiOpportunityContentList);
            AddTextCell(saiValueTable, saiOpportunityContentCell, Element.ALIGN_LEFT, Element.ALIGN_TOP, PDFBorderType.NONE);

            AddTextCell(saiValueTable, new PdfPCell(), Element.ALIGN_TOP, Element.ALIGN_CENTER, PDFBorderType.NONE);

            PdfPCell saiThreatContentCell = new PdfPCell();
            ZapfDingbatsList saiThreatContentList = new ZapfDingbatsList(110, 10);
            saiThreatContentList.IndentationLeft = 2;
            saiThreatContentList.Symbol.Font.Size = 10F;
            saiThreatContentList.Symbol.Font.Color = BaseColor.BLUE;

            foreach (String value in presentation.SWOTAnalysisInstance.Threat)
            {
                ListItem item = new ListItem(new Phrase(value, PDFFontStyle.STYLE_8));
                if (presentation.SWOTAnalysisInstance.Threat.IndexOf(value) == 0)
                {
                    item.SpacingBefore = listItemSpacing;
                }

                if (presentation.SWOTAnalysisInstance.Threat.IndexOf(value) !=
                    presentation.SWOTAnalysisInstance.Threat.Count - 1)
                {
                    item.SpacingAfter = listItemSpacing;
                }
                saiThreatContentList.Add(item);
            }
            saiThreatContentCell.AddElement(saiThreatContentList);
            AddTextCell(saiValueTable, saiThreatContentCell, Element.ALIGN_LEFT, Element.ALIGN_TOP, PDFBorderType.NONE);

            doc.Add(saiValueTable);
            #endregion

            //doc.Add(new LineSeparator() { Offset = -10 });
            #endregion

            doc.Close();
        }

        private static List<String> GetCompanyOverview(SlidePart slidePart)
        {
            List<String> items = new List<string>();

            TextBody overviewText = slidePart.Slide.Descendants<TextBody>().FirstOrDefault();

            var paraGraphs = overviewText.Descendants<Drawing.Paragraph>().ToList();

            if (paraGraphs != null && paraGraphs.Count > 0)
            {
                for (int i = 1; i < paraGraphs.Count; i++)
                {
                    if (!String.IsNullOrEmpty(paraGraphs[i].InnerText))
                    {
                        items.Add(paraGraphs[i].InnerText);
                    }
                }
            }

            return items;
        }

        private static List<String> GetInvestmentThesis(SlidePart slidePart)
        {
            List<String> items = new List<string>();

            CommonSlideData investmentText = slidePart.Slide.Descendants<CommonSlideData>().FirstOrDefault();

            var shapeTree = investmentText.Descendants<ShapeTree>().FirstOrDefault();

            var textBody = shapeTree.Descendants<TextBody>().ToList();

            if (textBody.Count > 0)
            {
                var paragraphs = textBody[1].Descendants<Drawing.Paragraph>().ToList();

                if (paragraphs != null && paragraphs.Count > 0)
                {
                    for (int i = 0; i < paragraphs.Count; i++)
                    {
                        string thesisText = String.Empty;
                        var runList = paragraphs[i].Descendants<Drawing.Run>().ToList();

                        if (runList != null && runList.Count > 0)
                        {
                            foreach (Drawing.Run run in runList)
                            {
                                thesisText += run.InnerText;
                            }
                        }

                        if (!String.IsNullOrEmpty(thesisText))
                        {
                            items.Add(thesisText);

                        }
                    }
                }
            }

            return items;
        }

        private static List<String> GetInvestmentRisk(SlidePart slidePart)
        {
            List<String> risks = new List<string>();

            CommonSlideData investmentText = slidePart.Slide.Descendants<CommonSlideData>().FirstOrDefault();

            var shapeTree = investmentText.Descendants<ShapeTree>().FirstOrDefault();
            var textBody = shapeTree.Descendants<TextBody>().ToList();
            if (textBody.Count > 0)
            {
                var paragraphs = textBody[2].Descendants<Drawing.Paragraph>().ToList();
                if (paragraphs != null && paragraphs.Count > 0)
                {
                    for (int i = 1; i < paragraphs.Count; i++)
                    {
                        string thesisText = String.Empty;
                        var runList = paragraphs[i].Descendants<Drawing.Run>().ToList();
                        if (runList != null && runList.Count > 0)
                        {
                            foreach (Drawing.Run run in runList)
                            {
                                thesisText += run.InnerText;
                            }
                        }
                        if (!String.IsNullOrEmpty(thesisText))
                        {
                            risks.Add(thesisText);
                        }
                    }
                }
            }

            return risks;
        }

        private static List<String> GetKeyOperatingAssumpations(SlidePart slidePart)
        {
            List<String> items = new List<String>();

            CommonSlideData investmentText = slidePart.Slide.Descendants<CommonSlideData>().FirstOrDefault();

            var shapeTree = investmentText.Descendants<ShapeTree>().FirstOrDefault();

            var shapeContent = investmentText.Descendants<Shape>().FirstOrDefault();

            var textBody = shapeContent.Descendants<TextBody>().FirstOrDefault();

            if (textBody != null)
            {
                var paragraphs = textBody.Descendants<Drawing.Paragraph>().ToList();

                if (paragraphs != null && paragraphs.Count > 0)
                {
                    for (int i = 0; i < paragraphs.Count; i++)
                    {
                        string thesisText = String.Empty;
                        var runList = paragraphs[i].Descendants<Drawing.Run>().ToList();

                        if (runList != null && runList.Count > 0)
                        {
                            foreach (Drawing.Run run in runList)
                            {
                                thesisText += run.InnerText;
                            }
                        }

                        if (!String.IsNullOrEmpty(thesisText))
                        {
                            items.Add(thesisText);
                        }
                    }
                }
            }

            return items;
        }

        private static Dictionary<int, List<String>> GetVQG(SlidePart slidePart)
        {
            Dictionary<int, List<String>> result = new Dictionary<int, List<string>>();

            CommonSlideData investmentText = slidePart.Slide.Descendants<CommonSlideData>().FirstOrDefault();

            var shapeTree = investmentText.Descendants<ShapeTree>().FirstOrDefault();

            var shapeContent = investmentText.Descendants<Shape>().ToList();

            for (int i = 3; i < 6; i++)
            {
                var textBody = shapeContent[i].Descendants<TextBody>().FirstOrDefault();

                if (textBody != null)
                {
                    var paragraphs = textBody.Descendants<Drawing.Paragraph>().ToList();

                    if (paragraphs != null && paragraphs.Count > 0)
                    {
                        List<String> items = new List<string>();

                        for (int j = 0; j < paragraphs.Count; j++)
                        {
                            string thesisText = String.Empty;
                            var runList = paragraphs[j].Descendants<Drawing.Run>().ToList();

                            if (runList != null && runList.Count > 0)
                            {
                                foreach (Drawing.Run run in runList)
                                {
                                    thesisText += run.InnerText;
                                }
                            }

                            if (!String.IsNullOrEmpty(thesisText))
                            {
                                items.Add(thesisText);
                            }
                        }
                        result.Add(i - 3, items);
                    }
                }
            }
            return result;
        }

        private static Dictionary<int, List<String>> GetSWOTAnalysis(SlidePart slidePart)
        {
            Dictionary<int, List<String>> result = new Dictionary<int, List<string>>();

            CommonSlideData investmentText = slidePart.Slide.Descendants<CommonSlideData>().FirstOrDefault();

            var shapeTree = investmentText.Descendants<ShapeTree>().FirstOrDefault();

            var groupShape = investmentText.Descendants<GroupShape>().FirstOrDefault();
            var shapeData = groupShape.Descendants<Shape>().ToList();

            for (int i = 4; i < 8; i++)
            {
                var textBody = shapeData[i].Descendants<TextBody>().FirstOrDefault();

                if (textBody != null)
                {
                    var paragraphs = textBody.Descendants<Drawing.Paragraph>().ToList();

                    if (paragraphs != null && paragraphs.Count > 0)
                    {
                        List<String> items = new List<string>();

                        for (int j = 0; j < paragraphs.Count; j++)
                        {
                            string thesisText = String.Empty;
                            var runList = paragraphs[j].Descendants<Drawing.Run>().ToList();

                            if (runList != null && runList.Count > 0)
                            {
                                foreach (Drawing.Run run in runList)
                                {
                                    thesisText += run.InnerText;
                                }
                            }

                            if (!String.IsNullOrEmpty(thesisText))
                            {
                                items.Add(thesisText);
                            }
                        }
                        result.Add(i - 4, items);
                    }
                }
            }

            return result;
        }

        private static void AddTextCell(PdfPTable table, PdfPCell cell, int HorizontalAlignment = Element.ALIGN_LEFT, int VerticalAlignment = Element.ALIGN_MIDDLE, int Border = 0)
        {
            //cell.Colspan = Colspan;
            cell.HorizontalAlignment = HorizontalAlignment; //0=Left, 1=Centre, 2=Right
            cell.VerticalAlignment = VerticalAlignment;
            cell.Border = Border;
            cell.BorderWidth = 1;
            //cell.Rowspan = Rowspan;
            table.AddCell(cell);
        }
    }
}