using InventorySystem.Models.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models;

public class Customer: AuditableEntity
{
    [Required, StringLength(150)]
    public string CustomerName { get; set; }

    [StringLength(20)]
    public string Phone { get; set; }

    [StringLength(150)]
    public string Email { get; set; }

    public string Address { get; set; }
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Warranty> Warranty { get; set; } = new List<Warranty>();
    public ICollection<WarrantyClaim> WarrantyClaim { get; set; } = new List<WarrantyClaim>();
}
