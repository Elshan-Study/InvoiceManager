using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using InvoiceManager.DTOs.InvoiceDto;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using PdfDocument = QuestPDF.Fluent.Document;
using WordDocument = DocumentFormat.OpenXml.Wordprocessing.Document;

namespace InvoiceManager.Services;

public class InvoiceExportService : IInvoiceExportService
{
    public async Task<byte[]> ExportAsync(InvoiceResponseDto invoice, InvoiceExportFormat format)
    {
        return format switch
        {
            InvoiceExportFormat.Docx => GenerateDocx(invoice),
            InvoiceExportFormat.Pdf => GeneratePdf(invoice),
            _ => throw new NotSupportedException()
        };
    }

    private byte[] GenerateDocx(InvoiceResponseDto invoice)
    {
        using var ms = new MemoryStream();
        using var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document);

        var mainPart = doc.AddMainDocumentPart();
        mainPart.Document = new WordDocument(new Body());

        var body = mainPart.Document.Body!;

        body.Append(new Paragraph(new Run(new Text($"Invoice #{invoice.Id}"))));
        body.Append(new Paragraph(new Run(new Text($"Period: {invoice.StartDate:d} - {invoice.EndDate:d}"))));
        body.Append(new Paragraph(new Run(new Text($"Total: {invoice.TotalSum:C}"))));
        body.Append(new Paragraph(new Run(new Text(" "))));
        body.Append(new Paragraph(new Run(new Text("Items:"))));

        foreach (var row in invoice.Rows)
        {
            body.Append(new Paragraph(
                new Run(new Text($"{row.Service} | Qty: {row.Quantity} | Rate: {row.Rate} | Sum: {row.Sum}"))
            ));
        }

        mainPart.Document.Save();
        return ms.ToArray();
    }

    private byte[] GeneratePdf(InvoiceResponseDto invoice)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return PdfDocument.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);

                page.Content().Column(col =>
                {
                    col.Item().Text($"Invoice #{invoice.Id}").FontSize(20).Bold();
                    col.Item().Text($"Period: {invoice.StartDate:d} - {invoice.EndDate:d}");
                    col.Item().Text($"Total: {invoice.TotalSum:C}");
                    col.Item().PaddingVertical(10);

                    foreach (var row in invoice.Rows)
                    {
                        col.Item().Text($"{row.Service} | Qty: {row.Quantity} | Rate: {row.Rate} | Sum: {row.Sum}");
                    }
                });
            });
        }).GeneratePdf();
    }
}