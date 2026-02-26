using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DIContainer.Exceptions;

namespace DIContainer
{
    internal class BuiltCollection : IBuiltCollection
    {
        private readonly IDictionary<string, object> _createdSingletons = new Dictionary<string, object>();

        private readonly IDictionary<string, Type> _singletons;
        private readonly IDictionary<string, Type> _scoped;

        internal BuiltCollection(
            IDictionary<string, Type> singletons,
            IDictionary<string, Type> scoped)
        {
            _singletons = singletons;
            _scoped = scoped;

            BuildAllSingletons();
        }

        public T Get<T>() where T : class
        {
            var type = typeof(T);
            var fullTypeName = type.FullName!;

            if (_createdSingletons.TryGetValue(fullTypeName, out var singleton))
            {
                return (T)singleton;
            }

            if (_scoped.TryGetValue(fullTypeName, out var typeToCreate))
            {
                return (T)CreateFromDiByConstructor(typeToCreate);
            }

            //TODO: throw an exception
            
            return null;
        }

        private void BuildAllSingletons()
        {
            foreach (var singleton in _singletons)
            {
                var obj = CreateFromDiByConstructor(singleton.Value);
                _createdSingletons[singleton.Key] = obj;
            }
        }

        private object CreateFromDiByConstructor(Type classType)
        {
            var ctor = GetCallableConstructor(classType.GetConstructors(BindingFlags.Public | BindingFlags.Instance));

            if (ctor == null)
            {
                throw new DiException($"There are no callable constructor for type {classType.Name}");
            }

            var ctorParams = GetConstructorParams(ctor);

            var instance = ctor.Invoke(ctorParams);

            return instance;
        }

        private object[] GetConstructorParams(ConstructorInfo constructor)
        {
            return GetConstructorParamsInner(constructor);
        }

        private object[] GetConstructorParamsInner(ConstructorInfo constructor)
        {
            var ctorData = new List<object>(4);

            var parameters = constructor.GetParameters();
            if (!parameters.Any())
            {
                return Array.Empty<object>();
            }

            foreach (var parameter in parameters)
            {
                var fullTypeName = parameter.ParameterType.FullName!;
                
                if (_createdSingletons.TryGetValue(fullTypeName, out var singleton))
                {
                    ctorData.Add(singleton);
                    continue;
                }

                var type = FindType(parameter.ParameterType);

                var ctor = GetCallableConstructor(type.GetConstructors());

                var paramCtor = ctor;

                if (paramCtor == null)
                {
                    throw new DiException($"Cannot create param`s ctor of type {parameter.ParameterType.Name}");
                }

                var param = paramCtor.Invoke(GetConstructorParamsInner(paramCtor));

                ctorData.Add(param);
            }

            return ctorData.ToArray();
        }

        private Type FindType(Type type)
        {
            var fullTypeName = type.FullName!;
            
            if (_singletons.TryGetValue(fullTypeName, out var foundSingleton))
            {
                return foundSingleton;
            }

            if (_singletons.TryGetValue(fullTypeName, out var singleton))
            {
                return singleton;
            }

            if (_scoped.TryGetValue(fullTypeName, out var foundScoped))
            {
                return foundScoped;
            }

            if (_scoped.TryGetValue(fullTypeName, out var scopedType))
            {
                return scopedType;
            }

            return null;
        }

        private ConstructorInfo GetCallableConstructor(IEnumerable<ConstructorInfo> constructors)
        {
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();

                if (!parameters.Any())
                {
                    return constructor;
                }

                var canCreate = false;

                foreach (var parameter in parameters)
                {
                    var fullTypeName = parameter.ParameterType.FullName!;
                    
                    var haveSingleton = _singletons.ContainsKey(fullTypeName);
                    var haveInScope = _scoped.ContainsKey(fullTypeName);

                    if (!haveSingleton && !haveInScope)
                    {
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
}