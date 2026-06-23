namespace InventorySystem.Cmmon;

public class PdfOptions
{
    // Standard paper settings
    public string PageSize { get; set; } = "A4";
    public bool Landscape { get; set; } = false;

    // Margins in mm
    public int MarginTop { get; set; } = 15;
    public int MarginBottom { get; set; } = 15;
    public int MarginLeft { get; set; } = 10;
    public int MarginRight { get; set; } = 10;

    // Headers
    public string HeaderLeft { get; set; }
    public string HeaderCenter { get; set; }
    public string HeaderRight { get; set; }
    public int HeaderFontSize { get; set; } = 10;
    public bool HideHeader { get; set; } = false;

    // Footers
    public string FooterLeft { get; set; }
    public string FooterCenter { get; set; }
    public string FooterRight { get; set; }
    public int FooterFontSize { get; set; } = 10;
    public bool HideFooter { get; set; } = false;
    public bool ShowPageNumbers { get; set; } = false;

    // Custom page size in mm
    public double? CustomWidthMm { get; set; } = null;
    public double? CustomHeightMm { get; set; } = null;

    public bool ColorMode { get; set; } = false;

    // Add this for relative images
    public string BaseUrl { get; set; }

    // Add this if you use local images from disk
    public bool EnableLocalFileAccess { get; set; } = false;

    // Optional: enable image loading explicitly
    public bool LoadImages { get; set; } = true;
}
