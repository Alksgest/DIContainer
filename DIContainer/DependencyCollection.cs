using System;
using System.Collections.Generic;

namespace DIContainer
{
    public class DependencyCollection : IDependencyCollection
    {
        private readonly IDictionary<string, Type> _scoped = new Dictionary<string, Type>();
        private readonly IDictionary<string, Type> _singletons = new Dictionary<string, Type>();

        internal DependencyCollection()
        {
        }

        public IDependencyCollection RegisterSingleton<TClass>() where TClass : class
        {
            var (type, fullTypeName) = GetTypeAndTypeFullName<TClass>();

            if (!_singletons.ContainsKey(fullTypeName))
            {
                _singletons[fullTypeName] = type;
            }

            return this;
        }

        public IDependencyCollection RegisterSingleton<TInterface, TClass>() where TClass : class, TInterface
        {
            var type = typeof(TInterface);
            var fullInterfaceName = type.FullName;

            if (fullInterfaceName == null)
            {
                // TODO: make an exception
                throw new Exception();
            }

            if (!_singletons.ContainsKey(fullInterfaceName))
            {
                _singletons[fullInterfaceName] = typeof(TClass);
            }

            return this;
        }

        public IDependencyCollection RegisterScoped<TClass>() where TClass : class
        {
            var (type, fullTypeName) = GetTypeAndTypeFullName<TClass>();

            if (!_scoped.ContainsKey(fullTypeName))
            {
                _scoped[fullTypeName] = type;
            }

            return this;
        }

        public IDependencyCollection RegisterScoped<TInterface, TClass>() where TClass : class, TInterface
        {
            var (_, fullInterfaceName) = GetTypeAndTypeFullName<TInterface>();

            if (!_scoped.ContainsKey(fullInterfaceName))
            {
                _scoped[fullInterfaceName] = typeof(TClass);
            }

            return this;
        }

        public IBuiltCollection Build()
        {
            return new BuiltCollection(_singletons, _scoped);
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
}