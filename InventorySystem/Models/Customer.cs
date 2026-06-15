using InventorySystem.Models.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models;

public class Customer: AuditableEntity
{
    [Required, StringLength(150)]
    public string Name { get; set; }

    [StringLength(20)]
    public string Phone { get; set; }

    [StringLength(150)]
    public string Email { get; set; }
    public decimal OpeningBalance { get; set; } = 0;
    public string Address { get; set; }
    public ICollection<Warranty> Warranty { get; set; } = new List<Warranty>();
    public ICollection<CustomerLedger> CustomerLedger { get; set; } = new List<CustomerLedger>();
    public ICollection<CustomerPayment> CustomerPayment { get; set; } = new List<CustomerPayment>();
    public ICollection<WarrantyClaim> WarrantyClaim { get; set; }
= new List<WarrantyClaim>();
}
