namespace DIContainer.Test.TestModels
{
    public interface IService
    {
        IRepository Repo { get; }
    }
}