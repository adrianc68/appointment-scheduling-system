using System.Security.Claims;
using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;

public class UserClaims
{
    public required string Username { get; set; }
    public required Guid Uuid { get; set; }
    public required string Email { get; set; }
    public required RoleType Role { get; set; }
}

public class ClaimsPOCO
{
    public static UserClaims GetUserClaims(ClaimsPrincipal user)
    {
        if (!Enum.TryParse<RoleType>(user.FindFirst(ClaimTypes.Role)!.Value, out var userRole))
        {
            throw new ArgumentException("Invalid role claim value");
        }

        return new UserClaims
        {
            Username = user.FindFirst("username")!.Value,
            Uuid = Guid.Parse(user.FindFirst("uuid")!.Value),
            Email = user.FindFirst(ClaimTypes.Email)!.Value,
            Role = userRole
        };
    }
}
