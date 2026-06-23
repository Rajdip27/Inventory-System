using DinkToPdf;
using DinkToPdf.Contracts;
using InventorySystem.Cmmon;
using System.Text;

namespace InventorySystem.Services;

/// <summary>
/// Defines a service for generating PDF documents from HTML content.
/// </summary>
public interface IPdfService
{
    byte[] GeneratePdf(string htmlContent, PdfOptions options = null);
}
/// <summary>
/// Provides functionality to generate PDF documents from HTML content using configurable options.
/// </summary>
/// <remarks>This service acts as a wrapper around an underlying PDF conversion engine, allowing customization of
/// page size, orientation, margins, headers, footers, and other rendering options through the provided configuration.
/// It is intended for scenarios where dynamic PDF generation from HTML is required, such as reporting or document
/// export features.</remarks>
public class PdfService : IPdfService
{
    private readonly IConverter _converter;

    public PdfService(IConverter converter)
    {
        _converter = converter;
    }

    public byte[] GeneratePdf(string htmlContent, PdfOptions options = null)
    {
        try
        {
            options ??= new PdfOptions();

            // Inject base URL for relative paths/images
            if (!string.IsNullOrWhiteSpace(options.BaseUrl))
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
                    DPI = 140, // Better text rendering
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