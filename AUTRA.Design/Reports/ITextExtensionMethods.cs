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
        public static Paragraph AddCustomParagraph(this Document doc , string text,float fontSize)
        {
            Paragraph p = new Paragraph(text)
                .SetFontSize(fontSize);
            doc.Add(p);
            return p;
        }
        public static List AddList(this Document doc , string title , List<string> items)
        {
            doc.AddCustomParagraph(title,12).SetBold();
            List lst = new List()
                .SetSymbolIndent(12) //How far items are far from header
                .SetListSymbol("\u2022"); //symbol used at the begining of each item
            items.ForEach(i => lst.Add(new ListItem(i)));
            doc.Add(lst);
            return lst;
        }
        public static Document AddHeader(this Document doc , string title , float fontSize = 20, TextAlignment textAlignment = TextAlignment.CENTER)
        {
            Paragraph header = new Paragraph(title)
                .SetTextAlignment(textAlignment)
                .SetFontSize(fontSize)
                .SetUnderline()
                .SetBold();
            doc.Add(header);
            return doc;
        }
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
        public static Document AddParagraph(this Document doc, string text) => doc.Add(new Paragraph(text));
        public static Cell AddHeaderCell(this Cell cell , string text)
        {
            cell.SetBackgroundColor(ColorConstants.GRAY).
                SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph(text));
            return cell;
        }
        public static Cell AddCell(this Cell cell, string text)
        {
            cell.SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph(text));
            return cell;
        }
    }
}
