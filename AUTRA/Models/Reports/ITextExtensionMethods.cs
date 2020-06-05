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

namespace AUTRA.Models.Reports
{
    internal static class ITextExtensionMethods
    {
        public static Cell AddHeaderCell(this Cell cell , string text)
        {
            cell.SetBackgroundColor(ColorConstants.GRAY).
                SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph(text));
            return cell;
        }
        public static Cell AddOrdinaryCell(this Cell cell, string text)
        {
            cell.SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph(text));
            return cell;
        }
    }
}
