using System;
using System.Globalization;
using System.Reflection;

namespace ConsoleTest;

public static class BuildInfo
{
    public static string GetEntryVersionInfo()
    {
        Assembly? entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly != null)
        {
            Attribute? attribute = Attribute.GetCustomAttribute(entryAssembly, typeof(AssemblyInformationalVersionAttribute));
            if (attribute is AssemblyInformationalVersionAttribute att)
                return att.InformationalVersion;
        }
        return "Unknown";
    }
}
