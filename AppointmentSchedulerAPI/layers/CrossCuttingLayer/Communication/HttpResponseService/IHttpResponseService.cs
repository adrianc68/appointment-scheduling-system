using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService
{
    public interface IHttpResponseService
    {
        IActionResult OkResponse<T>(T data, string version, string message = "Successful Request");
        IActionResult InternalServerErrorResponse(Exception error, string version, string? customMessage = null);
        IActionResult BadRequest(string version, string message = "Bad request");
        IActionResult Unauthorized(string version, string message = "Unauthorized");
    }
}