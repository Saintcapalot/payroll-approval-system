using PayrollApprovalSystem.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PayrollApprovalSystem.Api.Services;

public class PayslipPdfService
{
    public byte[] GeneratePdf(Employee employee, Payroll payroll, string approvalStatus, DateTime generatedAt)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Column(column =>
                    {
                        column.Item().Text("Payslip").SemiBold().FontSize(24);
                        column.Item().Text($"Payroll Period: {payroll.Month:D2}/{payroll.Year}");
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Spacing(12);

                        column.Item().Text("Employee Information").SemiBold().FontSize(16);
                        column.Item().Text($"Name: {employee.FirstName} {employee.LastName}");
                        column.Item().Text($"Email: {employee.Email}");
                        column.Item().Text($"Employee ID: {employee.Id}");

                        column.Item().PaddingTop(8).Text("Payroll Summary").SemiBold().FontSize(16);
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Element(CellStyle).Text("Base Salary");
                            table.Cell().Element(CellStyle).AlignRight().Text(payroll.BaseSalary.ToString("N2"));

                            table.Cell().Element(CellStyle).Text("Bonus");
                            table.Cell().Element(CellStyle).AlignRight().Text(payroll.Bonus.ToString("N2"));

                            table.Cell().Element(CellStyle).Text("Deductions");
                            table.Cell().Element(CellStyle).AlignRight().Text(payroll.Deductions.ToString("N2"));

                            table.Cell().Element(CellStyle).Text("Net Salary").SemiBold();
                            table.Cell().Element(CellStyle).AlignRight().Text(payroll.TotalAmount.ToString("N2")).SemiBold();
                        });

                        column.Item().PaddingTop(8).Text($"Approval Status: {approvalStatus}");
                        column.Item().Text($"Generated Date: {generatedAt:yyyy-MM-dd HH:mm:ss} UTC");
                    });

                page.Footer()
                    .AlignCenter()
                    .Text("Payroll Approval System Demo")
                    .FontSize(10)
                    .FontColor(Colors.Grey.Darken1);
            });
        }).GeneratePdf();
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(6);
    }
}
