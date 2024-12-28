using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService
{
    public interface IHttpResponseService
    {
        IActionResult OkResponse<T>(T data, string version, string message = "Successful Request");
        IActionResult BadRequest(string version, string message = "Bad request");
        IActionResult Unauthorized(string version, string message = "Unauthorized");
        IActionResult Forbidden(string version, string message = "Forbidden");
        IActionResult Conflict(string version, string message = "Conflict");
        IActionResult InternalServerErrorResponse(Exception error, string version, string? customMessage = null);
        

    }
}