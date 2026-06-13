namespace InventorySystem.Cmmon
{
    public static class InvoiceGenerator
    {
        public static string GenerateInvoiceNo()
        {
            var prefix = "INV";

            var datePart = DateTime.Now.ToString("yyyyMMdd");

            var randomPart = new Random().Next(1000, 9999);

            return $"{prefix}-{datePart}-{randomPart}";
        }
    }
}