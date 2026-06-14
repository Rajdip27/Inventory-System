using InventorySystem.Models.BaseEntities;

namespace InventorySystem.Models
{
    public class CustomerLedger: AuditableEntity
    {
        public long CustomerId { get; set; }
        public Customer Customer { get; set; }
        public long? InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public string ReferenceType { get; set; }
        public decimal Debit { get; set; }  
        public decimal Credit { get; set; }  
        public decimal Balance { get; set; }
        public string PaymentMethod { get; set; } // CASH, BANK (optional)
        public string Remarks { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
    }
}
