using System;

namespace DIContainer
{
    public interface IBuildedCollection
    {
        object Get(Type type);
        T Get<T>() where T : class;
    }
}
