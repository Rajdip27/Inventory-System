namespace InventorySystem.HealperUnit;

public static class BarcodeHelper
{
    public static string ToBarcodeText(string code)
    {
        if (string.IsNullOrEmpty(code))
            return "";

        return code;
    }
}
