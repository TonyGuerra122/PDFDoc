using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout;
using iText.Layout.Properties;
using System.Xml.Linq;

namespace PDFDoc.Core;

public static class XMLArgParser
{

    public static float GetFontSize(XAttribute? fontSizeAttr, float defaultSize)
    {
        if (fontSizeAttr != null && float.TryParse(fontSizeAttr.Value, out float fontSize))
        {
            return fontSize;
        }

        return defaultSize;
    }

    public static bool IsBold(XAttribute? boldAttr) => boldAttr != null && boldAttr.Value.Equals("true", StringComparison.CurrentCultureIgnoreCase);

    public static iText.Kernel.Colors.Color ParseColor(XAttribute? colorAttr, iText.Kernel.Colors.Color defaultColor)
    {
        if (colorAttr != null)
        {
            try
            {
                return WebColors.GetRGBColor(colorAttr.Value);
            }
            catch
            {
                return defaultColor;
            }
        }
        return defaultColor;
    }

    public static TextAlignment GetTextAlignment(XAttribute? alignmentAttr, TextAlignment defaultAlignment)
    {
        if (alignmentAttr != null)
        {
            return alignmentAttr.Value.ToLower() switch
            {
                "center" => TextAlignment.CENTER,
                "right" => TextAlignment.RIGHT,
                "left" => TextAlignment.LEFT,
                "justify" => TextAlignment.JUSTIFIED,
                _ => defaultAlignment
            };
        }

        return defaultAlignment;
    }

    public static byte[] ParseAll(XDocument doc)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdfDoc = new PdfDocument(writer);
        var document = new Document(pdfDoc);

        try
        {
            var titleElement = doc.Root?.Element("Title");
            if (titleElement != null)
            {
                var titleStyle = new Style()
                    .SetFontSize(GetFontSize(titleElement?.Attribute("fontSize"), 18))
                    .SetTextAlignment(GetTextAlignment(titleElement?.Attribute("alignment"), TextAlignment.CENTER));

                if (IsBold(titleElement?.Attribute("bold"))) titleStyle.SetBold();


                var colorAttr = titleElement?.Attribute("color");
                if (colorAttr != null) titleStyle.SetFontColor(ParseColor(colorAttr, ColorConstants.BLACK));

                document.Add(new Paragraph(titleElement?.Value).AddStyle(titleStyle));
            }

            var tableElement = doc.Root?.Element("Table");
            if (tableElement != null)
            {
                var columnCount = tableElement.Elements("Row").First().Elements("Cell").Count();
                Table table = new(columnCount);
                table.SetWidth(UnitValue.CreatePercentValue(100));
                table.SetMarginTop(10);

                var headerStyle = new Style()
                    .SetFontSize(12)
                    .SetBold()
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(5)
                    .SetBorder(Border.NO_BORDER);

                var cellStyle = new Style()
                    .SetFontSize(11)
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetPadding(5)
                    .SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f));

                foreach (var header in tableElement.Elements("Header"))
                {
                    table.AddHeaderCell(new Cell().Add(new Paragraph(header.Value)).AddStyle(headerStyle));
                }

                foreach (var row in tableElement.Elements("Row"))
                {
                    foreach (var cell in row.Elements("Cell"))
                    {
                        table.AddCell(new Cell().Add(new Paragraph(cell.Value)).AddStyle(cellStyle));
                    }
                }

                document.Add(table);
            }

            document.Close();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao processar o documento XML: {ex.Message}");
        }

        return memoryStream.ToArray();
    }
}
