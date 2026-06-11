using InventorySystem.ViewModel;
using Microsoft.AspNetCore.Identity;
using static InventorySystem.Models.Auth.IdentityModel;

namespace InventorySystem.Repositories;

public interface IAuthService
{
    Task<RegistrationResponse> Register(RegisterViewModel model);

}
public class AuthService(UserManager<User> _userManager) : IAuthService
{
    public async Task<RegistrationResponse> Register(RegisterViewModel request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new RegistrationResponse
            {
                Success = false,
                Errors = new() { $"Email '{request.Email}' is already registered." }
            };
        }

        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber,
            FullName = request.FullName,
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return new RegistrationResponse
            {
                Success = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        await _userManager.AddToRoleAsync(user, "User");

        return new RegistrationResponse
        {
            Success = true,
            UserId = user.Id
        };
    }
}