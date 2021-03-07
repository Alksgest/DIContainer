using System;
using System.Collections.Generic;

namespace DIContainer
{
    public class DependencyCollection : IDependencyCollection
    {
        private readonly IDictionary<Type, Type> _scoped = new Dictionary<Type, Type>();
        private readonly IDictionary<Type, Type> _singletons = new Dictionary<Type, Type>();

        public IDependencyCollection RegisterSingleton<TClass>() where TClass : class
        {
            if (_singletons.ContainsKey(typeof(TClass)))
            {
                return this;
            }

            _singletons[typeof(TClass)] = typeof(TClass);

            return this;
        }

        public IDependencyCollection RegisterSingleton<TInterface, TClass>() where TClass : class, TInterface
        {
            if (_singletons.ContainsKey(typeof(TInterface)))
            {
                return this;
            }

            _singletons[typeof(TInterface)] = typeof(TClass);

            return this;
        }

        public IDependencyCollection RegisterScoped<TClass>() where TClass : class
        {
            if (_scoped.ContainsKey(typeof(TClass)))
            {
                return this;
            }

            _scoped[typeof(TClass)] = typeof(TClass);

            return this;
        }

        public IDependencyCollection RegisterScoped<TInterface, TClass>() where TClass : class, TInterface
        {
            if (_scoped.ContainsKey(typeof(TInterface)))
            {
                return this;
            }

            _scoped[typeof(TInterface)] = typeof(TClass);

            return this;
        }

        public IBuildedCollection Build()
        {
            return BuildedCollection.BuildCollection(_singletons, _scoped);
        }
    }
}