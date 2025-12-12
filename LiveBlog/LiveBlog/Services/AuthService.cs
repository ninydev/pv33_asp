using System.Security.Claims;
using LiveBlog.Models.IdentityUser;
using Microsoft.AspNetCore.Identity;

namespace LiveBlog.Services;

public class AuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<MyIdentityUserEntity> _userManager;
    
    public AuthService(IHttpContextAccessor httpContextAccessor,
        UserManager<MyIdentityUserEntity> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }
    
    /// <summary>
    /// Повертає ідентифікатор поточного автентифікованого користувача або кидає виняток.
    /// </summary>
    public string GetCurrentUserIdOrThrow()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var id = user?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(id))
            throw new InvalidOperationException("Користувач не автентифікований або відсутній ідентифікатор користувача.");
        return id;
    }

    /// <summary>
    /// Повертає об'єкт поточного автентифікованого користувача (`MyIdentityUserEntity`) або кидає виняток.
    /// </summary>
    public async Task<MyIdentityUserEntity> GetCurrentUserOrThrowAsync()
    {
        var principal = _httpContextAccessor.HttpContext?.User;
        if (principal == null)
            throw new InvalidOperationException("Користувач не автентифікований.");

        var user = await _userManager.GetUserAsync(principal);
        if (user == null)
            throw new InvalidOperationException("Не вдалося завантажити дані поточного користувача.");
        return user;
    }
}