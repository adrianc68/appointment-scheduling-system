using AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Security.Authorization.Attributes;
using AppointmentSchedulerAPI.layers.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class HttpResponseAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public HttpResponseAuthorizationMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var endpoint = httpContext.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>();

        if (allowAnonymous != null)
        {
            await _next(httpContext);
            return;
        }

        if (httpContext.User.Identity == null || !httpContext.User.Identity.IsAuthenticated)
        {
            await RespondUnauthorized(httpContext);
            return;
        }

        var allowedRoles = endpoint?.Metadata.GetMetadata<AllowedRolesAttribute>();
        if (allowedRoles != null && !IsUserInAllowedRole(httpContext.User, allowedRoles.Roles))
        {
            await RespondForbidden(httpContext);
            return;
        }

        await _next(httpContext);
    }

    private bool IsUserInAllowedRole(ClaimsPrincipal user, RoleType[] allowedRoles)
    {
        var roleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        if (roleClaim == null || !Enum.TryParse<RoleType>(roleClaim, out var userRole))
        {
            return false;
        }

        return allowedRoles.Contains(userRole);
    }

    private async Task RespondUnauthorized(HttpContext httpContext)
    {
        using var scope = _serviceProvider.CreateScope();
        var httpResponseService = scope.ServiceProvider.GetRequiredService<IHttpResponseService>();
        var response = httpResponseService.Unauthorized(ApiVersionEnum.V1, MessageCodeType.UNAUTHORIZED.ToString());
        await response.ExecuteResultAsync(new ActionContext { HttpContext = httpContext });
    }

    private async Task RespondForbidden(HttpContext httpContext)
    {
        using var scope = _serviceProvider.CreateScope();
        var httpResponseService = scope.ServiceProvider.GetRequiredService<IHttpResponseService>();
        var response = httpResponseService.Forbidden(ApiVersionEnum.V1, MessageCodeType.UNAUTHORIZED.ToString());
        await response.ExecuteResultAsync(new ActionContext { HttpContext = httpContext });
    }
}
