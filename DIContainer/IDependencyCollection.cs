namespace DIContainer
{
    public interface IDependencyCollection
    {
        IDependencyCollection RegisterSingleton<TClass>() where TClass : class;
        IDependencyCollection RegisterSingleton<TInterface, TClass>() where TClass : class, TInterface;

        IDependencyCollection RegisterScoped<TClass>() where TClass : class;
        IDependencyCollection RegisterScoped<TInterface, TClass>() where TClass : class, TInterface;

        IBuildedCollection Build();
    }
}