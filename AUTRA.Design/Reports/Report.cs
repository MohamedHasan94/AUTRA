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
using iText.Kernel.Events;
using iText.Kernel.Geom;
using AUTRA.Tekla;

namespace AUTRA.Design
{
   public class Report
    {
        //Responsible for the IO operations
        public string FolderPath { get; set; }
        public ProjectProperties Project { get; set; }
        public string Owner { get; set; }
        public Report(string folderPath ,ProjectProperties project ,string owner)
        {
            FolderPath = folderPath;
            Project = project;
        }

        public void Create(string fileName,List<Group> secGroups,List<Group> mainGroups,Group columnGroups)
        {
            string fullpath = System.IO.Path.Combine(FolderPath, fileName);
            using (var writer = new PdfWriter(fullpath))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    Document doc = new Document(pdf);
                    doc.SetMargins(50, 40, 50, 40);
                    pdf.AddEventHandler(PdfDocumentEvent.START_PAGE, new Header(doc,Owner,  ImageDataFactory.Create(Assembly.GetExecutingAssembly().GoToPath(@"Resources\Images\AUTRA.PNG"))));
                    Footer foot = new Footer(doc);
                    pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, foot);
                    CoverPage(doc);
                    CreateTableOfContents(doc);
                    PageSize ps = PageSize.A4;
                    ReportForBeams("Secondary Beams", secGroups, doc);
                    ReportForBeams("Main Beams", mainGroups, doc);
                    ReportForColumns("Columns", columnGroups, doc);
                    foot.writeTotal(pdf);
                }
            }
        }
        private void ReportForColumns(string title , Group group , Document doc)
        {
            doc.AddHeader(title);
            CreateTableFromColumnGroup(group, doc);
        }
        private void CreateTableFromColumnGroup(Group group , Document doc)
        {
            doc.AddParagraph("");
            Table table = new Table(5);
            table.AddCell().AddHeaderCell("Column ID");
            table.AddCell().AddHeaderCell("Start Point");
            table.AddCell().AddHeaderCell("End Point");
            table.AddCell().AddHeaderCell("Height (m)");
            table.AddCell().AddHeaderCell("Nmax (ton)");
            foreach (var col in group.Elements)
            {
                table.AddCell().AddCell(col.Id.ToString());
                table.AddCell().AddCell(col.StartNode.Position.ToString());
                table.AddCell().AddCell(col.EndNode.Position.ToString());
                table.AddCell().AddCell(col.Length.Round().ToString());
                table.AddCell().AddCell(col.CombinedSA.GetMaxCompression().Round().ToString());
            }
            table.SetHorizontalAlignment(HorizontalAlignment.LEFT);
            doc.Add(table);
            doc.AddHeader( "Design Limit state:", 12, TextAlignment.LEFT);
            doc.AddParagraph($"Combo: {group.DesignValues.Combo}");
            doc.AddParagraph($"Nd: {group.DesignValues.Nd.Round().ToString()} ton");
            doc.AddHeader( "1-Check Local Buckling", 10, TextAlignment.LEFT);
            doc.AddParagraph($"{group.DesignResult.WebLocalBuckling}");
            doc.AddParagraph($"{group.DesignResult.FlangeLocalBuckling}");
            doc.AddHeader( "2-Check Normal Stress", 10, TextAlignment.LEFT);
            doc.AddParagraph($"Section: {group.Section.Name}");
            doc.AddParagraph(group.DesignResult.Lambda);
            doc.AddParagraph($"fc= {Math.Abs(group.DesignResult.Fcact.Round())} t/cm^2 < Fc= {group.DesignResult.Fcall.Round()} t/cm^2");
            doc.AddLineSeparator();
        }
        private void ReportForBeams(string title,List<Group> groups, Document doc)
        {
            doc.AddHeader(title);
            CreateTableFromBeamGroups(groups,doc);
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
            table.AddCell().AddHeaderCell("Beam ID");
            table.AddCell().AddHeaderCell("Start Point");
            table.AddCell().AddHeaderCell("End Point");
            table.AddCell().AddHeaderCell("Span (m)");
            table.AddCell().AddHeaderCell("Mmax (t.m)");
            table.AddCell().AddHeaderCell("Vmax (ton)");
            foreach (var beam in group.Elements)
            {
                table.AddCell().AddCell(beam.Id.ToString());
                table.AddCell().AddCell(beam.StartNode.Position.ToString());
                table.AddCell().AddCell(beam.EndNode.Position.ToString());
                table.AddCell().AddCell(beam.Length.Round().ToString());
                table.AddCell().AddCell(beam.CombinedSA.GetMaxMoment().Round().ToString());
                table.AddCell().AddCell(beam.CombinedSA.GetMaxShear().Round().ToString());
            }
            table.SetHorizontalAlignment(HorizontalAlignment.LEFT);
            doc.Add(table);
            doc.AddHeader("Design Limit state:", 12, TextAlignment.LEFT);
            doc.AddParagraph($"Combo: {group.DesignValues.Combo}");
            doc.AddParagraph($"Md: {group.DesignValues.Md.Round().ToString()} t.m");
            doc.AddParagraph($"Vd: {group.DesignValues.Vd.Round().ToString()} ton");
            doc.AddHeader( "Service Limit State", 12, TextAlignment.LEFT);
            doc.AddParagraph($"Combo: {group.ServiceValue.Combo}");
            doc.AddParagraph($"Span: {group.ServiceValue.CriticalBeam.Length.Round().ToString()} m");
            doc.AddParagraph($"Load: {group.ServiceValue.WLL.Round().ToString()} t/m'");
            doc.AddHeader( "Design Checks", 12, TextAlignment.LEFT);
            doc.AddHeader( "1-Check Local Buckling", 10, TextAlignment.LEFT);
            doc.AddParagraph($"{group.DesignResult.WebLocalBuckling}");
            doc.AddParagraph($"{group.DesignResult.FlangeLocalBuckling}");
            doc.AddHeader("2-Check Lateral Torsional Buckling", 10, TextAlignment.LEFT);
            doc.AddParagraph($"{group.DesignResult.Lu}");
            doc.AddHeader("3-Check Bending Stress", 10, TextAlignment.LEFT);
            doc.AddParagraph($"Section: {group.Section.Name}");
            doc.AddParagraph($"fact= {group.DesignResult.Fbact.Round()} t/cm^2 < Fb= {group.DesignResult.Fball.Round()} t/cm^2");
            doc.AddHeader("4-Check Shear Stress", 10, TextAlignment.LEFT);
            doc.AddParagraph($"qact= {group.DesignResult.Qact.Round()} t/cm^2 < qall= {group.DesignResult.Qall.Round()} t/cm^2");
            doc.AddHeader("5-Check Deflection", 10, TextAlignment.LEFT);
            doc.AddParagraph($"dact= {group.DesignResult.Dact.Round()} cm < dall= {group.DesignResult.Dall.Round()} cm");
            CreateConnectionReportForBeamGroup(group, doc);
            doc.AddLineSeparator();
        }
        private void CreateConnectionReportForBeamGroup(Group group, Document doc)
        {
            doc.AddImage(Assembly.GetExecutingAssembly().GoToPath(@"Resources\Images\shearPlate Connection.jpg")).Scale(0.85f,0.85f);
            doc.AddHeader( "Group Connection Design (Simple Shear Plate Connection)", 12, TextAlignment.LEFT);
            doc.AddHeader("1-Bolts Design", 10, TextAlignment.LEFT);
            doc.AddParagraph($"Bolts: M{group.Connection.Bolt.Dia*10} of Grade {group.Connection.Bolt.Grade.Name}");
            doc.AddParagraph($"Vd= {group.DesignValues.Vd.Round()} ton");
            doc.AddParagraph($"Rleast= {group.Connection.Rleast.Round()} ton");
            doc.AddParagraph($"N= {group.Connection.N} with Pitch= {group.Connection.Pitch} mm & Full Layout: ({group.Connection.ToString()})");
            doc.AddHeader( "2-Stresses Induced in Fillet Weld Lines at Plane(1-1)", 10, TextAlignment.LEFT);
            doc.AddParagraph($"{group.Connection.Plane11Check}");
            doc.AddHeader( "3-Stresses Induced in Fillet Weld Lines at Plane(2-2)", 10, TextAlignment.LEFT);
            doc.AddParagraph($"{group.Connection.Plane22Check}");
            doc.AddHeader( "4-Check Thickness of Plate", 10, TextAlignment.LEFT);
            doc.AddParagraph($"{group.Connection.PlateThicknessCheck}");
            doc.AddParagraph($"Plate Layout => L = {group.Connection.Length} mm & tp = {group.Connection.Tp} mm & Sw = {group.Connection.Sw} mm");
        }
        private void CreateTableOfContents(Document doc)
        {
            List<string> beams = new List<string> { "Design For Flexural and shear", "Design For serviceability", "Connections Design" };
            doc.AddHeader("Table of Contents");
            //Add Block for secondary beams
            doc.AddList("1-Secondary Beams", beams);
            //Add Block for Main Beams
            doc.AddList("2-Main Beams", beams);
            //Add Block for Secondary Beams
            doc.AddList("3-Columns", new List<string> { "Design For Normal Stress" });
            doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
        }
        private void CoverPage(Document doc)
        {
            doc.AddHeader($"Design Calculation Sheet for {Project.Name}");
            doc.AddCustomParagraph($"Designer: {Project.Designer}",12);
            doc.AddCustomParagraph($"Location: {Project.Location}", 12);
            doc.AddCustomParagraph($"City: {Project.City}", 12);
            doc.AddCustomParagraph($"Country: {Project.Country}", 12);
            doc.AddCustomParagraph($"Date: {DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}", 12);
            doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
        }
    }
}
