namespace Hello;

using System;
using System.Reflection;
using System.Runtime.Loader;

using Plugin;

public static class Program
{
    public static void Main()
    {
        Execute("OldVersion");
        Execute("NewVersion");
    }

    public static void Execute(string oldOrNew)
    {
        var assemblyLocation = Path.GetDirectoryName(typeof(Program).Assembly.Location);
        var pluginLocation = Path.Combine(assemblyLocation, oldOrNew, "MyLib.dll");
        var pluginLoadContext = new PluginLoadContext(pluginLocation);
        var assemblyName = new AssemblyName("MyLib");
        var assembly = pluginLoadContext.LoadFromAssemblyName(assemblyName);

        foreach (var type in assembly.GetTypes())
        {
            if (typeof(IPlugin).IsAssignableFrom(type))
            {
                var plugin = Activator.CreateInstance(type) as IPlugin;
                plugin.Execute("Hello");
            }
        }
    }
}

class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    public PluginLoadContext(string baseDir)
    {
        _resolver = new AssemblyDependencyResolver(baseDir);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath != null)
        {
            return LoadFromAssemblyPath(assemblyPath);
        }

        return null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (libraryPath != null)
        {
            return LoadUnmanagedDllFromPath(libraryPath);
        }

        return IntPtr.Zero;
    }
}
