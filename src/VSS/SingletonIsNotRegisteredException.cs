using System;

namespace VSS;

/// <summary>
/// Exception thrown when accessing or managing a singleton that has not been registered.
/// </summary>
public class SingletonIsNotRegisteredException(Type type) : Exception($"Singleton {type.FullName} is not registered.")
{
    /// <summary>
    /// Throws <see cref="SingletonIsNotRegisteredException"/> if the singleton for <typeparamref name="T"/> is not created.
    /// </summary>
    /// <typeparam name="T">The singleton type.</typeparam>
    public static void ThrowIfIsNotCreated<T>() where T : class
    {
        if(Singleton<T>.IsCreated) return;
        throw new SingletonIsNotRegisteredException(typeof(T));
    }
}