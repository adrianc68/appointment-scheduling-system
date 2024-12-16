using Microsoft.AspNetCore.Mvc;

namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.Communication.HttpResponseService
{
    public interface IHttpResponseService
    {
        IActionResult OkResponse<T>(T data, string version, string message = "Successful Request");
        IActionResult InternalServerErrorResponse(string error, string version, string? customMessage = null);
    }
}