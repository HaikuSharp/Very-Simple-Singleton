using System;

namespace VSS;

/// <summary>
/// Static facade for registering and managing singleton instances.
/// </summary>
public class Singleton
{
    /// <summary>
    /// Indicates whether the singleton of the specified type is registered.
    /// </summary>
    /// <typeparam name="TInstance">The singleton type.</typeparam>
    /// <returns><see langword="true"/> if a singleton accessor is registered; otherwise, <see langword="false"/>.</returns>
    public static bool IsRegistered<TInstance>() where TInstance : class => Singleton<TInstance>.IsRegistered;

    /// <summary>
    /// Indicates whether the singleton of the specified type has been created.
    /// </summary>
    /// <typeparam name="TInstance">The singleton type.</typeparam>
    /// <returns><see langword="true"/> if the singleton instance has been created; otherwise, <see langword="false"/>.</returns>
    public static bool IsCreated<TInstance>() where TInstance : class => Singleton<TInstance>.IsCreated;

    public static TInstance GetInstance<TInstance>() where TInstance : class => Singleton<TInstance>.Instance;

    public static bool TryGetInstance<TInstance>(out TInstance instance) where TInstance : class => Singleton<TInstance>.TryGetInstance(out instance);

    /// <summary>
    /// Registers a lazy factory for the singleton of the specified type.
    /// </summary>
    /// <typeparam name="TInstance">The singleton type.</typeparam>
    /// <param name="constructor">The factory used to create the instance on first access.</param>
    /// <exception cref="SingletonAlreadyRegisteredException">Thrown if the singleton is already registered.</exception>
    public static void RegisterLazy<TInstance>(Func<TInstance> constructor) where TInstance : class => Singleton<TInstance>.RegisterLazy(constructor);

    /// <summary>
    /// Registers a provided <see cref="Lazy{T}"/> for the singleton of the specified type.
    /// </summary>
    /// <typeparam name="TInstance">The singleton type.</typeparam>
    /// <param name="lazyInstance">The lazy instance wrapper.</param>
    /// <exception cref="SingletonAlreadyRegisteredException">Thrown if the singleton is already registered.</exception>
    public static void RegisterLazy<TInstance>(Lazy<TInstance> lazyInstance) where TInstance : class => Singleton<TInstance>.RegisterLazy(lazyInstance);

    /// <summary>
    /// Registers an already created instance as the singleton of the specified type.
    /// </summary>
    /// <typeparam name="TInstance">The singleton type.</typeparam>
    /// <param name="instance">The instance to register.</param>
    /// <exception cref="SingletonAlreadyRegisteredException">Thrown if the singleton is already registered.</exception>
    public static void RegisterInstance<TInstance>(TInstance instance) where TInstance : class => Singleton<TInstance>.RegisterInstance(instance);

    /// <summary>
    /// Unregisters the singleton of the specified type without disposing it.
    /// </summary>
    /// <typeparam name="TInstance">The singleton type.</typeparam>
    /// <exception cref="SingletonIsNotRegisteredException">Thrown if the singleton is not registered.</exception>
    public static void Unregister<TInstance>() where TInstance : class => Singleton<TInstance>.Unregister();

    /// <summary>
    /// Disposes the singleton of the specified type if it implements <see cref="IDisposable"/> and unregisters it.
    /// </summary>
    /// <typeparam name="TInstance">The singleton type.</typeparam>
    /// <exception cref="SingletonIsNotRegisteredException">Thrown if the singleton is not registered.</exception>
    public static void Dispose<TInstance>() where TInstance : class => Singleton<TInstance>.Dispose();
}

/// <summary>
/// Type-specific singleton registration and lifecycle management.
/// </summary>
/// <typeparam name="TInstance">The singleton type.</typeparam>
public class Singleton<TInstance> where TInstance : class
{
    private static ISingletonInstanceAccessor<TInstance> s_Accessor;

    /// <summary>
    /// Gets a value indicating whether a singleton accessor is registered for this type.
    /// </summary>
    public static bool IsRegistered => s_Accessor is not null;

    /// <summary>
    /// Gets a value indicating whether the singleton instance has been created.
    /// </summary>
    public static bool IsCreated => s_Accessor is { IsCreated: true };

    public static TInstance Instance => s_Accessor?.Access();

    public static bool TryGetInstance(out TInstance instance) => (instance = Instance) is not null;

    /// <summary>
    /// Registers a lazy factory for this singleton type.
    /// </summary>
    /// <param name="constructor">The factory used to create the instance on first access.</param>
    /// <exception cref="SingletonAlreadyRegisteredException">Thrown if the singleton is already registered.</exception>
    public static void RegisterLazy(Func<TInstance> constructor) => RegisterLazy(new Lazy<TInstance>(constructor, true));

    /// <summary>
    /// Registers a provided <see cref="Lazy{T}"/> wrapper for this singleton type.
    /// </summary>
    /// <param name="lazyInstance">The lazy instance wrapper.</param>
    /// <exception cref="SingletonAlreadyRegisteredException">Thrown if the singleton is already registered.</exception>
    public static void RegisterLazy(Lazy<TInstance> lazyInstance)
    {
        SingletonAlreadyRegisteredException.ThrowIfIsCreated<TInstance>();
        s_Accessor = new LazySingletonInstanceAccessor<TInstance>(lazyInstance);
    }

    /// <summary>
    /// Registers an already created instance as the singleton for this type.
    /// </summary>
    /// <param name="instance">The instance to register.</param>
    /// <exception cref="SingletonAlreadyRegisteredException">Thrown if the singleton is already registered.</exception>
    public static void RegisterInstance(TInstance instance)
    {
        SingletonAlreadyRegisteredException.ThrowIfIsCreated<TInstance>();
        s_Accessor = new SingletonInstanceAccessor<TInstance>(instance);
    }

    /// <summary>
    /// Unregisters the singleton without disposing it.
    /// </summary>
    /// <exception cref="SingletonIsNotRegisteredException">Thrown if the singleton is not registered.</exception>
    public static void Unregister()
    {
        SingletonIsNotRegisteredException.ThrowIfIsNotCreated<TInstance>();
        s_Accessor = null;
    }

    /// <summary>
    /// Disposes the singleton instance if it implements <see cref="IDisposable"/> and unregisters it.
    /// </summary>
    /// <exception cref="SingletonIsNotRegisteredException">Thrown if the singleton is not registered.</exception>
    public static void Dispose()
    {
        SingletonIsNotRegisteredException.ThrowIfIsNotCreated<TInstance>();
        s_Accessor.Dispose();
        s_Accessor = null;
    }

    private interface ISingletonInstanceAccessor<T> : IDisposable where T : class
    {
        bool IsCreated { get; }

        T Access();
    }

    private abstract class SingletonInstanceAccessorBase<T> : ISingletonInstanceAccessor<T> where T : class
    {
        public abstract bool IsCreated { get; }

        public abstract T Access();

        public void Dispose()
        {
            if(Access() is not IDisposable disposable) return;
            disposable.Dispose();
        }
    }

    private sealed class LazySingletonInstanceAccessor<T>(Lazy<T> lazyInstance) : SingletonInstanceAccessorBase<T>, ISingletonInstanceAccessor<T> where T : class
    {
        public override bool IsCreated => lazyInstance.IsValueCreated;

        public override T Access() => lazyInstance.Value;
    }

    private sealed class SingletonInstanceAccessor<T>(T instance) : SingletonInstanceAccessorBase<T>, ISingletonInstanceAccessor<T> where T : class
    {
        public override bool IsCreated => true;

        public override T Access() => instance;
    }
}
