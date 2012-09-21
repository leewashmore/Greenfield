using DocumentFormat.OpenXml.Packaging;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;

namespace GeneratedCode
{
    public class GeneratedClass
    {
        // Adds child parts and generates content of the specified part.
        public void CreateWorksheetPart(WorksheetPart part)
        {
            SpreadsheetPrinterSettingsPart spreadsheetPrinterSettingsPart1 = part.AddNewPart<SpreadsheetPrinterSettingsPart>("rId1");
            GenerateSpreadsheetPrinterSettingsPart1Content(spreadsheetPrinterSettingsPart1);

            GeneratePartContent(part);

        }

        // Generates content of spreadsheetPrinterSettingsPart1.
        private void GenerateSpreadsheetPrinterSettingsPart1Content(SpreadsheetPrinterSettingsPart spreadsheetPrinterSettingsPart1)
        {
            System.IO.Stream data = GetBinaryDataStream(spreadsheetPrinterSettingsPart1Data);
            spreadsheetPrinterSettingsPart1.FeedData(data);
            data.Close();
        }

        // Generates content of part.
        private void GeneratePartContent(WorksheetPart part)
        {
            Worksheet worksheet1 = new Worksheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            worksheet1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            worksheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            worksheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            SheetDimension sheetDimension1 = new SheetDimension() { Reference = "A1:AV7" };

            SheetViews sheetViews1 = new SheetViews();

            SheetView sheetView1 = new SheetView() { TabSelected = true, WorkbookViewId = (UInt32Value)0U };
            Selection selection1 = new Selection() { ActiveCell = "G2", SequenceOfReferences = new ListValue<StringValue>() { InnerText = "G2" } };

            sheetView1.Append(selection1);

            sheetViews1.Append(sheetView1);
            SheetFormatProperties sheetFormatProperties1 = new SheetFormatProperties() { DefaultRowHeight = 15D, OutlineLevelColumn = 1, DyDescent = 0.25D };

            Columns columns1 = new Columns();
            Column column1 = new Column() { Min = (UInt32Value)2U, Max = (UInt32Value)2U, Width = 38.28515625D, CustomWidth = true };
            Column column2 = new Column() { Min = (UInt32Value)3U, Max = (UInt32Value)4U, Width = 11.28515625D, Style = (UInt32Value)2U, CustomWidth = true, OutlineLevel = 1 };
            Column column3 = new Column() { Min = (UInt32Value)5U, Max = (UInt32Value)5U, Width = 10.7109375D, Style = (UInt32Value)2U, CustomWidth = true, OutlineLevel = 1 };
            Column column4 = new Column() { Min = (UInt32Value)6U, Max = (UInt32Value)6U, Width = 9.5703125D, Style = (UInt32Value)2U, CustomWidth = true, OutlineLevel = 1 };
            Column column5 = new Column() { Min = (UInt32Value)7U, Max = (UInt32Value)7U, Width = 10.140625D, Style = (UInt32Value)2U, CustomWidth = true };

            

            columns1.Append(column1);
            columns1.Append(column2);
            columns1.Append(column3);
            columns1.Append(column4);
            columns1.Append(column5);

            SheetData sheetData1 = new SheetData();

            Row row1 = new Row() { RowIndex = (UInt32Value)1U, Spans = new ListValue<StringValue>() { InnerText = "1:7" }, StyleIndex = (UInt32Value)5U, CustomFormat = true, DyDescent = 0.25D };

            Cell cell1 = new Cell() { CellReference = "A1", StyleIndex = (UInt32Value)6U, DataType = CellValues.SharedString };
            CellValue cellValue1 = new CellValue();
            cellValue1.Text = "6";

            cell1.Append(cellValue1);
            Cell cell2 = new Cell() { CellReference = "B1", StyleIndex = (UInt32Value)6U };

            Cell cell3 = new Cell() { CellReference = "C1", StyleIndex = (UInt32Value)4U };
            CellValue cellValue2 = new CellValue();
            cellValue2.Text = "2003";

            cell3.Append(cellValue2);

            Cell cell4 = new Cell() { CellReference = "D1", StyleIndex = (UInt32Value)4U };
            CellValue cellValue3 = new CellValue();
            cellValue3.Text = "2003";

            cell4.Append(cellValue3);

            Cell cell5 = new Cell() { CellReference = "E1", StyleIndex = (UInt32Value)4U };
            CellValue cellValue4 = new CellValue();
            cellValue4.Text = "2003";

            cell5.Append(cellValue4);

            Cell cell6 = new Cell() { CellReference = "F1", StyleIndex = (UInt32Value)4U };
            CellValue cellValue5 = new CellValue();
            cellValue5.Text = "2003";

            cell6.Append(cellValue5);

            Cell cell7 = new Cell() { CellReference = "G1", StyleIndex = (UInt32Value)4U };
            CellValue cellValue6 = new CellValue();
            cellValue6.Text = "2003";

            cell7.Append(cellValue6);

            row1.Append(cell1);
            row1.Append(cell2);
            row1.Append(cell3);
            row1.Append(cell4);
            row1.Append(cell5);
            row1.Append(cell6);
            row1.Append(cell7);

            Row row2 = new Row() { RowIndex = (UInt32Value)2U, Spans = new ListValue<StringValue>() { InnerText = "1:7" }, StyleIndex = (UInt32Value)5U, CustomFormat = true, DyDescent = 0.25D };
            Cell cell8 = new Cell() { CellReference = "A2", StyleIndex = (UInt32Value)6U };
            Cell cell9 = new Cell() { CellReference = "B2", StyleIndex = (UInt32Value)6U };

            Cell cell10 = new Cell() { CellReference = "C2", StyleIndex = (UInt32Value)4U, DataType = CellValues.SharedString };
            CellValue cellValue7 = new CellValue();
            cellValue7.Text = "0";

            cell10.Append(cellValue7);

            Cell cell11 = new Cell() { CellReference = "D2", StyleIndex = (UInt32Value)4U, DataType = CellValues.SharedString };
            CellValue cellValue8 = new CellValue();
            cellValue8.Text = "1";

            cell11.Append(cellValue8);

            Cell cell12 = new Cell() { CellReference = "E2", StyleIndex = (UInt32Value)4U, DataType = CellValues.SharedString };
            CellValue cellValue9 = new CellValue();
            cellValue9.Text = "2";

            cell12.Append(cellValue9);

            Cell cell13 = new Cell() { CellReference = "F2", StyleIndex = (UInt32Value)4U, DataType = CellValues.SharedString };
            CellValue cellValue10 = new CellValue();
            cellValue10.Text = "3";

            cell13.Append(cellValue10);

            Cell cell14 = new Cell() { CellReference = "G2", StyleIndex = (UInt32Value)4U, DataType = CellValues.SharedString };
            CellValue cellValue11 = new CellValue();
            cellValue11.Text = "4";

            cell14.Append(cellValue11);

            row2.Append(cell8);
            row2.Append(cell9);
            row2.Append(cell10);
            row2.Append(cell11);
            row2.Append(cell12);
            row2.Append(cell13);
            row2.Append(cell14);

            Row row3 = new Row() { RowIndex = (UInt32Value)3U, Spans = new ListValue<StringValue>() { InnerText = "1:7" }, DyDescent = 0.25D };

            Cell cell15 = new Cell() { CellReference = "A3" };
            CellValue cellValue12 = new CellValue();
            cellValue12.Text = "3";

            cell15.Append(cellValue12);

            Cell cell16 = new Cell() { CellReference = "B3", DataType = CellValues.SharedString };
            CellValue cellValue13 = new CellValue();
            cellValue13.Text = "5";

            cell16.Append(cellValue13);

            row3.Append(cell15);
            row3.Append(cell16);

            Row row4 = new Row() { RowIndex = (UInt32Value)5U, Spans = new ListValue<StringValue>() { InnerText = "1:7" }, DyDescent = 0.25D };
            Cell cell17 = new Cell() { CellReference = "A5", StyleIndex = (UInt32Value)1U };
            Cell cell18 = new Cell() { CellReference = "B5", StyleIndex = (UInt32Value)1U };
            Cell cell19 = new Cell() { CellReference = "C5", StyleIndex = (UInt32Value)3U };
            Cell cell20 = new Cell() { CellReference = "D5", StyleIndex = (UInt32Value)3U };
            Cell cell21 = new Cell() { CellReference = "E5", StyleIndex = (UInt32Value)3U };
            Cell cell22 = new Cell() { CellReference = "F5", StyleIndex = (UInt32Value)3U };
            Cell cell23 = new Cell() { CellReference = "G5", StyleIndex = (UInt32Value)3U };

            row4.Append(cell17);
            row4.Append(cell18);
            row4.Append(cell19);
            row4.Append(cell20);
            row4.Append(cell21);
            row4.Append(cell22);
            row4.Append(cell23);

            Row row5 = new Row() { RowIndex = (UInt32Value)7U, Spans = new ListValue<StringValue>() { InnerText = "1:7" }, DyDescent = 0.25D };
            Cell cell24 = new Cell() { CellReference = "A7", StyleIndex = (UInt32Value)1U };
            Cell cell25 = new Cell() { CellReference = "B7", StyleIndex = (UInt32Value)1U };
            Cell cell26 = new Cell() { CellReference = "C7", StyleIndex = (UInt32Value)3U };
            Cell cell27 = new Cell() { CellReference = "D7", StyleIndex = (UInt32Value)3U };
            Cell cell28 = new Cell() { CellReference = "E7", StyleIndex = (UInt32Value)3U };
            Cell cell29 = new Cell() { CellReference = "F7", StyleIndex = (UInt32Value)3U };
            Cell cell30 = new Cell() { CellReference = "G7", StyleIndex = (UInt32Value)3U };

            row5.Append(cell24);
            row5.Append(cell25);
            row5.Append(cell26);
            row5.Append(cell27);
            row5.Append(cell28);
            row5.Append(cell29);
            row5.Append(cell30);

            sheetData1.Append(row1);
            sheetData1.Append(row2);
            sheetData1.Append(row3);
            sheetData1.Append(row4);
            sheetData1.Append(row5);

            MergeCells mergeCells1 = new MergeCells() { Count = (UInt32Value)1U };
            MergeCell mergeCell1 = new MergeCell() { Reference = "A1:B2" };

            mergeCells1.Append(mergeCell1);
            PageMargins pageMargins1 = new PageMargins() { Left = 0.7D, Right = 0.7D, Top = 0.75D, Bottom = 0.75D, Header = 0.3D, Footer = 0.3D };
            PageSetup pageSetup1 = new PageSetup() { Orientation = OrientationValues.Portrait, VerticalDpi = (UInt32Value)2U, Id = "rId1" };

            worksheet1.Append(sheetDimension1);
            worksheet1.Append(sheetViews1);
            worksheet1.Append(sheetFormatProperties1);
            worksheet1.Append(columns1);
            worksheet1.Append(sheetData1);
            worksheet1.Append(mergeCells1);
            worksheet1.Append(pageMargins1);
            worksheet1.Append(pageSetup1);

            part.Worksheet = worksheet1;
        }

        #region Binary Data
        private string spreadsheetPrinterSettingsPart1Data = "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AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";

        private System.IO.Stream GetBinaryDataStream(string base64String)
        {
            return new System.IO.MemoryStream(System.Convert.FromBase64String(base64String));
        }

        #endregion

    }
}
