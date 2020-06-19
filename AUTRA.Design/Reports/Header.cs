using System;
using System.Collections.Generic;
using System.Text;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace AUTRA.Design
{
    public class Header : IEventHandler
    {
        private readonly Document _doc;
        private readonly string _userName;
        private readonly ImageData _img;
        public Header(Document doc,string userName,ImageData img)
        {
            _doc = doc;
            _userName = userName;
            _img = img;
        }
        public void HandleEvent(Event @event)
        {
            PdfDocumentEvent docEvent = @event as PdfDocumentEvent;
            PdfPage page = docEvent.GetPage();
            PdfDocument pdf = docEvent.GetDocument();
            Rectangle pageSize = page.GetPageSize();
            float leftx = pageSize.GetLeft() + _doc.GetLeftMargin()+10;
            float rightx = pageSize.GetRight() - _doc.GetRightMargin() - 10;
            float headery = pageSize.GetTop() - _doc.GetTopMargin() + 15;
            PdfCanvas pdfCanvas = new PdfCanvas(page.GetLastContentStream(), page.GetResources(), pdf);
            Rectangle rect = new Rectangle(leftx, headery, 60f, 20.0f);
            pdfCanvas.AddImage(_img, rect,true);
            Canvas canvas = new Canvas(pdfCanvas, pageSize);
            canvas.ShowTextAligned(_userName, rightx, headery, TextAlignment.RIGHT)
                .Close();
            pdfCanvas.Release();
        }
    }

}
