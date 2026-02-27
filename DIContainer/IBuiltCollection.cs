namespace DIContainer;

public interface IBuiltCollection
{
    T Get<T>() where T : class;
}