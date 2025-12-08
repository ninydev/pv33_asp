using System.Security.Claims;

namespace LiveBlog.Services;

public class AuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public AuthService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
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
}