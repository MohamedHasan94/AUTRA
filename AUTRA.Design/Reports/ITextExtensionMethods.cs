using System;
using System.Collections.Generic;
using System.Text;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Layout.Properties;
using iText.Kernel.Pdf.Action;

namespace AUTRA.Design
{
    internal static class ITextExtensionMethods
    {
        public static Image AddImage(this Document doc,string uri)
        {
            ImageData imageData = ImageDataFactory.Create(uri);
            // Create layout image object and provide parameters. Page number = 1
            Image image = new Image(imageData);
            // This adds the image to the page
            doc.Add(image);
            return image;
        }
        public static Cell AddCell(this Table table, int rowSpan = 1, int colSpan = 1)
        {
            var cell = new Cell(rowSpan, colSpan);
            table.AddCell(cell);
            return cell;
        } 
        public static void AddLineSeparator(this Document doc) => doc.Add(new LineSeparator(new SolidLine()));
        public static void AddParagraph(this Document doc, string text) => doc.Add(new Paragraph(text));
        public static Cell AddHeader(this Cell cell , string text)
        {
            cell.SetBackgroundColor(ColorConstants.GRAY).
                SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph(text));
            return cell;
        }
        public static Cell AddParagraph(this Cell cell, string text)
        {
            cell.SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph(text));
            return cell;
        }
    }
}
