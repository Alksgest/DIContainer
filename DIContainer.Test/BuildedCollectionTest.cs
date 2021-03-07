using System;
using DIContainer.Exceptions;
using Xunit;

namespace DIContainer.Test
{
    public class Repository : IRepository
    {

    }

    public interface IRepository
    {
    }

    public class Service: IService
    {
        private readonly IRepository _repo;

        public IRepository Repo => _repo;

        public Service(IRepository repo)
        {
            _repo = repo;
        }
    }

    public interface IService
    {
        IRepository Repo { get; }
    }

    public class BuildedCollectionTest
    {
        [Fact]
        public void SingletonTest1()
        {
            IDependencyCollection collection = new DependencyCollection();

            collection
                .RegisterSingleton<IRepository, Repository>()
                .RegisterSingleton<IService, Service>();

            var builder = collection.Build();

            var repo = builder.Get<IRepository>();
            var service = builder.Get<IService>();

            Assert.NotNull(repo);
            Assert.NotNull(service);
        }

        [Fact]
        public void SingletonTest2()
        {
            IDependencyCollection collection = new DependencyCollection();

            collection.RegisterSingleton<Repository>();

            var builder = collection.Build();

            var repo = builder.Get<Repository>();

            Assert.NotNull(repo);

            collection.RegisterSingleton<Service>();

            try
            {
                var _ = collection.Build();
            }
            catch (Exception e)
            {
                var exceptionType = typeof(DiException);
                Assert.IsType(exceptionType, e);
            }
        }

        [Fact]
        public void SingletonTest3()
        {
            IDependencyCollection collection = new DependencyCollection();

            collection.RegisterSingleton<Repository>();

            var builder = collection.Build();

            var repo1 = builder.Get<Repository>();
            var repo2 = builder.Get<Repository>();

            Assert.Same(repo1, repo2);
        }

        [Fact]
        public void SingletonTest4()
        {
            IDependencyCollection collection = new DependencyCollection();

            collection
                .RegisterSingleton<IRepository, Repository>()
                .RegisterSingleton<IService, Service>();

            var builder = collection.Build();

            var service1 = builder.Get<IService>();
            var service2 = builder.Get<IService>();

            Assert.Same(service1, service2);

            var repo1 = service1.Repo;
            var repo2 = service2.Repo;

            Assert.Same(repo1, repo2);
        }

        [Fact]
        public void ScopedTest1()
        {
            IDependencyCollection collection = new DependencyCollection();

            collection.RegisterScoped<Repository>();

            var builder = collection.Build();

            var repo1 = builder.Get<Repository>();
            var repo2 = builder.Get<Repository>();

            Assert.NotSame(repo1, repo2);
        }

        [Fact]
        public void ScopedTest2()
        {
            IDependencyCollection collection = new DependencyCollection();

            collection
                .RegisterSingleton<IRepository, Repository>()
                .RegisterScoped<Service>(); ;

            var builder = collection.Build();

            var repo1 = builder.Get<IRepository>();
            var repo2 = builder.Get<IRepository>();

            Assert.Same(repo1, repo2);

            var service1 = builder.Get<Service>();
            var service2 = builder.Get<Service>();

            Assert.NotSame(service1, service2);

            var r1 = service1.Repo;
            var r2 = service2.Repo;

            Assert.Same(r1, r2);
        }
    }
}
