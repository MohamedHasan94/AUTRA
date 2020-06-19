using System;
using System.Collections.Generic;
using System.Text;
using iText.IO.Font.Constants;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace AUTRA.Design
{
    public class Footer : IEventHandler
    {
        private readonly Document _doc;
        private readonly PdfFormXObject _placeholder;
        public Footer(Document doc)
        {
            _doc = doc;
            _placeholder = new PdfFormXObject(new Rectangle(0, 0, 20, 20));
        }
        public void HandleEvent(Event @event)
        {
            PdfDocumentEvent docEvent = @event as PdfDocumentEvent;
            PdfDocument pdf = docEvent.GetDocument();
            PdfPage page = docEvent.GetPage();
            int pageNum = pdf.GetPageNumber(page);
            Rectangle pageSize = page.GetPageSize();
            float coordx = ((pageSize.GetLeft() + _doc.GetLeftMargin()) + (pageSize.GetRight() - _doc.GetRightMargin())) / 2;
            float footery = pageSize.GetBottom() + _doc.GetBottomMargin() -15;
            PdfCanvas pdfCanvas = new PdfCanvas(page.GetLastContentStream(), page.GetResources(), pdf);
            Canvas canvas = new Canvas(pdfCanvas, pageSize);
            Paragraph p = new Paragraph(string.Format($"page {pageNum} of"));
            canvas.ShowTextAligned(p, coordx, footery, TextAlignment.CENTER).Close();
            pdfCanvas.AddXObject(_placeholder, coordx + 30f, footery - 3);
            pdfCanvas.Release();
        }
        public void writeTotal(PdfDocument pdf)
        {
            Canvas canvas = new Canvas(_placeholder, pdf);
            canvas.ShowTextAligned(pdf.GetNumberOfPages().ToString(), 0, 3, TextAlignment.LEFT);
        }
    }
}
