using System.Collections.Concurrent;
using System.Reflection;

namespace DevSAAS.Core.Helpers;

public static class EmbeddedResource
{
    private static readonly ConcurrentDictionary<string, string> ResourceCache =
        new ConcurrentDictionary<string, string>();

    public static Task<string> LoadResourceAsync(string resourceName)
    {
        if (HasResource(resourceName))
        {
            return Task.FromResult(GetResource(resourceName));
        }

        var assembly = Assembly.GetCallingAssembly();
        return LoadAsync(assembly, resourceName);
    }

    public static string LoadResource(string resourceName)
    {
        if (HasResource(resourceName))
        {
            return GetResource(resourceName);
        }

        var assembly = Assembly.GetCallingAssembly();
        return Load(assembly, resourceName);
    }

    private static string Load(Assembly assembly, string resourceName)
    {
        if (ResourceCache.TryGetValue(resourceName, out var result)) return GetResource(resourceName);

        using var stream = assembly?.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            throw new FileNotFoundException($"Embedded resource {resourceName} not found");
        }

        using var reader = new StreamReader(stream);
        result = reader.ReadToEnd();

        if (result is null) throw new FileNotFoundException($"Embedded resource {resourceName} not found");
        ResourceCache.TryAdd(resourceName, result);

        return GetResource(resourceName);
    }

    private static async Task<string> LoadAsync(Assembly assembly, string resourceName)
    {
        if (ResourceCache.TryGetValue(resourceName, out var result)) return GetResource(resourceName);

        await using var stream = assembly?.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            throw new FileNotFoundException($"Embedded resource {resourceName} not found");
        }

        using var reader = new StreamReader(stream);
        result = await reader.ReadToEndAsync();

        if (result is null) throw new FileNotFoundException($"Embedded resource {resourceName} not found");
        ResourceCache.TryAdd(resourceName, result);

        return GetResource(resourceName);
    }

    private static bool HasResource(string resourceName)
    {
        return ResourceCache.ContainsKey(resourceName);
    }

    private static string GetResource(string resourceName)
    {
        return ResourceCache[resourceName];
    }
}