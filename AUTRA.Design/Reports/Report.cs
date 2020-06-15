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
using System.Reflection;

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

        public void Create(string fileName,List<Group> secGroups,List<Group> mainGroups,Group columnGroups)
        {
            string fullpath = Path.Combine(FolderPath, fileName);

            using (var writer = new PdfWriter(fullpath))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    Document doc = new Document(pdf);
                    ReportForBeams("Secondary Beams", secGroups, doc);
                    ReportForBeams("Main Beams", mainGroups, doc);
                    ReportForColumns("Columns", columnGroups, doc);
                }
            }
        }
        private void ReportForColumns(string title , Group group , Document doc)
        {
            CreateHeader(doc, title);
            CreateTableFromColumnGroup(group, doc);
        }
        private void CreateTableFromColumnGroup(Group group , Document doc)
        {
            doc.AddParagraph("");
            Table table = new Table(5);
            table.AddCell().AddHeader("Column ID");
            table.AddCell().AddHeader("Start Point");
            table.AddCell().AddHeader("End Point");
            table.AddCell().AddHeader("Height (m)");
            table.AddCell().AddHeader("Nmax (ton)");
            foreach (var col in group.Elements)
            {
                table.AddCell().AddParagraph(col.Id.ToString());
                table.AddCell().AddParagraph(col.StartNode.Position.ToString());
                table.AddCell().AddParagraph(col.EndNode.Position.ToString());
                table.AddCell().AddParagraph(col.Length.Round().ToString());
                table.AddCell().AddParagraph(col.CombinedSA.GetMaxCompression().Round().ToString());
            }
            table.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            doc.Add(table);
            CreateHeader(doc, "Design Limit state:", 12, TextAlignment.LEFT);
            doc.AddParagraph($"Combo: {group.DesignValues.Combo}");
            doc.AddParagraph($"Nd: {group.DesignValues.Nd.Round().ToString()} ton");
            CreateHeader(doc, "1-Check Local Buckling", 10, TextAlignment.LEFT, false);
            doc.AddParagraph($"{group.DesignResult.WebLocalBuckling}");
            doc.AddParagraph($"{group.DesignResult.FlangeLocalBuckling}");
            CreateHeader(doc, "2-Check Normal Stress", 10, TextAlignment.LEFT, false);
            doc.AddParagraph($"Section: {group.Section.Name}");
            doc.AddParagraph(group.DesignResult.Lambda);
            doc.AddParagraph($"fc= {Math.Abs(group.DesignResult.Fcact.Round())} t/cm^2 < Fc= {group.DesignResult.Fcall.Round()} t/cm^2");
            doc.AddLineSeparator();
        }
        private void ReportForBeams(string title,List<Group> groups, Document doc)
        {
            CreateHeader( doc,title);
            CreateTableFromBeamGroups(groups,doc);
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
        private void CreateTableFromBeamGroups(List<Group> groups,Document doc)
        {
            foreach (var group in groups)
            {
                CreateTableFromBeamGroup(group, doc);
            }
        }
        private void CreateTableFromBeamGroup(Group group , Document doc)
        {
            Paragraph empty = new Paragraph("");
            doc.Add(empty);
            Table table = new Table(6);
            table.AddCell().AddHeader("Beam ID");
            table.AddCell().AddHeader("Start Point");
            table.AddCell().AddHeader("End Point");
            table.AddCell().AddHeader("Span (m)");
            table.AddCell().AddHeader("Mmax (t.m)");
            table.AddCell().AddHeader("Vmax (ton)");
            foreach (var beam in group.Elements)
            {
                table.AddCell().AddParagraph(beam.Id.ToString());
                table.AddCell().AddParagraph(beam.StartNode.Position.ToString());
                table.AddCell().AddParagraph(beam.EndNode.Position.ToString());
                table.AddCell().AddParagraph(beam.Length.Round().ToString());
                table.AddCell().AddParagraph(beam.CombinedSA.GetMaxMoment().Round().ToString());
                table.AddCell().AddParagraph(beam.CombinedSA.GetMaxShear().Round().ToString());
            }
            table.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            doc.Add(table);
            CreateHeader(doc, "Design Limit state:", 12, TextAlignment.LEFT);
            doc.AddParagraph($"Combo: {group.DesignValues.Combo}");
            doc.AddParagraph($"Md: {group.DesignValues.Md.Round().ToString()} t.m");
            doc.AddParagraph($"Vd: {group.DesignValues.Vd.Round().ToString()} ton");
            CreateHeader(doc, "Service Limit State", 12, TextAlignment.LEFT);
            doc.AddParagraph($"Combo: {group.ServiceValue.Combo}");
            doc.AddParagraph($"Span: {group.ServiceValue.CriticalBeam.Length.Round().ToString()} m");
            doc.AddParagraph($"Load: {group.ServiceValue.WLL.Round().ToString()} t/m'");
            CreateHeader(doc, "Design Checks", 12, TextAlignment.LEFT);
            CreateHeader(doc, "1-Check Local Buckling", 10, TextAlignment.LEFT, false);
            doc.AddParagraph($"{group.DesignResult.WebLocalBuckling}");
            doc.AddParagraph($"{group.DesignResult.FlangeLocalBuckling}");
            CreateHeader(doc, "2-Check Lateral Torsional Buckling", 10, TextAlignment.LEFT, false);
            doc.AddParagraph($"{group.DesignResult.Lu}");
            CreateHeader(doc, "3-Check Bending Stress", 10, TextAlignment.LEFT, false);
            doc.AddParagraph($"Section: {group.Section.Name}");
            doc.AddParagraph($"fact= {group.DesignResult.Fbact.Round()} t/cm^2 < Fb= {group.DesignResult.Fball.Round()} t/cm^2");
            CreateHeader(doc, "4-Check Shear Stress", 10, TextAlignment.LEFT, false);
            doc.AddParagraph($"qact= {group.DesignResult.Qact.Round()} t/cm^2 < qall= {group.DesignResult.Qall.Round()} t/cm^2");
            CreateHeader(doc, "5-Check Deflection", 10, TextAlignment.LEFT, false);
            doc.AddParagraph($"dact= {group.DesignResult.Dact.Round()} cm < dall= {group.DesignResult.Dall.Round()} cm");
            CreateConnectionReportForBeamGroup(group, doc);
            doc.AddLineSeparator();
        }
        private void CreateConnectionReportForBeamGroup(Group group, Document doc)
        {
            CreateHeader(doc, "Group Connection Design (Simple Shear Plate Connection)", 12, TextAlignment.LEFT);
            doc.AddImage(Assembly.GetExecutingAssembly().GoToPath(@"Resources\Images\shearPlate Connection.jpg"));
            CreateHeader(doc, "1-Bolts Design", 10, TextAlignment.LEFT, false);
            doc.AddParagraph($"Bolts: M{group.Connection.Bolt.Dia*10} of Grade {group.Connection.Bolt.Grade.Name}");
            doc.AddParagraph($"Vd= {group.DesignValues.Vd.Round()} ton");
            doc.AddParagraph($"Rleast= {group.Connection.Rleast.Round()} ton");
            doc.AddParagraph($"N= {group.Connection.N} with Pitch= {group.Connection.Pitch} mm & Full Layout: ({group.Connection.ToString()})");
            CreateHeader(doc, "2-Stresses Induced in Fillet Weld Lines at Plane(1-1)", 10, TextAlignment.LEFT, false);
            doc.AddParagraph($"{group.Connection.Plane11Check}");
            CreateHeader(doc, "3-Stresses Induced in Fillet Weld Lines at Plane(2-2)", 10, TextAlignment.LEFT, false);
            doc.AddParagraph($"{group.Connection.Plane22Check}");
            CreateHeader(doc, "4-Check Thickness of Plate", 10, TextAlignment.LEFT, false);
            doc.AddParagraph($"{group.Connection.PlateThicknessCheck}");
            doc.AddParagraph($"Plate Layout => L = {group.Connection.Length} mm & tp = {group.Connection.Tp} mm & Sw = {group.Connection.Sw} mm");
        }
    }
}
