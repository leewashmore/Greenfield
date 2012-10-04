using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.ComponentModel;
using Drawing = DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;

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

                String investmentRisk = GetInvestmentRisk(slide1);
                result.InvestmentThesisInstance.HighlightedRisk = investmentRisk;

                string relId2 = (slideIds[2] as SlideId).RelationshipId;
                // Get the slide part from the relationship ID.
                SlidePart slide2 = (SlidePart)presentatioPart.GetPartById(relId2);

                List<String> keyOperatingAssumpations = GetKeyOperatingAssumpations(slide2);
                result.KeyOperatingAssumpationsInstance =
                    new KeyOperatingAssumpations { Assumpations = keyOperatingAssumpations };

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
                        result.SWOTAnalysisInstance.Oppurtunity = kvp.Value;
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

        public static List<String> GetCompanyOverview(SlidePart slidePart)
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

        public static List<String> GetInvestmentThesis(SlidePart slidePart)
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

        public static String GetInvestmentRisk(SlidePart slidePart)
        {
            String risk = String.Empty;

            CommonSlideData investmentText = slidePart.Slide.Descendants<CommonSlideData>().FirstOrDefault();

            var shapeTree = investmentText.Descendants<ShapeTree>().FirstOrDefault();

            var textBody = shapeTree.Descendants<TextBody>().ToList();

            if (textBody.Count > 0)
            {
                var paragraphs = textBody[2].Descendants<Drawing.Paragraph>().ToList();

                if (paragraphs != null && paragraphs.Count > 0)
                {

                    string thesisText = String.Empty;
                    var runList = paragraphs[1].Descendants<Drawing.Run>().ToList();

                    if (runList != null && runList.Count > 0)
                    {
                        foreach (Drawing.Run run in runList)
                        {
                            thesisText += run.InnerText;
                        }
                    }

                    if (!String.IsNullOrEmpty(thesisText))
                    {
                        risk = thesisText;
                    }
                }
            }

            return risk;
        }

        public static List<String> GetKeyOperatingAssumpations(SlidePart slidePart)
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

        public static Dictionary<int, List<String>> GetVQG(SlidePart slidePart)
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

        public static Dictionary<int, List<String>> GetSWOTAnalysis(SlidePart slidePart)
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
    }
}