using System;
using DIContainer.Exceptions;
using DIContainer.Factories;
using DIContainer.Test.TestModels;
using Xunit;

namespace DIContainer.Test
{
    public class BuildedCollectionTest
    {
        [Fact]
        public void SingletonTest1()
        {
            var collection = CollectionFactory.CreateCollection();

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
            var collection = CollectionFactory.CreateCollection();

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
            var collection = CollectionFactory.CreateCollection();

            collection.RegisterSingleton<Repository>();

            var builder = collection.Build();

            var repo1 = builder.Get<Repository>();
            var repo2 = builder.Get<Repository>();

            Assert.Same(repo1, repo2);
        }

        [Fact]
        public void SingletonTest4()
        {
            var collection = CollectionFactory.CreateCollection();

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
            var collection = CollectionFactory.CreateCollection();

            collection.RegisterScoped<Repository>();

            var builder = collection.Build();

            var repo1 = builder.Get<Repository>();
            var repo2 = builder.Get<Repository>();

            Assert.NotSame(repo1, repo2);
        }

        [Fact]
        public void ScopedTest2()
        {
            var collection = CollectionFactory.CreateCollection();

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
