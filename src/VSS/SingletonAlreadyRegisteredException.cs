using System;

namespace VSS;

/// <summary>
/// Exception thrown when attempting to register a singleton that is already registered.
/// </summary>
public class SingletonAlreadyRegisteredException(Type type) : Exception($"Singleton {type.FullName} already registered.")
{
    /// <summary>
    /// Throws <see cref="SingletonAlreadyRegisteredException"/> if the singleton for <typeparamref name="T"/> is already created.
    /// </summary>
    /// <typeparam name="T">The singleton type.</typeparam>
    public static void ThrowIfIsCreated<T>() where T : class
    {
        if(!Singleton<T>.IsCreated) return;
        throw new SingletonAlreadyRegisteredException(typeof(T));
    }
}
