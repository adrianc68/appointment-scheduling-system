using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.ServiceLayer;
using Microsoft.AspNetCore.Mvc;

public class HttpResponseAuthorizationMiddleware
{
    private readonly RequestDelegate next;
    private readonly IServiceProvider serviceProvider;

    public HttpResponseAuthorizationMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        this.next = next;
        this.serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (httpContext.User.Identity == null || !httpContext.User.Identity.IsAuthenticated)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var httpResponseService = scope.ServiceProvider.GetRequiredService<IHttpResponseService>();
                var version = ApiVersionEnum.V1;
                var message = MessageCodeType.UNAUTHORIZED;
                var response = httpResponseService.Unauthorized(version, message.ToString());
                await response.ExecuteResultAsync(new ActionContext { HttpContext = httpContext });
            }
            return;
        }
        await next(httpContext);
    }
}
