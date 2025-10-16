using CasherSystem.Models;
using SkiaSharp;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using ZXing;
using ZXing.Common;

public class InvoicePrinter
{
    private Sale _sale;
    private Font _titleFont;
    private Font _normalFont;
    private Font _boldFont;
    private Font _smallFont;
    private Font _barcodeFont;


    public void PrintStoreOwnerReport(Graphics graphics, decimal salesTotal, int salesCount,
                                decimal purchasesTotal, decimal returnsTotal, decimal netProfit,
                                string startDate, string endDate, int width)
    {
        // Fonts
        var titleFont = new Font(new FontFamily("Arial"), 14, System.Drawing.FontStyle.Bold);
        var headerFont = new Font(new FontFamily("Arial"), 12, System.Drawing.FontStyle.Bold);
        var normalFont = new Font(new FontFamily("Arial"), 11, System.Drawing.FontStyle.Regular);
        var totalFont = new Font(new FontFamily("Arial"), 12, System.Drawing.FontStyle.Bold);

        // Format for right alignment (Arabic)
        var rightFormat = new StringFormat
        {
            Alignment = StringAlignment.Far,
            LineAlignment = StringAlignment.Center
        };

        var leftFormat = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center
        };

        var centerFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        int yPos = 50; // Starting position

        // Report Title
        graphics.DrawString("تقرير المبيعات للمالك", titleFont, Brushes.Black, width / 2, yPos, centerFormat);
        yPos += 40;

        // Date Range
        graphics.DrawString($"الفترة من {startDate} إلى {endDate}", normalFont, Brushes.Black, width / 2, yPos, centerFormat);
        yPos += 30;

        // Separator line
        graphics.DrawLine(new Pen(Color.Black, 1), 50, yPos, width - 50, yPos);
        yPos += 20;

        // Sales Section
        graphics.DrawString("المبيعات:", headerFont, Brushes.Black, width - 60, yPos, rightFormat);
        yPos += 25;

        graphics.DrawString($"إجمالي المبيعات: {salesTotal.ToString("N2")} جنيه", normalFont, Brushes.Black, width - 60, yPos, rightFormat);
        yPos += 20;

        graphics.DrawString($"عدد الفواتير: {salesCount}", normalFont, Brushes.Black, width - 60, yPos, rightFormat);
        yPos += 30;

        // Purchases Section
        graphics.DrawString("المشتريات:", headerFont, Brushes.Black, width - 60, yPos, rightFormat);
        yPos += 25;

        graphics.DrawString($"إجمالي المشتريات: {purchasesTotal.ToString("N2")} جنيه", normalFont, Brushes.Black, width - 60, yPos, rightFormat);
        yPos += 30;

        // Returns Section
        graphics.DrawString("المرتجعات:", headerFont, Brushes.Black, width - 60, yPos, rightFormat);
        yPos += 25;

        graphics.DrawString($"إجمالي المرتجعات: {returnsTotal.ToString("N2")} جنيه", normalFont, Brushes.Black, width - 60, yPos, rightFormat);
        yPos += 40;

        // Separator line
        graphics.DrawLine(new Pen(Color.Black, 2), 50, yPos, width - 50, yPos);
        yPos += 30;

        // Net Profit Section
        string profitColor = netProfit >= 0 ? "أخضر" : "أحمر";
        Brush profitBrush = netProfit >= 0 ? Brushes.Green : Brushes.Red;

        graphics.DrawString("صافي الربح:", totalFont, profitBrush, width - 60, yPos, rightFormat);
        yPos += 25;

        graphics.DrawString($"{netProfit.ToString("N2")} جنيه", totalFont, profitBrush, width - 60, yPos, rightFormat);
        yPos += 20;

        graphics.DrawString($"( {profitColor} )", normalFont, profitBrush, width - 60, yPos, rightFormat);
        yPos += 40;

        // Additional Metrics
        graphics.DrawString("مؤشرات إضافية:", headerFont, Brushes.Black, width - 60, yPos, rightFormat);
        yPos += 25;

        decimal averageSale = salesCount > 0 ? salesTotal / salesCount : 0;
        decimal profitMargin = salesTotal > 0 ? (netProfit / salesTotal) * 100 : 0;

        graphics.DrawString($"متوسط قيمة الفاتورة: {averageSale.ToString("N2")} جنيه", normalFont, Brushes.Black, width - 60, yPos, rightFormat);
        yPos += 20;

        graphics.DrawString($"هامش الربح: {profitMargin.ToString("N2")}%", normalFont, Brushes.Black, width - 60, yPos, rightFormat);
        yPos += 30;

        // Footer
        graphics.DrawString("تم الإنشاء: " + DateTime.Now.ToString("yyyy/MM/dd HH:mm"), normalFont, Brushes.Gray, width / 2, yPos, centerFormat);
    }

    public void PrintInvoice(Sale sale)
    {
        try
        {
            _sale = sale;

            // Initialize fonts
            _titleFont = new Font(new FontFamily("Arial"), 14, System.Drawing.FontStyle.Bold);
            _boldFont = new Font(new FontFamily("Arial"), 10, System.Drawing.FontStyle.Bold);
            _normalFont = new Font("Arial", 9);
            _smallFont = new Font("Arial", 8);
            _barcodeFont = new Font("IDAutomationC39S", 18); // Barcode font

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintInvoicePage);

            // Set printer settings for 80mm thermal printer
            pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", 280, 1000); // 80mm width
            pd.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);

            // Try to find thermal printer, fallback to default
            string printerName = FindThermalPrinter();
            if (!string.IsNullOrEmpty(printerName))
            {
                pd.PrinterSettings.PrinterName = printerName;
            }

            pd.Print();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطأ في الطباعة: {ex.Message}", "خطأ",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private string FindThermalPrinter()
    {
        foreach (string printer in PrinterSettings.InstalledPrinters)
        {
            if (printer.ToLower().Contains("pos") ||
                printer.ToLower().Contains("thermal") ||
                printer.ToLower().Contains("80mm") ||
                printer.ToLower().Contains("receipt"))
            {
                return printer;
            }
        }
        return null; // Use default printer
    }

    private void PrintInvoicePage(object sender, PrintPageEventArgs e)
    {
        Graphics graphics = e.Graphics;
        float yPos = 0;
        float leftMargin = 10;
        float width = e.PageBounds.Width - 20;

        // Center alignment helper
        StringFormat centerFormat = new StringFormat();
        centerFormat.Alignment = StringAlignment.Center;

        // Right alignment for Arabic
        StringFormat rightFormat = new StringFormat();
        rightFormat.Alignment = StringAlignment.Far;

        // Header Section
        graphics.DrawString("فاتورة بيع", _titleFont, Brushes.Black, width / 2, yPos, centerFormat);
        yPos += _titleFont.GetHeight() + 5;

        graphics.DrawString("متجرك", _boldFont, Brushes.Black, width / 2, yPos, centerFormat);
        yPos += _boldFont.GetHeight() + 10;

        // Invoice Info
        graphics.DrawString($"رقم الفاتورة: {_sale.Id}", _normalFont, Brushes.Black, leftMargin, yPos);
        yPos += _normalFont.GetHeight();

        graphics.DrawString($"التاريخ: {_sale.Date:yyyy/MM/dd HH:mm}", _normalFont, Brushes.Black, leftMargin, yPos);
        yPos += _normalFont.GetHeight();

        graphics.DrawString($"طريقة الدفع: {GetPaymentTypeArabic(_sale.PaymentType)}", _normalFont, Brushes.Black, leftMargin, yPos);
        yPos += _normalFont.GetHeight() + 5;

        // Line separator
        graphics.DrawLine(new Pen(Color.Black), leftMargin, yPos, width, yPos);
        yPos += 10;

        // Items Header
        graphics.DrawString("الصنف", _boldFont, Brushes.Black, leftMargin, yPos);
        graphics.DrawString("الكمية", _boldFont, Brushes.Black, width - 120, yPos);
        graphics.DrawString("السعر", _boldFont, Brushes.Black, width - 60, yPos);
        yPos += _boldFont.GetHeight() + 5;

        // Items
        foreach (var item in _sale.products)
        {
            string productName = item.Product.Name.Length > 20 ?
                item.Product.Name.Substring(0, 20) + "..." : item.Product.Name;

            graphics.DrawString(productName, _normalFont, Brushes.Black, leftMargin, yPos);
            graphics.DrawString(item.saledQuantity.ToString(), _normalFont, Brushes.Black, width - 120, yPos);
            graphics.DrawString(item.Product.SellPrice.ToString("C"), _normalFont, Brushes.Black, width - 60, yPos);
            yPos += _normalFont.GetHeight() + 2;
        }

        yPos += 5;

        // Line separator
        graphics.DrawLine(new Pen(Color.Black), leftMargin, yPos, width, yPos);
        yPos += 10;

        // Totals
        graphics.DrawString($"المجموع: {_sale.Total.ToString("C")}", _boldFont, Brushes.Black, width, yPos, rightFormat);
        yPos += _boldFont.GetHeight();

        if (_sale.Discount > 0)
        {
            graphics.DrawString($"الخصم: {_sale.Discount.ToString("C")}", _normalFont, Brushes.Black, width, yPos, rightFormat);
            yPos += _normalFont.GetHeight();
        }

        graphics.DrawString($"المجموع النهائي: {_sale.NetTotal.ToString("C")}", _boldFont, Brushes.Black, width, yPos, rightFormat);
        yPos += _boldFont.GetHeight() + 5;

        // Debt information if applicable
        if (_sale.PaymentType.ToLower() == "debt" && _sale.User.Username != null)
        {
            graphics.DrawLine(new Pen(Color.Black), leftMargin, yPos, width, yPos);
            yPos += 10;

            graphics.DrawString("معلومات الدين", _boldFont, Brushes.Black, width / 2, yPos, centerFormat);
            yPos += _boldFont.GetHeight();

            graphics.DrawString($"العميل: {_sale.User.Username}", _normalFont, Brushes.Black, leftMargin, yPos);
            yPos += _normalFont.GetHeight();

            graphics.DrawString($"الهاتف: {_sale.User.PhoneNumber}", _normalFont, Brushes.Black, leftMargin, yPos);
            yPos += _normalFont.GetHeight();

            graphics.DrawString($"المدفوع: {_sale.paidAmount?.ToString("C")}", _normalFont, Brushes.Black, leftMargin, yPos);
            yPos += _normalFont.GetHeight();

            graphics.DrawString($"المتبقي: {_sale.remainingAmount?.ToString("C")}", _normalFont, Brushes.Black, leftMargin, yPos);
            yPos += _normalFont.GetHeight() + 5;
        }

        yPos += 10;

        // Barcode Section
        graphics.DrawString("كود الفاتورة", _boldFont, Brushes.Black, width / 2, yPos, centerFormat);
        yPos += _boldFont.GetHeight();

        // Barcode (using text-based barcode)
        if (_barcodeFont != null)
        {
            try
            {
                var barcodeWriter = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions
                    {
                        Height = 100,
                        Width = 300,
                        PureBarcode = true,
                        Margin = 5
                    }
                };
                

                var pixelData = barcodeWriter.Write(_sale.SecretCode);

                // Create bitmap from pixel data
                using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
                {
                    var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                        ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

                    try
                    {
                        Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                    bitmap.SetResolution(203, 203); // thermal printer DPI
                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                    graphics.SmoothingMode = SmoothingMode.None;
                    graphics.PixelOffsetMode = PixelOffsetMode.None;
                    // Draw barcode image centered
                    var xPos = (width - pixelData.Width) / 2;
                    graphics.DrawImage(bitmap, xPos, yPos);
                    yPos += pixelData.Height;

                }

            }
            catch (Exception ex)
            {
                // Fallback to text with error logging
                System.Diagnostics.Debug.WriteLine($"Barcode generation failed: {ex.Message}");
                graphics.DrawString(_sale.SecretCode, _boldFont, Brushes.Black, width / 2, yPos, centerFormat);
                yPos += _boldFont.GetHeight();
            }
        }
        else
        {
            graphics.DrawString(_sale.SecretCode, _boldFont, Brushes.Black, width / 2, yPos, centerFormat);
            yPos += _boldFont.GetHeight();
        }

        // Human readable secret code
        graphics.DrawString(_sale.SecretCode, _smallFont, Brushes.Black, width / 2, yPos, centerFormat);
        yPos += _smallFont.GetHeight() + 10;

        // Footer
        graphics.DrawString("شكراً لزيارتكم", _boldFont, Brushes.Black, width / 2, yPos, centerFormat);
        yPos += _boldFont.GetHeight();

        graphics.DrawString("نرجو زيارتكم مرة أخرى", _normalFont, Brushes.Black, width / 2, yPos, centerFormat);
    }

    private string GetPaymentTypeArabic(string paymentType)
    {
        return paymentType.ToLower() switch
        {
            "cash" => "نقدي",
            "debt" => "آجل",
            _ => paymentType
        };
    }
    
}