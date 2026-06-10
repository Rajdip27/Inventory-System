using System.ComponentModel.DataAnnotations;

namespace InventorySystem.ViewModel;

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required, DataType(DataType.Password)]
    public string Password { get; set; }
    public string ReturnUrl { get; set; } = "/Dashboard";
}
