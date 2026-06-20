using iTextSharp.text;
using iTextSharp.text.pdf;
using OnlineFruitShop.Core.Entities;
using OnlineFruitShop.Core.Interfaces;

namespace OnlineFruitShop.Infrastructure.Services
{
    public class DeliveryReceiptService : IDeliveryReceiptService
    {
        public byte[] GenerateDeliveryReceipt(Order order, string customerName)
        {
            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 40, 40, 40, 40);
                var writer = PdfWriter.GetInstance(document, stream);
                document.Open();

                // Header
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.WHITE);
                var header = new PdfPTable(1);
                header.WidthPercentage = 100;
                var headerCell = new PdfPCell(new Phrase("FRUITSHOP - DELIVERY RECEIPT", headerFont))
                {
                    BackgroundColor = new BaseColor(16, 185, 129),
                    Padding = 15,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                header.AddCell(headerCell);
                document.Add(header);

                document.Add(new Paragraph("\n"));

                // Order Info Section
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
                var labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.DARK_GRAY);

                document.Add(new Paragraph("ORDER INFORMATION", titleFont));
                document.Add(new Paragraph("_________________________________________________________________________________"));

                var infoTable = new PdfPTable(2);
                infoTable.WidthPercentage = 100;
                infoTable.SetWidths(new float[] { 50, 50 });

                AddInfoRow(infoTable, labelFont, normalFont, "Order ID:", $"#{order.Id}");
                AddInfoRow(infoTable, labelFont, normalFont, "Status:", "DELIVERED");
                AddInfoRow(infoTable, labelFont, normalFont, "Delivered On:", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                AddInfoRow(infoTable, labelFont, normalFont, "Tracking Number:", order.TrackingNumber);

                document.Add(infoTable);
                document.Add(new Paragraph("\n"));

                // Customer Info Section
                document.Add(new Paragraph("CUSTOMER DETAILS", titleFont));
                document.Add(new Paragraph("_________________________________________________________________________________"));

                var customerTable = new PdfPTable(2);
                customerTable.WidthPercentage = 100;
                customerTable.SetWidths(new float[] { 50, 50 });

                AddInfoRow(customerTable, labelFont, normalFont, "Customer Name:", customerName);
                AddInfoRow(customerTable, labelFont, normalFont, "Shipping Address:", order.ShippingAddress);
                AddInfoRow(customerTable, labelFont, normalFont, "Shipping Method:", order.ShippingMethod);

                document.Add(customerTable);
                document.Add(new Paragraph("\n"));

                // Order Items Section
                document.Add(new Paragraph("ORDER ITEMS", titleFont));
                document.Add(new Paragraph("_________________________________________________________________________________"));

                var itemsTable = new PdfPTable(4);
                itemsTable.WidthPercentage = 100;
                itemsTable.SetWidths(new float[] { 40, 20, 20, 20 });

                // Headers
                var headerBgColor = new BaseColor(37, 99, 235);
                var headerCellFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE);

                AddTableHeader(itemsTable, headerCellFont, headerBgColor, "Fruit Name");
                AddTableHeader(itemsTable, headerCellFont, headerBgColor, "Quantity");
                AddTableHeader(itemsTable, headerCellFont, headerBgColor, "Unit Price");
                AddTableHeader(itemsTable, headerCellFont, headerBgColor, "Total");

                // Items
                foreach (var item in order.Items)
                {
                    itemsTable.AddCell(new PdfPCell(new Phrase(item.Fruit?.Name ?? "", normalFont)) { Padding = 8 });
                    itemsTable.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString("0.##"), normalFont)) { Padding = 8, HorizontalAlignment = Element.ALIGN_RIGHT });
                    itemsTable.AddCell(new PdfPCell(new Phrase($"₹{item.UnitPrice:N2}", normalFont)) { Padding = 8, HorizontalAlignment = Element.ALIGN_RIGHT });
                    itemsTable.AddCell(new PdfPCell(new Phrase($"₹{(item.Quantity * item.UnitPrice):N2}", normalFont)) { Padding = 8, HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                document.Add(itemsTable);
                document.Add(new Paragraph("\n"));

                // Price Breakdown Section
                document.Add(new Paragraph("PRICE BREAKDOWN", titleFont));
                document.Add(new Paragraph("_________________________________________________________________________________"));

                var breakdownTable = new PdfPTable(2);
                breakdownTable.WidthPercentage = 100;
                breakdownTable.SetWidths(new float[] { 70, 30 });

                AddBreakdownRow(breakdownTable, labelFont, normalFont, "Subtotal:", $"₹{order.Subtotal:N2}", false);
                AddBreakdownRow(breakdownTable, labelFont, normalFont, "Tax:", $"₹{order.Tax:N2}", false);
                AddBreakdownRow(breakdownTable, labelFont, normalFont, "Shipping Charge:", $"₹{order.ShippingCharge:N2}", false);
                AddBreakdownRow(breakdownTable, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK), 
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(16, 185, 129)), "TOTAL:", $"₹{order.Total:N2}", true);

                document.Add(breakdownTable);
                document.Add(new Paragraph("\n"));

                // Footer
                var footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.DARK_GRAY);
                document.Add(new Paragraph("Thank you for shopping with FruitShop!", FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 10)));
                document.Add(new Paragraph("This is an automated receipt. Please keep it for your records.", footerFont));
                document.Add(new Paragraph($"Generated on: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}", footerFont));

                document.Close();
                return stream.ToArray();
            }
        }

        private void AddInfoRow(PdfPTable table, Font labelFont, Font valueFont, string label, string value)
        {
            var labelCell = new PdfPCell(new Phrase(label, labelFont)) { Border = Rectangle.NO_BORDER, Padding = 8 };
            var valueCell = new PdfPCell(new Phrase(value, valueFont)) { Border = Rectangle.NO_BORDER, Padding = 8 };

            table.AddCell(labelCell);
            table.AddCell(valueCell);
        }

        private void AddTableHeader(PdfPTable table, Font font, BaseColor bgColor, string text)
        {
            var cell = new PdfPCell(new Phrase(text, font))
            {
                BackgroundColor = bgColor,
                Padding = 8,
                HorizontalAlignment = text.Contains("Total") || text.Contains("Price") || text.Contains("Quantity") ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT
            };
            table.AddCell(cell);
        }

        private void AddBreakdownRow(PdfPTable table, Font labelFont, Font valueFont, string label, string value, bool isTotal)
        {
            var labelCell = new PdfPCell(new Phrase(label, labelFont))
            {
                Border = isTotal ? Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER : Rectangle.NO_BORDER,
                Padding = 8,
                HorizontalAlignment = Element.ALIGN_RIGHT
            };

            var valueCell = new PdfPCell(new Phrase(value, valueFont))
            {
                Border = isTotal ? Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER : Rectangle.NO_BORDER,
                Padding = 8,
                HorizontalAlignment = Element.ALIGN_RIGHT
            };

            table.AddCell(labelCell);
            table.AddCell(valueCell);
        }
    }
}
