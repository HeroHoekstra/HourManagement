using HoursSharp.Data.Repository;
using HoursSharp.Models;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;

namespace HoursSharp.Controllers;

public class PDFController : Controller
{
    private readonly TimeSheetRepository _tsRepository;
    private readonly SheetDayRepository _dayRepository;
    
    private readonly TimeSheetService _tsService;

    public PDFController(TimeSheetRepository tsRepository, SheetDayRepository dayRepository, TimeSheetService tsService)
    {
        _tsRepository = tsRepository;
        _dayRepository = dayRepository;
        
        _tsService = tsService;
    }
    
    [HttpPost("/api/pdf/download/{id}")]
    public IActionResult Download([FromForm] SheetDay[] sheetDays, string id)
    {
        List<float> totalHours = _tsService.getHours(id);
        User? user = _tsRepository.GetUser(id);
        
        using var memoryStream = new MemoryStream();
        using (var writer = new PdfWriter(memoryStream))
        {
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf);
            
            var normal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            document.Add(new Paragraph($"Uren brief {sheetDays[0].Date.ToString("MM/yy")} - {user.FullName()}"));

            document.SetFontSize(8);
            
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 15, 15, 60 }))
                .SetWidth(UnitValue.CreatePercentValue(100));
            
            table.AddCell(new Paragraph("Datum").SetFont(bold));
            table.AddCell(new Paragraph("Extra uren").SetFont(bold));
            table.AddCell(new Paragraph("Uren").SetFont(bold));
            table.AddCell(new Paragraph("Omschrijving").SetFont(bold));
            
            table.SetFont(normal);

            foreach (SheetDay sheetDay in sheetDays)
            {
                string extraHours = sheetDay.ExtraHours == 0 ? "" : $"{sheetDay.ExtraHours}";
                string hours = sheetDay.Hours == 0 ? "" : $"{sheetDay.Hours}";
                
                table.AddCell(sheetDay.Date.ToString("dd/MM/yyyy"));
                table.AddCell(extraHours);
                table.AddCell(hours);
                table.AddCell($"{sheetDay.Description}");
            }
            
            document.Add(table);
            
            document.Add(new Paragraph(""));
            
            table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 15, 15, 15, 45 }))
                .SetWidth(UnitValue.CreatePercentValue(100));

            table.AddCell("");
            table.AddCell($"{totalHours[1]}");
            table.AddCell($"{totalHours[0]}");
            table.AddCell($"{totalHours[2]}");
            
            document.Add(table);
        }
        
        return File(
            memoryStream.ToArray(), 
            "application/pdf", 
            $"{sheetDays[0].Date.ToString("MM/yy").Replace("/", "-")} {user.FullName()}.pdf"
        );
    }
}