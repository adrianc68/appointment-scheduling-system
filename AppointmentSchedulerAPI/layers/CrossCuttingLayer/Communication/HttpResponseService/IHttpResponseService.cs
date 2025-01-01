using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService
{
    public interface IHttpResponseService
    {
        IActionResult OkResponse<T>(T data, string version, string message = "Successful Request");
        IActionResult BadRequest(string version, string message);
        IActionResult Unauthorized(string version, string message);
        IActionResult Forbidden(string version, string message);
        IActionResult Conflict<T>(T data, string version, string message);
        IActionResult Conflict(string version, string message);
        IActionResult InternalServerErrorResponse(Exception error, string version, string? customMessage = null);
        

    }
}