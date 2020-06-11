using System;
using System.Collections.Generic;
using System.Text;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Properties;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Action;

namespace AUTRA.Design
{
   public class Report
    {
        //Responsible for the IO operations
        public string FolderPath { get; set; }
        public Report(string folderPath)
        {
            FolderPath = folderPath;
        }

        public void Create(string fileName,List<Group> secGroups,List<Group> mainGroups)
        {
            string fullpath = Path.Combine(FolderPath, fileName);

            using (var writer = new PdfWriter(fullpath))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    Document doc = new Document(pdf);
                    ReportForBeams("Secondary Beams", secGroups, doc);
                    ReportForBeams("Main Beams", mainGroups, doc);
                }
            }
        }
        public void ReportForBeams(string title,List<Group> groups, Document doc)
        {
            CreateHeader( doc,title);
            CreateTableFromGroups(groups,doc);
        }
        private void CreateHeader( Document doc,string title,float fontSize=20,TextAlignment textAlignment = TextAlignment.CENTER,bool isSepartorExist = true)
        {
            Paragraph header= new Paragraph(title)
                .SetTextAlignment(textAlignment)
                .SetFontSize(fontSize)
                .SetBold();
            doc.Add(header);
            if (isSepartorExist)
            {
                LineSeparator ls = new LineSeparator(new SolidLine());
                doc.Add(ls);
            }
        }
        private void CreateTableFromGroups(List<Group> groups,Document doc)
        {
            foreach (var group in groups)
            {
                CreateTableFromGroup(group, doc);
            }
        }
        
        private void CreateTableFromGroup(Group group , Document doc)
        {
            //Create a table consists of three Columns(ID , Start Point , End Point)
            Paragraph empty = new Paragraph("");
            doc.Add(empty);
            Table table = new Table(6);
            Cell cell11 = new Cell(1, 1).AddHeaderCell("Beam ID");
            Cell cell12 = new Cell(1, 1).AddHeaderCell("Start Point");
            Cell cell13 = new Cell(1, 1).AddHeaderCell("End Point");
            Cell cell14 = new Cell(1, 1).AddHeaderCell("Span");
            Cell cell15 = new Cell(1, 1).AddHeaderCell("Mmax");
            Cell cell16 = new Cell(1, 1).AddHeaderCell("Vmax");
            List<Cell> cells = new List<Cell>();
            foreach (var beam in group.Beams)
            {
                Cell cell1 = new Cell(1, 1).AddOrdinaryCell(beam.Id.ToString());
                Cell cell2 = new Cell(1, 1).AddOrdinaryCell(beam.StartNode.Position.ToString());
                Cell cell3 = new Cell(1, 1).AddOrdinaryCell(beam.EndNode.Position.ToString());
                Cell cell4 = new Cell(1, 1).AddOrdinaryCell(beam.Length.ToString());
                Cell cell5 = new Cell(1, 1).AddOrdinaryCell(beam.CombinedSA.GetMaxMoment().ToString());
                Cell cell6 = new Cell(1, 1).AddOrdinaryCell(beam.CombinedSA.GetMaxShear().ToString());
                cells.Add(cell1);
                cells.Add(cell2);
                cells.Add(cell3);
                cells.Add(cell4);
                cells.Add(cell5);
                cells.Add(cell6);

            }
            table.AddCell(cell11);
            table.AddCell(cell12);
            table.AddCell(cell13);
            table.AddCell(cell14);
            table.AddCell(cell15);
            table.AddCell(cell16);
            foreach (var cell in cells)
            {
                table.AddCell(cell);
            }
            doc.Add(table);
            CreateHeader(doc, "Design Limit state:", 12, TextAlignment.LEFT);
            Paragraph combo = new Paragraph($"Combo: {group.DesignValues.Combo}");
            Paragraph moment = new Paragraph($"Md: {group.DesignValues.Md.ToString()} t.m");
            Paragraph shear = new Paragraph($"Vd: {group.DesignValues.Vd.ToString()} ton");
            doc.Add(combo);
            doc.Add(moment);
            doc.Add(shear);
            CreateHeader(doc, "Service Limit State", 12, TextAlignment.LEFT);
            Paragraph serviceCombo = new Paragraph($"Combo: {group.ServiceValue.Combo}");
            Paragraph span = new Paragraph($"Span: {group.ServiceValue.CriticalBeam.Length.ToString()} m");
            Paragraph wll = new Paragraph($"Load: {group.ServiceValue.WLL.ToString()} t/m'");
            doc.Add(serviceCombo);
            doc.Add(span);
            doc.Add(wll);
            CreateHeader(doc, "Design Checks", 12, TextAlignment.LEFT);
            CreateHeader(doc, "1-Check Local Buckling", 10, TextAlignment.LEFT, false);
            Paragraph webLocalBuckling = new Paragraph($"{group.DesignResult.WebLocalBuckling}");
            Paragraph flangeLocalBuckling = new Paragraph($"{group.DesignResult.FlangeLocalBuckling}");
            doc.Add(webLocalBuckling);
            doc.Add(flangeLocalBuckling);
            CreateHeader(doc, "2-Check Lateral Torsional Buckling", 10, TextAlignment.LEFT, false);
            Paragraph ltb = new Paragraph($"{group.DesignResult.Lu}");
            doc.Add(ltb);
            CreateHeader(doc, "3-Check Bending Stress", 10, TextAlignment.LEFT, false);
            Paragraph section = new Paragraph($"Section: {group.Section.Name}");
            Paragraph bending = new Paragraph($"fact= {group.DesignResult.Fbact} t/cm^2 < Fb= {group.DesignResult.Fball} t/cm^2");
            doc.Add(section);
            doc.Add(bending);
            CreateHeader(doc, "4-Check Shear Stress", 10, TextAlignment.LEFT, false);
            Paragraph shearStress = new Paragraph($"qact= {group.DesignResult.Qact} t/cm^2 < qall= {group.DesignResult.Qall} t/cm^2");
            doc.Add(shearStress);
            CreateHeader(doc, "5-Check Deflection", 10, TextAlignment.LEFT, false);
            Paragraph deflection = new Paragraph($"dact= {group.DesignResult.Dact} cm < dall= {group.DesignResult.Dall} cm");
            doc.Add(deflection);
            LineSeparator ls = new LineSeparator(new SolidLine());
            doc.Add(ls);
        }

    }
}
