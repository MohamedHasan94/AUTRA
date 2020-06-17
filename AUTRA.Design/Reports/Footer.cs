using System;
using System.Collections.Generic;
using System.Text;
using iText.IO.Font.Constants;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace AUTRA.Design
{
    public class Footer : IEventHandler
    {
        private readonly Document _doc;
        public Footer(Document doc)
        {
            _doc = doc;
        }
        public void HandleEvent(Event @event)
        {
            PdfDocumentEvent docEvent = @event as PdfDocumentEvent;
            PdfPage page = docEvent.GetPage();
            int pageNum = docEvent.GetDocument().GetPageNumber(page);
            Rectangle pageSize = docEvent.GetPage().GetPageSize();
            float coordx = ((pageSize.GetLeft() + _doc.GetLeftMargin()) + (pageSize.GetRight() - _doc.GetRightMargin())) / 2;
            float headery = pageSize.GetBottom() + _doc.GetBottomMargin() -15;
            Canvas canvas = new Canvas(docEvent.GetPage(), pageSize);
            //_doc.Add(new Paragraph(""));
            canvas.ShowTextAligned(string.Format($"Page: {pageNum.ToString()}"), coordx, headery, TextAlignment.CENTER).Close();
        }
    }
}
