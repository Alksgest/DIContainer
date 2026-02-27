using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DIContainer.Exceptions;
using DIContainer.Models;

namespace DIContainer;

internal class BuiltCollection : IBuiltCollection
{
    private readonly Dictionary<string, object> _createdSingletons = new();
    private readonly Dictionary<string, RegisteredType> _registeredTypes;

    internal BuiltCollection(Dictionary<string, RegisteredType> registeredTypes)
    {
        _registeredTypes = registeredTypes;
    }

    public T Get<T>() where T : class
    {
        var type = typeof(T);
        var fullTypeName = type.FullName!;

        if (!_registeredTypes.TryGetValue(fullTypeName, out var registeredType))
        {
            //TODO: throw an exception
            return null;
        }
        
        if (registeredType.Lifetime == Lifetime.Singleton)
        {
            return GetSingleton<T>(registeredType.Type, fullTypeName);
        }
            
        return (T)CreateFromDiByConstructor(registeredType.Type);
    }

    private T GetSingleton<T>(Type type, string fullTypeName) where T : class
    {
        if (_createdSingletons.TryGetValue(fullTypeName, out var singleton))
        {
            return (T)singleton;
        }
        
        var obj = CreateFromDiByConstructor(type, true);
        _createdSingletons[fullTypeName] = obj;

        return obj as T;
    }
    

    private object CreateFromDiByConstructor(Type classType, bool isSingleton = false)
    {
        var ctor = GetCallableConstructor(classType.GetConstructors(BindingFlags.Public | BindingFlags.Instance));

        if (ctor == null)
        {
            throw new DiException($"There are no callable constructor for type {classType.Name}");
        }

        var ctorParams = GetConstructorParams(ctor, isSingleton);
        var instance = ctor.Invoke(ctorParams);

        return instance;
    }

    private object[] GetConstructorParams(ConstructorInfo constructor, bool isSingleton = false)
    {
        var ctorData = new List<object>(4);

        var parameters = constructor.GetParameters();
        if (parameters.Length == 0)
        {
            return [];
        }

        foreach (var parameter in parameters)
        {
            var fullTypeName = parameter.ParameterType.FullName!;

            if (_createdSingletons.TryGetValue(fullTypeName, out var singleton))
            {
                ctorData.Add(singleton);
                continue;
            }

            var fullName = parameter.ParameterType.FullName;
            if (!_registeredTypes.TryGetValue(fullName!, out var registeredType))
            {
                throw new DiException($"Cannot create param of type {parameter.ParameterType.Name}");
            }

            if (isSingleton && registeredType.Lifetime != Lifetime.Singleton)
            {
                //TODO: make an exception
                throw new Exception("Singleton cannot depend on transient");
            }

            var type = registeredType.Type;
            var ctor = GetCallableConstructor(type.GetConstructors());

            if (ctor == null)
            {
                throw new DiException($"Cannot create param`s ctor of type {parameter.ParameterType.Name}");
            }

            var param = ctor.Invoke(GetConstructorParams(ctor));

            ctorData.Add(param);
        }

        return ctorData.ToArray();
    }
    

    private ConstructorInfo GetCallableConstructor(IEnumerable<ConstructorInfo> constructors)
    {
        foreach (var constructor in constructors)
        {
            var parameters = constructor.GetParameters();

            if (parameters.Length == 0)
            {
                return constructor;
            }

            var canCreate = false;

            foreach (var parameter in parameters)
            {
                var fullTypeName = parameter.ParameterType.FullName!;

                var isRegistered = _registeredTypes.ContainsKey(fullTypeName);

                if (!isRegistered)
                {
                    // TODO: throw an exception
                    break;
                }

                canCreate = true;
            }

            if (canCreate)
            {
                return constructor;
            }
        }

        // TODO: throw an exception
        return null;
    }
}