namespace DIContainer;

public interface IDependencyCollection
{
    IDependencyCollection RegisterSingleton<TClass>() where TClass : class;
    IDependencyCollection RegisterSingleton<TInterface, TClass>() where TClass : class, TInterface;

    IDependencyCollection RegisterTransient<TClass>() where TClass : class;
    IDependencyCollection RegisterTransient<TInterface, TClass>() where TClass : class, TInterface;

    IBuiltCollection Build();
}