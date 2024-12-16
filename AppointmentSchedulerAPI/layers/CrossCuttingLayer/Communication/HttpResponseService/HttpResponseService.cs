using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService.Model;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService
{
    public class HttpResponseService : IHttpResponseService
    {
        private readonly ILogger<HttpResponseService> logger;

        public HttpResponseService(ILogger<HttpResponseService> logger)
        {
            this.logger = logger;
        }

        public IActionResult OkResponse<T>(T data, string version, string message = "Successful Request")
        {
            var payload = new ApiResponse<T>(StatusCodes.Status200OK, "OK", data, version);
            return new ObjectResult(payload) { StatusCode = StatusCodes.Status200OK };
        }
        public IActionResult InternalServerErrorResponse(string error, string version, string? customMessage = null)
        {
            var identifier = Guid.NewGuid().ToString();
            var errorData = new
            {
                error = "Internal Server Error",
                message = "Please contact an administrator and provide the identifier.",
                details = customMessage,
                identifier
            };
            logger.LogError("Error Identifier: {Identifier}, Error: {Error}", identifier, error);
            var payload = new ApiResponse<object>(StatusCodes.Status500InternalServerError, "Internal Server Error", errorData, version);
            return new ObjectResult(payload) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}