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
    public class Header : IEventHandler
    {
        private readonly Document _doc;
        public Header(Document doc)
        {
            _doc = doc;
        }
        public void HandleEvent(Event @event)
        {
            PdfDocumentEvent docEvent = @event as PdfDocumentEvent;
            Rectangle pageSize = docEvent.GetPage().GetPageSize();
            float leftx = pageSize.GetLeft() + _doc.GetLeftMargin()+10;
            float rightx = pageSize.GetRight() - _doc.GetRightMargin() - 10;
            float headery = pageSize.GetTop() - _doc.GetTopMargin() + 15;
            Canvas canvas = new Canvas(docEvent.GetPage(), pageSize);
            //_doc.Add(new Paragraph(""));
            //_doc.AddLineSeparator();
            canvas.ShowTextAligned("AUTRA", leftx, headery, TextAlignment.LEFT)
                .ShowTextAligned("Project", rightx, headery, TextAlignment.RIGHT)
                .Close();
        }
    }

}
