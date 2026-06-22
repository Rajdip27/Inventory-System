namespace InventorySystem.Cmmon;

public class PdfOptions
{
    public string BaseUrl { get; set; }
    public bool EnableLocalFileAccess { get; set; } = false;
    public bool LoadImages { get; set; } = true;
    public string PageSize { get; set; } = "A4";
    public bool Landscape { get; set; } = false;
    public int MarginTop { get; set; } = 10;
    public int MarginBottom { get; set; } = 10;
    public int MarginLeft { get; set; } = 10;
    public int MarginRight { get; set; } = 10;
    public bool ShowPageNumbers { get; set; } = false;
    public bool ColorMode { get; set; } = true;
    public bool HideHeader { get; set; } = true;
    public bool HideFooter { get; set; } = true;

    // Optional custom size
    public double? CustomWidthMm { get; set; }
    public double? CustomHeightMm { get; set; }

    // Header/Footer settings
    public int HeaderFontSize { get; set; } = 10;
    public int FooterFontSize { get; set; } = 10;
    public string HeaderLeft { get; set; }
    public string HeaderCenter { get; set; }
    public string HeaderRight { get; set; }
    public string FooterLeft { get; set; }
    public string FooterCenter { get; set; }
    public string FooterRight { get; set; }
}
