using System.Security.Claims;

public class UserClaims
{
    public required string Username { get; set; }
    public required string Uuid { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
}

public class ClaimsPOCO
{
    public static UserClaims GetUserClaims(ClaimsPrincipal user)
    {
        return new UserClaims
        {
            Username = user.FindFirst("username")!.Value,
            Uuid = user.FindFirst("uuid")!.Value,
            Email = user.FindFirst(ClaimTypes.Email)!.Value,
            Role = user.FindFirst(ClaimTypes.Role)!.Value
        };
    }
}
