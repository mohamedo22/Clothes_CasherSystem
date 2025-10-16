using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CasherSystem.utilities
{
    public class PDFGenerator
    {
        public void ExportToPdf(decimal salesTotal, int salesCount, decimal purchasesTotal,
                       decimal returnsTotal, decimal netProfit, string startDate,
                       string endDate)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = $"Sales_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                    Title = "Save Sales Report As",
                    DefaultExt = ".pdf"
                };

                if (saveFileDialog.ShowDialog() != true)
                    return;

                string filePath = saveFileDialog.FileName;

                var document = new Document(PageSize.A4.Rotate(), 20, 20, 30, 30);

                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);
                    document.Open();

                    // Arabic-capable fonts with correct encoding
                    BaseFont arabicBaseFont = GetArabicBaseFont();
                    Font titleFont = new Font(arabicBaseFont, 18, Font.BOLD);
                    Font headerFont = new Font(arabicBaseFont, 14, Font.BOLD);
                    Font normalFont = new Font(arabicBaseFont, 12, Font.NORMAL);

                    // Title (RTL)
                    AddRtlParagraph(document, "تقرير المبيعات للمالك", titleFont, Element.ALIGN_CENTER, 0f, 20f);
                    // Date range (RTL)
                    AddRtlParagraph(document, $"الفترة من {startDate} إلى {endDate}", normalFont, Element.ALIGN_CENTER, 0f, 30f);

                    // Data table (RTL)
                    PdfPTable table = new PdfPTable(2)
                    {
                        WidthPercentage = 90,
                        SpacingBefore = 20f,
                        SpacingAfter = 20f,
                        RunDirection = PdfWriter.RUN_DIRECTION_RTL
                    };
                    table.SetWidths(new float[] { 3f, 2f });

                    // Sales Section
                    AddTableRowRtl(table, "المبيعات:", string.Empty, headerFont);
                    AddTableRowRtl(table, "إجمالي المبيعات:", $"{salesTotal.ToString("N2")} جنيه", normalFont);
                    AddTableRowRtl(table, "عدد الفواتير:", salesCount.ToString(), normalFont);
                    AddTableRowRtl(table, string.Empty, string.Empty, normalFont);

                    // Purchases Section
                    AddTableRowRtl(table, "المشتريات:", string.Empty, headerFont);
                    AddTableRowRtl(table, "إجمالي المشتريات:", $"{purchasesTotal.ToString("N2")} جنيه", normalFont);
                    AddTableRowRtl(table, string.Empty, string.Empty, normalFont);

                    // Returns Section
                    AddTableRowRtl(table, "المرتجعات:", string.Empty, headerFont);
                    AddTableRowRtl(table, "إجمالي المرتجعات:", $"{returnsTotal.ToString("N2")} جنيه", normalFont);

                    document.Add(table);

                    // Net Profit Section (RTL)
                    BaseColor profitColor = netProfit >= 0 ? BaseColor.GREEN : BaseColor.RED;
                    Font profitFont = new Font(arabicBaseFont, 14, Font.BOLD, profitColor);
                    AddRtlParagraph(document, "صافي الربح:", profitFont, Element.ALIGN_RIGHT, 0f, 10f);
                    AddRtlParagraph(document, $"{netProfit.ToString("N2")} جنيه", profitFont, Element.ALIGN_RIGHT, 0f, 20f);

                    // Additional Metrics (RTL)
                    AddRtlParagraph(document, "مؤشرات إضافية:", headerFont, Element.ALIGN_RIGHT, 0f, 10f);
                    decimal averageSale = salesCount > 0 ? salesTotal / salesCount : 0;
                    decimal profitMargin = salesTotal > 0 ? (netProfit / salesTotal) * 100 : 0;

                    PdfPTable metricsTable = new PdfPTable(2)
                    {
                        WidthPercentage = 60,
                        RunDirection = PdfWriter.RUN_DIRECTION_RTL
                    };
                    metricsTable.HorizontalAlignment = Element.ALIGN_RIGHT;
                    AddTableRowRtl(metricsTable, "متوسط قيمة الفاتورة:", $"{averageSale.ToString("N2")} جنيه", normalFont);
                    AddTableRowRtl(metricsTable, "هامش الربح:", $"{profitMargin.ToString("N2")}%", normalFont);

                    document.Add(metricsTable);

                    // Footer (RTL)
                    AddRtlParagraph(document, $"تم الإنشاء: {DateTime.Now:yyyy/MM/dd HH:mm}", normalFont, Element.ALIGN_CENTER, 40f, 0f);

                    document.Close();
                }

                MessageBox.Show($"تم حفظ التقرير بنجاح في:\n{filePath}", "تم التصدير بنجاح",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير PDF: {ex.Message}", "خطأ",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Helper method to get Arabic font (IDENTITY_H encoding for proper shaping)
        private BaseFont GetArabicBaseFont()
        {
            try
            {
                // Try common Arabic font paths
                string[] possibleFontPaths = {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "tahoma.ttf"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arialuni.ttf")
        };

                foreach (string fontPath in possibleFontPaths)
                {
                    if (File.Exists(fontPath))
                    {
                        return BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    }
                }

                // If no font found, use default (may not support Arabic)
                return BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            }
            catch
            {
                // Fallback to default font
                return BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            }
        }

        // Helper method to add RTL table rows
        private void AddTableRowRtl(PdfPTable table, string text, string value, Font font)
        {
            var cell1 = new PdfPCell(new Phrase(text, font))
            {
                Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Padding = 5f,
                RunDirection = PdfWriter.RUN_DIRECTION_RTL
            };
            var cell2 = new PdfPCell(new Phrase(value, font))
            {
                Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5f,
                RunDirection = PdfWriter.RUN_DIRECTION_RTL
            };
            table.AddCell(cell1);
            table.AddCell(cell2);
        }

        // Helper to add RTL paragraph-like blocks using a 1-column table
        private void AddRtlParagraph(Document doc, string text, Font font, int alignment, float spacingBefore, float spacingAfter)
        {
            PdfPTable wrapper = new PdfPTable(1)
            {
                WidthPercentage = 100,
                SpacingBefore = spacingBefore,
                SpacingAfter = spacingAfter,
                RunDirection = PdfWriter.RUN_DIRECTION_RTL
            };
            PdfPCell cell = new PdfPCell(new Phrase(text, font))
            {
                Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = alignment,
                RunDirection = PdfWriter.RUN_DIRECTION_RTL
            };
            wrapper.AddCell(cell);
            doc.Add(wrapper);
        }
        
    }
}
