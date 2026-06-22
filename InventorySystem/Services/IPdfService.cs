using DinkToPdf;
using DinkToPdf.Contracts;
using InventorySystem.Cmmon;
using System.Text;

namespace InventorySystem.Services;

public interface IPdfService
{
    Task<byte[]> GeneratePdf(string htmlContent, PdfOptions options = null);
}

public class PdfService(IConverter _converter) : IPdfService
{
    public async Task<byte[]> GeneratePdf(string htmlContent, PdfOptions options = null)
    {
        try
        {
            options ??= new PdfOptions();

            // Fix: Ensure proper HTML structure with encoding and fonts
            htmlContent = FixHtmlForPdf(htmlContent, options);

            // Inject base URL for relative paths/images
            if (!string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                var htmlLower = htmlContent?.ToLowerInvariant() ?? "";
                if (!htmlLower.Contains("<base ") && !htmlLower.Contains("<head"))
                {
                    htmlContent = $@"
                         <html>
                         <head>
                             <base href='{options.BaseUrl}' />
                         </head>
                         <body>
                             {htmlContent}
                         </body>
                         </html>";
                }
                else if (!htmlLower.Contains("<base "))
                {
                    htmlContent = htmlContent.Replace("<head>", $"<head><base href='{options.BaseUrl}' />");
                }
            }

            // Inject custom page size CSS if provided
            string htmlContentWithCustomSize = htmlContent;
            if (options.CustomWidthMm.HasValue && options.CustomHeightMm.HasValue)
            {
                htmlContentWithCustomSize = $@"
                <style>
                    @page {{
                        size: {options.CustomWidthMm.Value}mm {options.CustomHeightMm.Value}mm;
                        margin: {options.MarginTop}mm {options.MarginRight}mm {options.MarginBottom}mm {options.MarginLeft}mm;
                    }}
                </style>
                {htmlContent}";
            }

            // Configure object settings with font fixes
            var objSettings = new ObjectSettings
            {
                HtmlContent = htmlContentWithCustomSize,
                WebSettings = new WebSettings
                {
                    DefaultEncoding = "utf-8",
                    LoadImages = options.LoadImages,
                    // Fix: Enable JavaScript for better rendering
                    EnableJavascript = true,
                    // Fix: Set print media type
                    PrintMediaType = true,
                },
                LoadSettings = new LoadSettings
                {
                    BlockLocalFileAccess = !options.EnableLocalFileAccess,
                    // Fix: Increase timeout for complex pages
                    StopSlowScript = true,
                },
                HeaderSettings = options.HideHeader ? null : new HeaderSettings
                {
                    FontSize = options.HeaderFontSize,
                    Left = options.HeaderLeft,
                    Center = options.HeaderCenter,
                    Right = options.HeaderRight,
                    Line = !string.IsNullOrEmpty(options.HeaderLeft) ||
                           !string.IsNullOrEmpty(options.HeaderCenter) ||
                           !string.IsNullOrEmpty(options.HeaderRight)
                },
                FooterSettings = options.HideFooter ? null : new FooterSettings
                {
                    FontSize = options.FooterFontSize,
                    Left = options.FooterLeft,
                    Center = options.ShowPageNumbers ? "Page [page] of [toPage]" : options.FooterCenter,
                    Right = options.FooterRight,
                    Line = !string.IsNullOrEmpty(options.FooterLeft) ||
                           !string.IsNullOrEmpty(options.FooterCenter) ||
                           !string.IsNullOrEmpty(options.FooterRight) ||
                           options.ShowPageNumbers,
                    Spacing = 5
                }
            };

            // Configure global settings with DPI fix for better text rendering
            var doc = new HtmlToPdfDocument
            {
                GlobalSettings = new GlobalSettings
                {
                    ColorMode = options.ColorMode ? ColorMode.Color : ColorMode.Grayscale,
                    Orientation = options.Landscape ? Orientation.Landscape : Orientation.Portrait,
                    PaperSize = PaperKindFromString(options.PageSize),
                    Margins = new MarginSettings
                    {
                        Top = options.MarginTop,
                        Bottom = options.MarginBottom,
                        Left = options.MarginLeft,
                        Right = options.MarginRight
                    },
                    DocumentTitle = "Invoice",
                    // Fix: Higher DPI for sharper text
                    DPI = 150,
                },
                Objects = { objSettings }
            };

            return _converter.Convert(doc);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Fix HTML for better PDF rendering - adds proper fonts and encoding
    /// </summary>
    private string FixHtmlForPdf(string html, PdfOptions options)
    {
        // Check if HTML already has proper structure
        var hasHtmlTag = html.Contains("<html", StringComparison.OrdinalIgnoreCase);
        var hasHeadTag = html.Contains("<head", StringComparison.OrdinalIgnoreCase);
        var hasBodyTag = html.Contains("<body", StringComparison.OrdinalIgnoreCase);

        var sb = new StringBuilder();

        if (!hasHtmlTag)
        {
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
        }

        if (!hasHeadTag)
        {
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset='UTF-8'>");
            sb.AppendLine("<meta http-equiv='Content-Type' content='text/html; charset=utf-8'>");
            sb.AppendLine("<style>");
            sb.AppendLine("  /* ── FIX: Better font rendering for PDF ── */");
            sb.AppendLine("  body {");
            sb.AppendLine("    font-family: Arial, Helvetica, sans-serif;");
            sb.AppendLine("    font-size: 12px;");
            sb.AppendLine("    -webkit-font-smoothing: antialiased;");
            sb.AppendLine("    -moz-osx-font-smoothing: grayscale;");
            sb.AppendLine("    text-rendering: optimizeLegibility;");
            sb.AppendLine("    background: #ffffff;");
            sb.AppendLine("    color: #000000;");
            sb.AppendLine("    margin: 0;");
            sb.AppendLine("    padding: 0;");
            sb.AppendLine("  }");
            sb.AppendLine("  table { border-collapse: collapse; }");
            sb.AppendLine("  * { -webkit-print-color-adjust: exact !important; print-color-adjust: exact !important; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
        }
        else
        {
            // Insert meta tags if head exists but no charset
            if (!html.Contains("charset", StringComparison.OrdinalIgnoreCase))
            {
                html = html.Replace("<head>", "<head><meta charset='UTF-8'>");
            }
        }

        if (!hasBodyTag)
        {
            sb.AppendLine("<body>");
        }

        // Add the content
        sb.AppendLine(html);

        if (!hasBodyTag)
        {
            sb.AppendLine("</body>");
        }

        if (!hasHtmlTag)
        {
            sb.AppendLine("</html>");
        }

        return sb.ToString();
    }

    private PaperKind PaperKindFromString(string size) => size?.ToUpper() switch
    {
        "A3" => PaperKind.A3,
        "A4" => PaperKind.A4,
        "A5" => PaperKind.A5,
        "LETTER" => PaperKind.Letter,
        "LEGAL" => PaperKind.Legal,
        _ => PaperKind.A4
    };
}