using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DIContainer.Exceptions;

namespace DIContainer
{
    internal class BuildedCollection : IBuildedCollection
    {
        private readonly IDictionary<Type, object> _createdSingletons = new Dictionary<Type, object>();

        private readonly IDictionary<Type, Type> _singletons;
        private readonly IDictionary<Type, Type> _scoped;

        internal BuildedCollection(
            IDictionary<Type, Type> singletons,
            IDictionary<Type, Type> scoped)
        {
            _singletons = singletons;
            _scoped = scoped;

            //BuildAllSingletons();
        }

        public T Get<T>() where T : class => (T)GetObjectFromCollections(typeof(T));

        public object Get(Type type) => GetObjectFromCollections(type);

        private object GetObjectFromCollections(Type type)
        {
            if (_singletons.ContainsKey(type))
            {
                return GetOrCreateSingleton(type);
            }

            if (_scoped.ContainsKey(type))
            {
                return CreateFromCollectionByConstructor(_scoped[type]);
            }

            return null;
        }

        private object GetOrCreateSingleton(Type type)
        {
            if (_createdSingletons.ContainsKey(type))
            {
                return _createdSingletons[type];
            }
            else
            {
                object obj = CreateFromCollectionByConstructor(_singletons[type]);
                _createdSingletons[type] = obj;
                return obj;
            }
        }


        //private void BuildAllSingletons()
        //{
        //    foreach (var singleton in _singletons)
        //    {
        //        object obj = CreateFromCollectionByConstructor(singleton.Value);
        //        _createdSingletons[singleton.Key] = obj;
        //    }
        //}

        private object CreateFromCollectionByConstructor(Type classType)
        {
            var ctor = GetCallableConstructor(classType.GetConstructors(BindingFlags.Public | BindingFlags.Instance));

            if (ctor == null) throw new DiException($"There are no callable constructor for type {classType.Name}");

            object[] ctorParams = GetConstructorParams(ctor);

            object instance = ctor.Invoke(ctorParams);

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
            if (!parameters.Any()) return Array.Empty<object>();

            foreach (var parameter in parameters)
            {
                if (_singletons.ContainsKey(parameter.ParameterType))
                {
                    ctorData.Add(GetOrCreateSingleton(parameter.ParameterType));
                    continue;
                }

                var type = FindType(parameter.ParameterType);

                var ctor = GetCallableConstructor(type.GetConstructors());

                var paramCtor = ctor;

                if (paramCtor == null)
                {
                    throw new DiException($"Cannot create param`s ctor of type {parameter.ParameterType.Name}");
                }

                object param = paramCtor.Invoke(GetConstructorParamsInner(paramCtor));

                ctorData.Add(param);
            }

            return ctorData.ToArray();
        }

        private Type FindType(Type type)
        {
            if(_singletons.ContainsKey(type))
            {
                return _singletons[type];
            }
            if(_singletons.TryGetValue(type, out var singleton))
            {
                return singleton;
            }
            if(_scoped.ContainsKey(type))
            {
                return _scoped[type];
            }
            if(_scoped.TryGetValue(type, out var scopedType))
            {
                return scopedType;
            }

            return null;
        }

        private ConstructorInfo GetCallableConstructor(IEnumerable<ConstructorInfo> constructors)
        {
            ConstructorInfo resultConstructor = null;

            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();

                if (!parameters.Any())
                {
                    return constructor;
                }

                bool canCreate = false;

                foreach (var parameter in parameters)
                {
                    bool haveSingleton = _singletons.ContainsKey(parameter.ParameterType);
                    bool haveInScope = _scoped.ContainsKey(parameter.ParameterType);

                    if (!haveSingleton && !haveInScope)
                    {
                        break;
                    }

                    canCreate = true;
                }

                if (canCreate) return constructor;
            }

            return resultConstructor;
        }
    }
}