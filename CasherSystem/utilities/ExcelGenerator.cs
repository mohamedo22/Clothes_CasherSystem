using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.IO;
using System.Windows;

namespace CasherSystem.utilities
{
    public class ExcelGenerator
    {
        public void ExportToExcel(decimal salesTotal, int salesCount, decimal purchasesTotal,
                                  decimal returnsTotal, decimal netProfit,
                                  DateTime startDate, DateTime endDate)
        {
            try
            {
                var sfd = new SaveFileDialog
                {
                    Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                    FileName = $"Sales_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Title = "Save Sales Report As",
                    DefaultExt = ".xlsx"
                };

                if (sfd.ShowDialog() != true)
                    return;

                var filePath = sfd.FileName;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("تقرير");
                    ws.View.RightToLeft = true;

                    int row = 1;
                    ws.Cells[row, 1].Value = "تقرير المبيعات للمالك";
                    ws.Cells[row, 1, row, 4].Merge = true;
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    ws.Cells[row, 1].Style.Font.Size = 18;
                    ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    row += 2;

                    ws.Cells[row, 1].Value = $"الفترة من {startDate:yyyy/MM/dd} إلى {endDate:yyyy/MM/dd}";
                    ws.Cells[row, 1, row, 4].Merge = true;
                    ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    row += 2;

                    void SectionHeader(string text)
                    {
                        ws.Cells[row, 1].Value = text;
                        ws.Cells[row, 1].Style.Font.Bold = true;
                        row++;
                    }

                    void KV(string key, object value)
                    {
                        ws.Cells[row, 1].Value = key;
                        ws.Cells[row, 2].Value = value;
                        row++;
                    }

                    SectionHeader("المبيعات:");
                    KV("إجمالي المبيعات:", $"{salesTotal:N2} جنيه");
                    KV("عدد الفواتير:", salesCount);
                    row++;

                    SectionHeader("المشتريات:");
                    KV("إجمالي المشتريات:", $"{purchasesTotal:N2} جنيه");
                    row++;

                    SectionHeader("المرتجعات:");
                    KV("إجمالي المرتجعات:", $"{returnsTotal:N2} جنيه");
                    row += 2;

                    ws.Cells[row, 1].Value = "صافي الربح:";
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    ws.Cells[row, 2].Value = $"{netProfit:N2} جنيه";
                    ws.Cells[row, 2].Style.Font.Bold = true;
                    ws.Cells[row, 2].Style.Font.Color.SetColor(netProfit >= 0 ? System.Drawing.Color.Green : System.Drawing.Color.Red);
                    row += 2;

                    ws.Cells[row, 1].Value = "مؤشرات إضافية:";
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    row++;
                    var averageSale = salesCount > 0 ? salesTotal / salesCount : 0;
                    var profitMargin = salesTotal > 0 ? (netProfit / salesTotal) * 100 : 0;
                    KV("متوسط قيمة الفاتورة:", $"{averageSale:N2} جنيه");
                    KV("هامش الربح:", $"{profitMargin:N2}%");

                    ws.Cells[1, 1, row, 2].AutoFitColumns();
                    using (var range = ws.Cells[1, 1, row - 1, 2])
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        package.SaveAs(fs);
                    }
                }

                MessageBox.Show($"تم حفظ التقرير في:\n{filePath}", "تم التصدير", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير Excel: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}


