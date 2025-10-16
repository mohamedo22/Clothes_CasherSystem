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
                // Show Save File Dialog for user to choose location
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                saveFileDialog.FileName = $"Sales_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                saveFileDialog.Title = "Save Sales Report As";
                saveFileDialog.DefaultExt = ".pdf";

                // Show dialog and check if user selected a location
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    // Create the document
                    Document document = new Document(PageSize.A4.Rotate(), 20, 20, 30, 30);

                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        PdfWriter writer = PdfWriter.GetInstance(document, fs);
                        document.Open();

                        // Handle Arabic font
                        BaseFont arabicBaseFont = GetArabicBaseFont();
                        Font arabicTitleFont = new Font(arabicBaseFont, 18, Font.BOLD);
                        Font arabicHeaderFont = new Font(arabicBaseFont, 14, Font.BOLD);
                        Font arabicNormalFont = new Font(arabicBaseFont, 12, Font.NORMAL);

                        // Title
                        Paragraph title = new Paragraph("تقرير المبيعات للمالك", arabicTitleFont);
                        title.Alignment = Element.ALIGN_CENTER;
                        title.SpacingAfter = 20f;
                        document.Add(title);

                        // Date Range
                        Paragraph dateRange = new Paragraph($"الفترة من {startDate} إلى {endDate}", arabicNormalFont);
                        dateRange.Alignment = Element.ALIGN_CENTER;
                        dateRange.SpacingAfter = 30f;
                        document.Add(dateRange);

                        // Create table for data
                        PdfPTable table = new PdfPTable(2);
                        table.WidthPercentage = 90;
                        table.SpacingBefore = 20f;
                        table.SpacingAfter = 20f;

                        float[] columnWidths = { 3f, 2f };
                        table.SetWidths(columnWidths);

                        // Sales Section
                        AddTableRow(table, "المبيعات:", "", arabicHeaderFont);
                        AddTableRow(table, "إجمالي المبيعات:", $"{salesTotal.ToString("N2")} ر.س", arabicNormalFont);
                        AddTableRow(table, "عدد الفواتير:", salesCount.ToString(), arabicNormalFont);
                        AddTableRow(table, "", "", arabicNormalFont); // Spacing

                        // Purchases Section
                        AddTableRow(table, "المشتريات:", "", arabicHeaderFont);
                        AddTableRow(table, "إجمالي المشتريات:", $"{purchasesTotal.ToString("N2")} ر.س", arabicNormalFont);
                        AddTableRow(table, "", "", arabicNormalFont); // Spacing

                        // Returns Section
                        AddTableRow(table, "المرتجعات:", "", arabicHeaderFont);
                        AddTableRow(table, "إجمالي المرتجعات:", $"{returnsTotal.ToString("N2")} ر.س", arabicNormalFont);

                        document.Add(table);

                        // Net Profit Section
                        BaseColor profitColor = netProfit >= 0 ? BaseColor.GREEN : BaseColor.RED;
                        Font profitFont = new Font(arabicBaseFont, 14, Font.BOLD, profitColor);

                        Paragraph profitTitle = new Paragraph("صافي الربح:", profitFont);
                        profitTitle.Alignment = Element.ALIGN_RIGHT;
                        profitTitle.SpacingAfter = 10f;
                        document.Add(profitTitle);

                        Paragraph profitValue = new Paragraph($"{netProfit.ToString("N2")} ر.س", profitFont);
                        profitValue.Alignment = Element.ALIGN_RIGHT;
                        profitValue.SpacingAfter = 20f;
                        document.Add(profitValue);

                        // Additional Metrics
                        Paragraph metricsTitle = new Paragraph("مؤشرات إضافية:", arabicHeaderFont);
                        metricsTitle.Alignment = Element.ALIGN_RIGHT;
                        metricsTitle.SpacingAfter = 10f;
                        document.Add(metricsTitle);

                        decimal averageSale = salesCount > 0 ? salesTotal / salesCount : 0;
                        decimal profitMargin = salesTotal > 0 ? (netProfit / salesTotal) * 100 : 0;

                        PdfPTable metricsTable = new PdfPTable(2);
                        metricsTable.WidthPercentage = 60;
                        metricsTable.HorizontalAlignment = Element.ALIGN_RIGHT;

                        AddTableRow(metricsTable, "متوسط قيمة الفاتورة:", $"{averageSale.ToString("N2")} ر.س", arabicNormalFont);
                        AddTableRow(metricsTable, "هامش الربح:", $"{profitMargin.ToString("N2")}%", arabicNormalFont);

                        document.Add(metricsTable);

                        // Footer
                        Paragraph footer = new Paragraph($"تم الإنشاء: {DateTime.Now.ToString("yyyy/MM/dd HH:mm")}", arabicNormalFont);
                        footer.Alignment = Element.ALIGN_CENTER;
                        footer.SpacingBefore = 40f;
                        document.Add(footer);

                        document.Close();
                    }

                    MessageBox.Show($"تم حفظ التقرير بنجاح في:\n{filePath}", "تم التصدير بنجاح",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير PDF: {ex.Message}", "خطأ",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Helper method to get Arabic font (safer implementation)
        private BaseFont GetArabicBaseFont()
        {
            try
            {
                // Try common Arabic font paths
                string[] possibleFontPaths = {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "tahoma.ttf"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf")
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

        // Helper method to add table rows
        private void AddTableRow(PdfPTable table, string text, string value, Font font)
        {
            PdfPCell cell1 = new PdfPCell(new Phrase(text, font));
            cell1.Border = PdfPCell.NO_BORDER;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Padding = 5f;

            PdfPCell cell2 = new PdfPCell(new Phrase(value, font));
            cell2.Border = PdfPCell.NO_BORDER;
            cell2.HorizontalAlignment = Element.ALIGN_LEFT;
            cell2.Padding = 5f;

            table.AddCell(cell1);
            table.AddCell(cell2);
        }
        
    }
}
