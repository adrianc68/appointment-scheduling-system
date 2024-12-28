using AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.Model;
using AppointmentSchedulerAPI.layers.CrossCuttingLayer.OperatationManagement.ExceptionHandlerService;
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

        public IActionResult OkResponse<T>(T data, string version, string message = "Successful Request")
        {
            var payload = new ApiResponse<T>(StatusCodes.Status200OK, "OK", version, data);
            return new ObjectResult(payload) { StatusCode = StatusCodes.Status200OK };
        }

        public IActionResult BadRequest(string version, string message = "Bad request")
        {
            var payload = new ApiResponse<object>(StatusCodes.Status400BadRequest, message, version);
            return new ObjectResult(payload) { StatusCode = StatusCodes.Status400BadRequest };
        }

        public IActionResult Unauthorized(string version, string message = "Unauthorized")
        {
            var payload = new ApiResponse<object>(StatusCodes.Status401Unauthorized, message, version);
            return new ObjectResult(payload) { StatusCode = StatusCodes.Status401Unauthorized };
        }

        public IActionResult Forbidden(string version, string message = "Forbidden")
        {
            var payload = new ApiResponse<object>(StatusCodes.Status403Forbidden, message, version);
            return new ObjectResult(payload) { StatusCode = StatusCodes.Status403Forbidden };
        }

        public IActionResult Conflict(string version, string message = "Conflict")
        {
            var payload = new ApiResponse<object>(StatusCodes.Status409Conflict, message, version);
            return new ObjectResult(payload) { StatusCode = StatusCodes.Status409Conflict };
        }

        public IActionResult InternalServerErrorResponse(Exception exception, string version, string? customMessage = null)
        {
            string identifier = exceptionHandlerService.HandleException(exception, version);

            ErrorDetails errorData = new ErrorDetails
            {
                Error = MessageCodeType.SERVER_ERROR.ToString(),
                Message = "Please contact an administrator and provide the identifier.",
                Details = customMessage,
                Identifier = identifier
            };
            var payload = new ApiResponse<object>(StatusCodes.Status500InternalServerError, "Internal Server Error", version, errorData);
            return new ObjectResult(payload) { StatusCode = StatusCodes.Status500InternalServerError };
        }

    }
}