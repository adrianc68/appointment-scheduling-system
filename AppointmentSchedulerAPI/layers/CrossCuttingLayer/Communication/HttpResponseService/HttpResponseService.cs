using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.OperatationManagement.ExceptionHandlerService;
using AppointmentSchedulerAPI.layers.ServiceLayer;
using Microsoft.AspNetCore.Mvc;


namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService
{
    public class HttpResponseService : IHttpResponseService
    {
        private readonly IExceptionHandlerService exceptionHandlerService;


        public HttpResponseService(IExceptionHandlerService exceptionHandlerService)
        {
            this.exceptionHandlerService = exceptionHandlerService;
        }

        public IActionResult OkResponse<T>(T data, ApiVersionEnum version, string message = "Successful Request")
        {
            var payload = new ApiResponse<T>(StatusCodes.Status200OK, "OK", version.ToString(), data);
            return new ObjectResult(payload) { StatusCode = StatusCodes.Status200OK };
        }

        public IActionResult BadRequest(ApiVersionEnum version, string message)
        {
            var payload = new ApiResponse<object>(StatusCodes.Status400BadRequest, message, version.ToString());
            return new ObjectResult(payload) { StatusCode = StatusCodes.Status400BadRequest };
        }

        public IActionResult Unauthorized(ApiVersionEnum version, string message)
        {
            var payload = new ApiResponse<object>(StatusCodes.Status401Unauthorized, message, version.ToString());
            return new ObjectResult(payload) { StatusCode = StatusCodes.Status401Unauthorized };
        }

        public IActionResult Unauthorized<T>(T data, ApiVersionEnum version, string message)
        {
            var payload = new ApiResponse<T>(StatusCodes.Status401Unauthorized, message, version.ToString(), data);
            return new ObjectResult(payload) { StatusCode = StatusCodes.Status401Unauthorized };
        }

        public IActionResult Forbidden(ApiVersionEnum version, string message)
        {
            var payload = new ApiResponse<object>(StatusCodes.Status403Forbidden, message, version.ToString());
            return new ObjectResult(payload) { StatusCode = StatusCodes.Status403Forbidden };
        }

        public IActionResult Conflict<T>(T data, ApiVersionEnum version, string message)
        {
            var payload = new ApiResponse<T>(StatusCodes.Status409Conflict, message, version.ToString(), data);
            return new ObjectResult(payload) { StatusCode = StatusCodes.Status409Conflict };
        }

        public IActionResult Conflict<T>(List<T> data, ApiVersionEnum version, string message)
        {
            var payload = new ApiResponse<List<T>>(StatusCodes.Status409Conflict, message, version.ToString(), data);
            return new ObjectResult(payload) { StatusCode = StatusCodes.Status409Conflict };
        }

        public IActionResult Conflict(ApiVersionEnum version, string message)
        {
            var payload = new ApiResponse<object>(StatusCodes.Status409Conflict, message, version.ToString());
            return new ObjectResult(payload) { StatusCode = StatusCodes.Status409Conflict };
        }

        public IActionResult InternalServerErrorResponse(Exception exception, ApiVersionEnum version, string? customMessage = null)
        {
            string identifier = exceptionHandlerService.HandleException(exception, version.ToString());

            ErrorDetails errorData = new ErrorDetails
            {
                Error = MessageCodeType.SERVER_ERROR.ToString(),
                Message = "Please contact an administrator and provide the identifier.",
                Details = customMessage,
                Identifier = identifier
            };
            var payload = new ApiResponse<object>(StatusCodes.Status500InternalServerError, "Internal Server Error", version.ToString(), errorData);
            return new ObjectResult(payload) { StatusCode = StatusCodes.Status500InternalServerError };
        }

    }
}