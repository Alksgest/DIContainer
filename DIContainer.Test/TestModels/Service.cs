namespace DIContainer.Test.TestModels
{
    public class Service: IService
    {
        private readonly IRepository _repo;

        public IRepository Repo => _repo;

        public Service(IRepository repo)
        {
            _repo = repo;
        }
    }
}