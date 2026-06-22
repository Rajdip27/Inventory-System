using DinkToPdf;
using DinkToPdf.Contracts;
using InventorySystem.Cmmon;

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

            // Inject base URL for relative paths/images
            // Inject base URL for relative paths/images if Html does not already contain a base/have a head tag
            if (!string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                // Only inject base tag if there's no <base ...> already in the HTML.
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
                    // If HTML already has a <head> but no <base>, insert a base tag into <head>.
                    // simple insertion - assumes a <head> tag exists:
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
            // Configure object settings based on options
            var objSettings = new ObjectSettings
            {
                HtmlContent = htmlContentWithCustomSize,
                WebSettings = new WebSettings
                {
                    DefaultEncoding = "utf-8",
                    LoadImages = options.LoadImages
                },
                LoadSettings = new LoadSettings
                {
                    BlockLocalFileAccess = !options.EnableLocalFileAccess
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
            // Configure global settings based on options

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
                    DocumentTitle = "Generated PDF"
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

    private PaperKind PaperKindFromString(string size) => size.ToUpper() switch
    {
        "A3" => PaperKind.A3,
        "A4" => PaperKind.A4,
        "A5" => PaperKind.A5,
        "LETTER" => PaperKind.Letter,
        "LEGAL" => PaperKind.Legal,
        _ => PaperKind.A4
    };
}
