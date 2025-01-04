using AppointmentSchedulerAPI.layers.ServiceLayer;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService
{
    public interface IHttpResponseService
    {
        IActionResult OkResponse<T>(T data, ApiVersionEnum version, string message = "Successful Request");
        IActionResult BadRequest(ApiVersionEnum version, string message);
        IActionResult Unauthorized(ApiVersionEnum version, string message);
        IActionResult Forbidden(ApiVersionEnum version, string message);
        IActionResult Conflict<T>(T data, ApiVersionEnum version, string message);
        IActionResult Conflict<T>(List<T> data, ApiVersionEnum version, string message);
        IActionResult Conflict(ApiVersionEnum version, string message);
        IActionResult InternalServerErrorResponse(Exception error, ApiVersionEnum version, string? customMessage = null);
        

    }
}