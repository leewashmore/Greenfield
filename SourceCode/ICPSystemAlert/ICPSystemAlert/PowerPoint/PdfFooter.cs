using System;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ICPSystemAlert
{
    /// <summary>
    /// Marks footer information in pdf file
    /// </summary>
    public class PdfFooter : PdfPageEventHelper
    {
        #region Fields
        /// <summary>
        /// contentbyte object of the writer
        /// </summary>
        PdfContentByte cb;

        /// <summary>
        /// final number of pages in a template
        /// </summary>
        PdfTemplate template;

        /// <summary>
        /// BaseFont we are going to use for the header / footer
        /// </summary>
        BaseFont bf = null;

        /// <summary>
        /// keeps track of the creation time
        /// </summary>
        DateTime PrintTime = DateTime.UtcNow;
        #endregion

        #region Properties
        /// <summary>
        /// Stores page title
        /// </summary>
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// Stores date in the footer cell
        /// </summary>
        private String footerDate;
        public String FooterDate
        {
            get { return footerDate; }
            set { footerDate = value; }
        }

        /// <summary>
        /// Stores header on the left side
        /// </summary>
        private string headerLeft;
        public string HeaderLeft
        {
            get { return headerLeft; }
            set { headerLeft = value; }
        }

        /// <summary>
        /// Stores header on the right side
        /// </summary>
        private string headerRight;
        public string HeaderRight
        {
            get { return headerRight; }
            set { headerRight = value; }
        }

        /// <summary>
        /// Stores header font
        /// </summary>
        private Font headerFont;
        public Font HeaderFont
        {
            get { return headerFont; }
            set { headerFont = value; }
        }

        /// <summary>
        /// Stores footer font
        /// </summary>
        private Font footerFont;
        public Font FooterFont
        {
            get { return footerFont; }
            set { footerFont = value; }
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// override the onOpenDocument method
        /// </summary>
        /// <param name="writer">PdfWriter</param>
        /// <param name="document">Document</param>
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                template = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException)
            {
            }
            catch (System.IO.IOException)
            {
            }
        }

        /// <summary>
        /// override the OnStartPage method
        /// </summary>
        /// <param name="writer">PdfWriter</param>
        /// <param name="document">Document</param>
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);

            Rectangle pageSize = document.PageSize;

            if (Title != string.Empty)
            {
                cb.BeginText();
                cb.SetFontAndSize(bf, 15);
                cb.SetRGBColorFill(50, 50, 200);
                cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetTop(40));
                cb.ShowText(Title);
                cb.EndText();
            }

            if (HeaderLeft + HeaderRight != string.Empty)
            {
                PdfPTable HeaderTable = new PdfPTable(2);
                HeaderTable.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                HeaderTable.TotalWidth = pageSize.Width - 80;
                HeaderTable.SetWidthPercentage(new float[] { 45, 45 }, pageSize);

                PdfPCell HeaderLeftCell = new PdfPCell(new Phrase(8, HeaderLeft, HeaderFont));
                HeaderLeftCell.Padding = 5;
                HeaderLeftCell.PaddingBottom = 8;
                HeaderLeftCell.BorderWidthRight = 0;
                HeaderTable.AddCell(HeaderLeftCell);

                PdfPCell HeaderRightCell = new PdfPCell(new Phrase(8, HeaderRight, HeaderFont));
                HeaderRightCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                HeaderRightCell.Padding = 5;
                HeaderRightCell.PaddingBottom = 8;
                HeaderRightCell.BorderWidthLeft = 0;
                HeaderTable.AddCell(HeaderRightCell);

                cb.SetRGBColorFill(0, 0, 0);
                HeaderTable.WriteSelectedRows(0, -1, pageSize.GetLeft(40), pageSize.GetTop(50), cb);
            }
        }

        /// <summary>
        /// override the OnEndPage method
        /// </summary>
        /// <param name="writer">PdfWriter</param>
        /// <param name="document">Document</param>
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            int pageN = writer.PageNumber;
            String text = "Page " + pageN + " of ";
            float len = bf.GetWidthPoint(text, 8);

            Rectangle pageSize = document.PageSize;

            cb.SetRGBColorFill(100, 100, 100);

            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetBottom(30));
            cb.ShowText(text);
            cb.EndText();

            cb.AddTemplate(template, pageSize.GetLeft(40) + len, pageSize.GetBottom(30));

            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT,
                FooterDate,
                pageSize.GetRight(40),
                pageSize.GetBottom(30), 0);
            cb.EndText();

            cb.MoveTo(pageSize.GetLeft(40), pageSize.GetBottom(40));
            cb.LineTo(pageSize.GetRight(40), pageSize.GetBottom(40));
            cb.Stroke();
        }

        /// <summary>
        /// override the OnCloseDocument method
        /// </summary>
        /// <param name="writer">PdfWriter</param>
        /// <param name="document">Document</param>
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            template.BeginText();
            template.SetFontAndSize(bf, 8);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber - 1));
            template.EndText();
        }
        #endregion
    }
}