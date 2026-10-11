using System.Reflection;

namespace Uptime.Web.Services;

public class VersionProvider
{
    public string Version =>
        Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion?.Split("+")[0]
        ?? "Unknown";
}
