using System.Xml.Linq;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using PDFDoc.Errors;
using PDFDoc.Core;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace PDFDoc;

public class PdfBuilder(string xml)
{
    private readonly string _xmlContent = xml ?? throw new ArgumentNullException(nameof(xml), "XML content cannot be null.");
    public PdfFont? CustomFont { get; set; }

    public byte[] BuildPdf()
    {
        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdfDoc = new PdfDocument(writer);
        var document = new Document(pdfDoc);

        try
        {
            var defaultFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            var xmlDoc = XDocument.Parse(_xmlContent);
            if (xmlDoc == null || xmlDoc.Root == null)
                throw new PdfDocException("The provided XML content is invalid or empty.");

            var textElement = xmlDoc.Root.Element("Text");
            if (textElement != null)
            {
                var isBold = XMLArgParser.IsBold(textElement.Attribute("bold"));

                var fontAttr = textElement.Attribute("font");
                var fontToUse = fontAttr != null && fontAttr.Value == "custom" && CustomFont != null
                    ? CustomFont
                    : defaultFont;

                var paragraph = new Paragraph(textElement.Value).SetFont(fontToUse);

                if (isBold)
                {
                    paragraph.SetBold();
                }

                document.Add(paragraph);
            }

            var tableElement = xmlDoc.Root.Element("Table");
            if (tableElement != null)
            {
                var columnCount = tableElement.Elements("Row").First().Elements("Cell").Count();
                Table table = new(columnCount);
                table.SetWidth(UnitValue.CreatePercentValue(100));
                table.SetMarginTop(10);

                foreach (var row in tableElement.Elements("Row"))
                {
                    foreach (var cell in row.Elements("Cell"))
                    {
                        var cellBackgroundColor = XMLArgParser.ParseColor(cell.Attribute("color"), ColorConstants.WHITE);

                        var isBold = XMLArgParser.IsBold(cell.Attribute("bold"));

                        var alignment = XMLArgParser.GetTextAlignment(cell.Attribute("alignment"), TextAlignment.LEFT);

                        var fontAttr = cell.Attribute("font");
                        var cellFont = fontAttr != null && fontAttr.Value == "custom" && CustomFont != null
                            ? CustomFont
                            : defaultFont;

                        var cellStyle = new Style()
                            .SetFont(cellFont)
                            .SetFontSize(11)
                            .SetTextAlignment(alignment)
                            .SetPadding(5)
                            .SetBorder(new iText.Layout.Borders.SolidBorder(ColorConstants.BLACK, 0.5f))
                            .SetBackgroundColor(cellBackgroundColor);

                        if (isBold)
                        {
                            cellStyle.SetBold();
                        }

                        table.AddCell(new Cell().Add(new Paragraph(cell.Value)).AddStyle(cellStyle));
                    }
                }

                document.Add(table);
            }
        }
        finally
        {
            document.Close();
        }

        return memoryStream.ToArray();
    }
}
