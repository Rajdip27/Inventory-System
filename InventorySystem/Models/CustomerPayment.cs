using InventorySystem.Models.BaseEntities;

namespace InventorySystem.Models;

public class CustomerPayment: AuditableEntity
{
    public long CustomerId { get; set; }
    public Customer Customer { get; set; }

    public long SalesInvoiceId { get; set; }
    public SalesInvoice SalesInvoice { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; }
}
