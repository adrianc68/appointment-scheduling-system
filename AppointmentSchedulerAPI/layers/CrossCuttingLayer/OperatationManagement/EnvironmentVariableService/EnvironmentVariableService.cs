using DotNetEnv;

namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.OperatationManagement;

public class EnvironmentVariableService
{
    public EnvironmentVariableService()
    {
        Env.Load();
    }

    public string Get(string key, string? defaultValue = null)
    {
        return Environment.GetEnvironmentVariable(key) ?? defaultValue ?? string.Empty;
    }
}
