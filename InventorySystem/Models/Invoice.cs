using InventorySystem.Models.BaseEntities;
using System.ComponentModel.DataAnnotations;
using static InventorySystem.Models.Auth.IdentityModel;

namespace InventorySystem.Models;

public class Invoice: AuditableEntity
{
    [Required, StringLength(50)]
    public string InvoiceNo { get; set; }
    public long CustomerId { get; set; }
    public Customer Customer { get; set; }
    public long UserId { get; set; }
    public User User { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.Now;
    // totals
    public decimal TotalPurchaseCost { get; set; }
    public decimal TotalSellingAmount { get; set; }
    public decimal TotalTaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public decimal TotalProfit { get; set; }
    public string PaymentStatus { get; set; } = "due";
    public string Notes { get; set; }
    public ICollection<InvoiceItem> InvoiceItems { get; set; }=new List<InvoiceItem>();
    public ICollection<Warranty> Warranty { get; set; }=new List<Warranty>();
    public ICollection<WarrantyClaim> WarrantyClaim { get; set; } = new List<WarrantyClaim>();
}
