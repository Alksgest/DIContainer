using System;
using System.Collections.Generic;
using DIContainer.Models;

namespace DIContainer;

public class DependencyCollection : IDependencyCollection
{
    private readonly Dictionary<string, RegisteredType> _registeredTypes = new();

    internal DependencyCollection()
    {
    }

    public IDependencyCollection RegisterSingleton<TClass>() where TClass : class
    {
        var (type, fullTypeName) = GetTypeAndTypeFullName<TClass>();

        _registeredTypes.TryAdd(fullTypeName, new RegisteredType
        {
            Lifetime = Lifetime.Singleton,
            Type = type
        });

        return this;
    }

    public IDependencyCollection RegisterSingleton<TInterface, TClass>() where TClass : class, TInterface
    {
        var (_, fullInterfaceName) = GetTypeAndTypeFullName<TInterface>();
        
        _registeredTypes.TryAdd(fullInterfaceName, new RegisteredType
        {
            Lifetime = Lifetime.Singleton,
            Type = typeof(TClass)
        });

        return this;
    }

    public IDependencyCollection RegisterTransient<TClass>() where TClass : class
    {
        var (type, fullTypeName) = GetTypeAndTypeFullName<TClass>();

        _registeredTypes.TryAdd(fullTypeName, new RegisteredType
        {
            Lifetime = Lifetime.Transient,
            Type = type
        });

        return this;
    }

    public IDependencyCollection RegisterTransient<TInterface, TClass>() where TClass : class, TInterface
    {
        var (_, fullInterfaceName) = GetTypeAndTypeFullName<TInterface>();

        _registeredTypes.TryAdd(fullInterfaceName, new RegisteredType
        {
            Lifetime = Lifetime.Transient,
            Type = typeof(TClass)
        });

        return this;
    }

    public IBuiltCollection Build()
    {
        return new BuiltCollection(_registeredTypes);
    }

    private static (Type, string) GetTypeAndTypeFullName<T>()
    {
        var type = typeof(T);
        var fullTypeName = type.FullName;

        if (fullTypeName == null)
        {
            // TODO: make an exception
            throw new Exception();
        }

        return (type, fullTypeName);
    }
}