using System;
using DotNetEnv;

namespace AppointmentSchedulerAPI.layers.CrossCuttingLayer.OperatationManagement
{
    public class EnvironmentVariableMgr
    {
        public EnvironmentVariableMgr()
        {
            Env.Load();
        }

        public string Get(string key, string? defaultValue = null)
        {
            return Environment.GetEnvironmentVariable(key) ?? defaultValue ?? string.Empty;
        }
    }
}
